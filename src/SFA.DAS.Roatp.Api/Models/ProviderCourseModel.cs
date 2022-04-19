using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseModel
    {
        public int Id { get; set; }
        public string CourseName { get; set; }
        public int LarsCode { get; set; }
        public int Level { get; set; }
        public string IfateReferenceNumber { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; } = new List<DeliveryModel>();

        public static implicit operator ProviderCourseModel(ProviderCourse providerCourse)
        {
            if (providerCourse == null) return null;

            var model = new ProviderCourseModel
            {
                Id = providerCourse.Id,
                LarsCode = providerCourse.LarsCode,
                IfateReferenceNumber = providerCourse.IfateReferenceNumber,
            };

            //Regular is assumed by default
            model.DeliveryModels.Add(DeliveryModel.Regular);

            //For pilot assume all the standards are flexible
            //For MVS this flag will have to be derived from ProviderCourseLocation
            model.DeliveryModels.Add(DeliveryModel.PortableFlexiJob);

            return model;
        }
    }

    public enum DeliveryModel
    {
        Regular,
        PortableFlexiJob
    }
}
