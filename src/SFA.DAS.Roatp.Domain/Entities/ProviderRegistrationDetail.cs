using System;

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
    }
}
