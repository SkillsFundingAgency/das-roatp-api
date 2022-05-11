using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;

namespace SFA.DAS.Roatp.Jobs.Infrastructure.Tokens
{
    [ExcludeFromCodeCoverage]
    public static class BearerTokenGenerator
    {
        private const string JwtBearerScheme = "Bearer";

        public static async Task<AuthenticationHeaderValue> GenerateTokenAsync(string identifier)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var accessToken = await azureServiceTokenProvider.GetAccessTokenAsync(identifier);

            return new AuthenticationHeaderValue(JwtBearerScheme, accessToken);
        }
    }
}