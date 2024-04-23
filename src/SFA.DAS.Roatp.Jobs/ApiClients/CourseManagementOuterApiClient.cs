using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SFA.DAS.Roatp.Jobs.ApiClients;

public class CourseManagementOuterApiClient : ICourseManagementOuterApiClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger<CourseManagementOuterApiClient> Logger;

    protected const string ContentType = "application/json";

    public CourseManagementOuterApiClient(HttpClient httpClient, ILogger<CourseManagementOuterApiClient> logger)
    {
        _httpClient = httpClient;
        Logger = logger;
    }

    public async Task<(bool, T)> Get<T>(string uri)
    {
        try
        {
            using var response = await _httpClient.GetAsync(new Uri(uri, UriKind.Relative));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsAsync<T>();
                return (true, content);
            }
            await LogErrorIfUnsuccessfulResponse(response);
            return (false, default(T));
        }
        catch (HttpRequestException ex)
        {
            Logger.LogError(ex, "Error when processing request: GET - {Uri}", uri);
            throw;
        }
    }

    public async Task<(bool, U)> Post<T, U>(string uri, T model)
    {
        var serializeObject = JsonConvert.SerializeObject(model);

        try
        {
            using (var response = await _httpClient.PostAsync(new Uri(uri, UriKind.Relative),
                       new StringContent(serializeObject, Encoding.UTF8,
                           ContentType)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return (true, await response.Content.ReadAsAsync<U>());
                }

                await LogErrorIfUnsuccessfulResponse(response);
                return (false, default(U));

            }
        }
        catch (HttpRequestException ex)
        {
            Logger.LogError(ex, "Error when processing request: POST - {Uri}", uri);
            throw;
        }
    }

    private async Task LogErrorIfUnsuccessfulResponse(HttpResponseMessage response)
    {
        var callingMethod = new System.Diagnostics.StackFrame(1).GetMethod()?.Name;

        var httpMethod = response.RequestMessage?.Method.ToString();
        var statusCode = (int)response.StatusCode;
        var reasonPhrase = response.ReasonPhrase;
        var requestUri = response.RequestMessage?.RequestUri;

        var responseContent = await response.Content.ReadAsStringAsync();
        var apiErrorMessage = responseContent;

        Logger.LogError($"Method: {callingMethod} || HTTP {statusCode} {reasonPhrase} || {httpMethod}: {requestUri} || Message: {apiErrorMessage}");
    }
}
