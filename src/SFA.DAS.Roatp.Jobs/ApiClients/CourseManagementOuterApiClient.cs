using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SFA.DAS.Roatp.Jobs.ApiClients;

[ExcludeFromCodeCoverage]
public class CourseManagementOuterApiClient : ICourseManagementOuterApiClient
{
    protected readonly HttpClient _httpClient;
    protected readonly ILogger<CourseManagementOuterApiClient> _logger;

    protected const string _contentType = "application/json";

    public static JsonSerializerOptions JsonSerializerOptions => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public CourseManagementOuterApiClient(HttpClient httpClient, ILogger<CourseManagementOuterApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<(bool, T)> Get<T>(string uri)
    {
        try
        {
            using (var response = await _httpClient.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                var content = default(T);
                if (response.IsSuccessStatusCode)
                {
                    content = await response.Content.ReadFromJsonAsync<T>(JsonSerializerOptions);
                    return (true, content);
                }
                await LogErrorIfUnsuccessfulResponse(response);
                return (false, default(T));
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error when processing request: Get - {Uri}", uri);
            throw;
        }
    }

    public async Task<(bool, U)> Post<T, U>(string uri, T model)
    {
        var serializeObject = JsonConvert.SerializeObject(model);

        try
        {
            using (HttpResponseMessage response = await _httpClient.PostAsync(new Uri(uri, UriKind.Relative),
                       new StringContent(serializeObject, Encoding.UTF8,
                           _contentType)))
            {
                if (response.IsSuccessStatusCode)
                {
                    return (true, await response.Content.ReadFromJsonAsync<U>());
                }

                await LogErrorIfUnsuccessfulResponse(response);
                return (false, default(U));

            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Error when processing request: Post - {Uri}", uri);
            throw;
        }
    }

    public async Task Delete(string uri, CancellationToken cancellationToken)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Delete, uri);

        var response = await _httpClient.SendAsync(requestMessage, cancellationToken);

        response.EnsureSuccessStatusCode();
    }

    private async Task LogErrorIfUnsuccessfulResponse(HttpResponseMessage response)
    {
        var callingMethod = new System.Diagnostics.StackFrame(1).GetMethod().Name;

        var httpMethod = response.RequestMessage.Method.ToString();
        var statusCode = (int)response.StatusCode;
        var reasonPhrase = response.ReasonPhrase;
        var requestUri = response.RequestMessage.RequestUri;

        var responseContent = await response.Content.ReadAsStringAsync();
        var apiErrorMessage = responseContent;

        _logger.LogError("Method: {CallingMethod} || HTTP {StatusCode} {ReasonPhrase} || {HttpMethod}: {RequestUri} || Message: {ApiErrorMessage}", callingMethod, statusCode, reasonPhrase, httpMethod, requestUri, apiErrorMessage);
    }
}
