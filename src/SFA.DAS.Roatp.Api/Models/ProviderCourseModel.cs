using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseModel
    {
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public List<string> DeliveryModels { get; set; } = new List<string>();

        public static implicit operator ProviderCourseModel(ProviderCourse providerCourse)
        {
            var model = new ProviderCourseModel
            {
                LarsCode = providerCourse.LarsCode,
                IfateReferenceNumber = providerCourse.IfateReferenceNumber,
            };

            model.DeliveryModels.Add("Regular");

            if (providerCourse.Locations.Any(l => l.OffersPortableFlexiJob.GetValueOrDefault()))
            {
                model.DeliveryModels.Add("PortableFlexiJob");
            }

            return model;
        }
    }

    public static class Constants
    {
        public static class DeliveryModels
        {
            public const string Regular = "Regular";
            public const string PortableFlexiJob = "PortableFlexiJob";
        }
    }
}
