using System.Net.Sockets;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory
{
    public class CdProviderLocation
    {
        // put in class???
        const decimal NationalLatitude = (decimal)52.564269;
        const decimal NationalLongitude = (decimal)-1.466056;

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

        public decimal? Latitude
        {
            get
            {
                if (decimal.TryParse(Address?.Lat, out var latitude))
                {
                    return latitude;
                }

                return null;
            }
        }

        public decimal? Longitude
        {
            get
            {
                if (decimal.TryParse(Address?.Long, out var longitude))
                {
                    return longitude;
                }

                return null;
            }
        }

        public LocationType LocationType {

            get
            {
                if (AddressLine1 == Name && Town == null && Postcode == null)
                {
                    if (Name == null && Latitude == NationalLatitude && Longitude == NationalLongitude)
                    {
                        return LocationType.National;
                    }
                    else
                    {
                        return LocationType.Regional;
                    }
                }

                return LocationType.Provider;
            }
        }
    }
}
