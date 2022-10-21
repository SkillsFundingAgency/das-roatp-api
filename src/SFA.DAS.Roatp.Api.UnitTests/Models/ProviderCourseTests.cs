using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderCourseTests
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

            var entity = new ProviderCourse
            {
                ContactUsEmail = contactUsEmail,
                ContactUsPageUrl = contactUsPageUrl,
                ContactUsPhoneNumber = contactUsPhoneNumber,
                StandardInfoUrl = standardInfoUrl,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            var patchProviderCourse = new Domain.Entities.ProviderCourse
            {
                ContactUsEmail = contactUsEmail,
                ContactUsPageUrl = contactUsPageUrl,
                ContactUsPhoneNumber = contactUsPhoneNumber,
                StandardInfoUrl = standardInfoUrl,
                IsApprovedByRegulator = isApprovedByRegulator
            };

            var expectedPatchProvider = (Domain.Models.ProviderCourse)entity;

            expectedPatchProvider.Should().BeEquivalentTo(patchProviderCourse, c=>c
                .Excluding(s => s.Id)
                .Excluding(s => s.ProviderId)
                .Excluding(s => s.Provider)
                .Excluding(s => s.Providers)
                .Excluding(s => s.Locations)
                .Excluding(s => s.Versions));
        }


        [Test]
        public void ImplicitOperator_NullEntityReturnsNull()
        {
            var expectedPatchProvider = (Domain.Models.ProviderCourse)null;
            Assert.IsNull(expectedPatchProvider);
        }
    }
}
