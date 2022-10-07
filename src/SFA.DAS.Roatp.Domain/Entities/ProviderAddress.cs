using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderAddress
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime? AddressUpdateDate { get; set; }
        public DateTime? CoordinatesUpdateDate { get; set; }

        public virtual Provider Provider { get; set; }
        //MFCMFC public static implicit operator ProviderAddress(UkrlpProviderAddress source)
        //    => new ProviderAddress
        //    {
          //      ProviderId = source.ProviderId,
            //    AddressLine1 = source.Address1,
              //  AddressLine2 = source.Address2,
                //AddressLine3 = source.Address3,
        //        AddressLine4 = source.Address4,
          //      Town=source.Town,
            //    Postcode = source.Postcode,
              //  AddressUpdateDate = DateTime.Now,
                //Id = source.Id
            //};
    }
}