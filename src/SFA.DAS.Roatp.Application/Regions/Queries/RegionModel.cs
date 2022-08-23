namespace SFA.DAS.Roatp.Application.Regions.Queries
{
    public class RegionModel
    {
        public int Id { get; set; }
        public string SubregionName { get; set; }
        public string RegionName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public static implicit operator RegionModel(Domain.Entities.Region source) =>
            new RegionModel
            {
                Id = source.Id,
                SubregionName = source.SubregionName,
                RegionName = source.RegionName,
                Latitude = source.Latitude,
                Longitude = source.Longitude,
            };
    }
}
