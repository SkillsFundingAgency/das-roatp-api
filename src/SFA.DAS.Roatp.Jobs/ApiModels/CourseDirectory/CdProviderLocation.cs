using SFA.DAS.Roatp.Domain.Models;
using static SFA.DAS.Roatp.Domain.Constants.Constants;

namespace SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory
{
    public class CdProviderLocation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
       
        public string Email { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }

        public string AddressLine1 => Address?.Address1;
        public string AddressLine2 => Address?.Address2;
        public string Town => Address?.Town;
        public string Postcode => Address?.Postcode;
        public string County => Address?.County;

        public decimal? Latitude => ConvertToDecimal(Address?.Lat);
        public decimal? Longitude => ConvertToDecimal(Address?.Long);

        public LocationType LocationType {
            get
            {
                if (AddressLine1 == Name && Town == null && Postcode == null)
                {
                    if (Name == null && Latitude == NationalLatLong.NationalLatitude && Longitude == NationalLatLong.NationalLongitude)
                    {
                        return LocationType.National;
                    }

                    return LocationType.Regional;
                }

                return LocationType.Provider;
            }
        }

        private decimal? ConvertToDecimal(string conversionString)
        {
            if (decimal.TryParse(conversionString, out var decimalResult))
            {
                return decimalResult;
            }

            return null;
        }
    }
}
