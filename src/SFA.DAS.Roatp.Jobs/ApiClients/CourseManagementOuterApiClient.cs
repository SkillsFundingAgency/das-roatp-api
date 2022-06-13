using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.ApiClients
{
    public class CourseManagementOuterApiClient : ICourseManagementOuterApiClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly ILogger<CourseManagementOuterApiClient> _logger;

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
                        // This code is to sidestep an issue using the mock server, which otherwise generates error
                        // System.Private.CoreLib: Exception while executing function: LoadCourseDirectoryDataFunction. Newtonsoft.Json: Unexpected character encountered while parsing value: [. Path '', line 1, position 1.
                        // Which looks like something to do with file encoding type, but file seems to be UTF which should not cause a problem
                        // but it just doesn't work...
                        var type = typeof(T);
                        if (type == typeof(String))
                        {
                            var dynamicContent = await response.Content.ReadAsAsync<dynamic>();
                            var dynamicContentAsString = dynamicContent.ToString();
                            return (true, dynamicContentAsString);
                        }
                       

                        content = await response.Content.ReadAsAsync<T>();
                        return (true, content);
                    }
                    await LogErrorIfUnsuccessfulResponse(response);
                    return (false, default(T));
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Get} - {uri}");
                throw;
            }
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

            _logger.LogError($"Method: {callingMethod} || HTTP {statusCode} {reasonPhrase} || {httpMethod}: {requestUri} || Message: {apiErrorMessage}");
        }
    }
}
