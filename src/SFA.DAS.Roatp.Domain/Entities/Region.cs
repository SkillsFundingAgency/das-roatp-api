using System.Collections.Generic;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class Region
    {
        public int Id { get; set; }
        public string SubregionName { get; set; }
        public string RegionName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public virtual List<ProviderLocation> Locations { get; set; } = new List<ProviderLocation>();
    }
}
