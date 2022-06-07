﻿using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Roatp.Application.ProviderCourse;

namespace SFA.DAS.Roatp.Api.Models
{
    public class ProviderCourseEditModel
    {
        public string UserId { get; set; }
        public string ContactUsEmail { get; set; }
        public string ContactUsPhoneNumber { get; set; }
        public string ContactUsPageUrl { get; set; }
        public string StandardInfoUrl { get; set; }
        public static implicit operator UpdateProviderCourseCommand(ProviderCourseEditModel model) =>
            new UpdateProviderCourseCommand
            {
                UserId = model.UserId,
                ContactUsEmail = model.ContactUsEmail,
                ContactUsPhoneNumber = model.ContactUsPhoneNumber,
                StandardInfoUrl = model.StandardInfoUrl,
                ContactUsPageUrl = model.ContactUsPageUrl
            };
    }
}
