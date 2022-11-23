using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Api.Infrastructure;

[ExcludeFromCodeCoverage]
public static class Constants
{
    public static class EndpointGroups
    {
        public const string Integration = nameof(Integration);
        public const string Operation = nameof(Operation);
    }
}
