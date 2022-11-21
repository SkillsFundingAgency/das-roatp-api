using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Domain.Constants
{
    [ExcludeFromCodeCoverage]
    public static class NationalLatLong
    {
        public const decimal NationalLatitude = 52.564269m;
        public const decimal NationalLongitude = -1.466056m;
        public const decimal MaximumLatitude = 90m;
        public const decimal MinimumLatitude = -90m;
        public const decimal MaximumLongitude = 180m;
        public const decimal MinimumLongitude = -180m;
    }
}
