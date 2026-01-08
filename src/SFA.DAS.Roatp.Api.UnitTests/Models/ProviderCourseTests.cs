using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.UnitTests.Models;

[TestFixture]
public class ProviderCourseTests
{
    [TestCase(true)]
    [TestCase(false)]
    [TestCase(null)]
    public void ImplicitOperator_ReturnsCommand(bool? isApprovedByRegulator)
    {
        var contactUsEmail = "test@test.com";
        var contactUsPhoneNumber = "1234567890";
        var standardInfoUrl = "http://www.test.com";
        var hasOnlineDeliveryOption = true;

        var entity = new ProviderCourse
        {
            ContactUsEmail = contactUsEmail,
            ContactUsPhoneNumber = contactUsPhoneNumber,
            StandardInfoUrl = standardInfoUrl,
            IsApprovedByRegulator = isApprovedByRegulator,
            HasOnlineDeliveryOption = hasOnlineDeliveryOption
        };

        var patchProviderCourse = new ProviderCourse
        {
            ContactUsEmail = contactUsEmail,
            ContactUsPhoneNumber = contactUsPhoneNumber,
            StandardInfoUrl = standardInfoUrl,
            IsApprovedByRegulator = isApprovedByRegulator,
            HasOnlineDeliveryOption = hasOnlineDeliveryOption
        };

        var expectedPatchProvider = (Domain.Models.ProviderCourse)entity;

        expectedPatchProvider.Should().BeEquivalentTo(patchProviderCourse, c => c
            .Excluding(s => s.Id)
            .Excluding(s => s.ProviderId)
            .Excluding(s => s.Provider)
            .Excluding(s => s.Locations)
            .Excluding(s => s.Versions)
            .Excluding(s => s.Standard)
        );
    }


    [Test]
    public void ImplicitOperator_NullEntityReturnsNull()
    {
        var expectedPatchProvider = (Domain.Models.ProviderCourse)null;
        expectedPatchProvider.Should().BeNull();
    }
}