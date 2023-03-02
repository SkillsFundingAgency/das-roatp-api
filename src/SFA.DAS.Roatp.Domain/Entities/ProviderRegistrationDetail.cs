using System;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ProviderRegistrationDetail
    {
        public int Ukprn { get; set; }
        public int StatusId { get; set; }
        public DateTime StatusDate { get; set; }
        public int OrganisationTypeId { get; set; }
        public int ProviderTypeId { get; set; }
        public string LegalName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public string Town { get; set; }
        public string Postcode { get; set; }

        public void UpdateAddress(UkrlpProviderAddress source)
        {
            AddressLine1 = source.Address1;
            AddressLine2 = source.Address2;
            AddressLine3 = source.Address3;
            AddressLine4 = source.Address4;
            Town = source.Town;
            Postcode = source.Postcode;
        }
    }
}
