using System.Net.Sockets;

namespace SFA.DAS.Roatp.Jobs.ApiModels.CourseDirectory
{
    public class CdProviderLocation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        // public string AddressLine1 { get; set; }
        // public string AddressLine2 { get; set; }
        // public string Town { get; set; }
        // public string Postcode { get; set; }
        // public string County { get; set; }
        // public decimal? Latitude { get; set; }
        // public decimal? Longitude { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Phone { get; set; }
    }

    public class Address
    {
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string County { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string Postcode { get; set; }
        public string Town { get; set; }
    }
}
