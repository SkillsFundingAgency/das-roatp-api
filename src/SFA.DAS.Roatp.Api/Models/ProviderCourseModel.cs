﻿using System.Collections.Generic;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseModel
    {
        public int ProviderCourseId { get; set; }
        public string CourseName { get; set; }
        public int Level { get; set; }
        public int LarsCode { get; set; }
        public string IfateReferenceNumber { get; set; }
        public string StandardInfoUrl { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPageUrl { get; set; }
        public bool? IsApprovedByRegulator { get; set; }
        public bool IsImported { get; set; } = false;
        public bool? IsConfirmed { get; set; } //required if imported
        public bool? HasNationalDeliveryOption { get; set; }
        public bool? HasHundredPercentEmployerDeliveryOption { get; set; }
        public List<DeliveryModel> DeliveryModels { get; set; } = new List<DeliveryModel>();

        public static implicit operator ProviderCourseModel(ProviderCourse providerCourse)
        {
            if (providerCourse == null) return null;

            var model = new ProviderCourseModel
            {
                ProviderCourseId = providerCourse.Id,
                LarsCode = providerCourse.LarsCode,
                IfateReferenceNumber = providerCourse.IfateReferenceNumber,
                StandardInfoUrl = providerCourse.StandardInfoUrl,
                ContactUsPhoneNumber = providerCourse.ContactUsPhoneNumber,
                ContactUsEmail = providerCourse.ContactUsEmail,
                ContactUsPageUrl = providerCourse.ContactUsPageUrl,
                IsApprovedByRegulator = providerCourse.IsApprovedByRegulator,
                IsImported = providerCourse.IsImported,
                IsConfirmed = providerCourse.IsConfirmed,
                HasNationalDeliveryOption = providerCourse.HasNationalDeliveryOption,
                HasHundredPercentEmployerDeliveryOption =providerCourse.HasHundredPercentEmployerDeliveryOption
            };

            //Regular is assumed by default
            model.DeliveryModels.Add(DeliveryModel.Regular);

            //For pilot assume all the standards are flexible
            //For MVS this flag will have to be derived from ProviderCourseLocation
            model.DeliveryModels.Add(DeliveryModel.PortableFlexiJob);

            return model;
        }
        public void UpdateCourseDetails(string ifateRefNum, int level, string title)
        {
            IfateReferenceNumber = ifateRefNum;
            Level = level;
            CourseName = title;
        }
    }

    public enum DeliveryModel
    {
        Regular,
        PortableFlexiJob
    }
}
