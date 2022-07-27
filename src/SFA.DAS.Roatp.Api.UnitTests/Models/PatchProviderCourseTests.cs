using System;
using System.Collections.Generic;
using System.Text;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class PatchProviderCourseTests
    {
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void ImplicitOperator_ReturnsCommand(bool? isApprovedByRegulator)
        {
            var contactUsEmail = "test@test.com";
            var contactUsPageUrl = "http://www.test.com/contact-us";
            var contactUsPhoneNumber = "1234567890";
            var standardInfoUrl = "http://www.test.com";

            var entity = new Domain.Entities.ProviderCourse
            {
                ContactUsEmail = contactUsEmail,
                ContactUsPageUrl = contactUsPageUrl,
                ContactUsPhoneNumber = contactUsPhoneNumber,
                StandardInfoUrl = standardInfoUrl,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            var patchProviderCourse = new PatchProviderCourse
            {
                ContactUsEmail = contactUsEmail,
                ContactUsPageUrl = contactUsPageUrl,
                ContactUsPhoneNumber = contactUsPhoneNumber,
                StandardInfoUrl = standardInfoUrl,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            var expectedPatchProvider = (PatchProviderCourse)entity;

            expectedPatchProvider.Should().BeEquivalentTo(patchProviderCourse);
        }
    }
}
