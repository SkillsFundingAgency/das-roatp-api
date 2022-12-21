namespace SFA.DAS.Roatp.Domain.Models
{
    public class PatchProvider
    {
        public string MarketingInfo { get; set; }

        public static implicit operator PatchProvider(Entities.Provider source)
        {
            return new PatchProvider
            {
                MarketingInfo = source.MarketingInfo,
            };
        }
    }
}
