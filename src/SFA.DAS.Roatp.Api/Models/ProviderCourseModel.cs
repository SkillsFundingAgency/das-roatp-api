using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseModel
    {
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; } = new List<DeliveryModel>();

        public static implicit operator ProviderCourseModel(ProviderCourse providerCourse)
        {
            var model = new ProviderCourseModel
            {
                LarsCode = providerCourse.LarsCode,
                IfateReferenceNumber = providerCourse.IfateReferenceNumber,
            };

            model.DeliveryModels.Add(DeliveryModel.Regular);

            if (providerCourse.Locations.Any(l => l.OffersPortableFlexiJob.GetValueOrDefault()))
            {
                model.DeliveryModels.Add(DeliveryModel.PortableFlexiJob);
            }

            return model;
        }
    }

    public enum DeliveryModel
    {
        Regular,
        PortableFlexiJob
    }
}
