using System;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Jobs.UnitTests.ApiModels;

public class RegisteredProviderModelTests
{
    [Test]
    public void ImplicitOperator_MapsPropertiesCorrectly()
    {
        // Arrange
        var registeredProviderModel = new SFA.DAS.Roatp.Jobs.ApiModels.RegisteredProviderModel
        {
            Ukprn = 12345678,
            Status = SFA.DAS.Roatp.Domain.Models.ProviderStatusType.Active,
            StatusDate = DateTime.UtcNow,
            OrganisationTypeId = 2,
            ProviderType = ProviderType.Main,
            LegalName = "Test Legal Name"
        };
        // Act
        Domain.Entities.ProviderRegistrationDetail providerRegistrationDetail = registeredProviderModel;
        // Assert
        Assert.AreEqual(registeredProviderModel.Ukprn, providerRegistrationDetail.Ukprn);
        Assert.AreEqual((int)registeredProviderModel.Status, providerRegistrationDetail.StatusId);
        Assert.AreEqual(registeredProviderModel.StatusDate, providerRegistrationDetail.StatusDate);
        Assert.AreEqual(registeredProviderModel.OrganisationTypeId, providerRegistrationDetail.OrganisationTypeId);
        Assert.AreEqual((int)registeredProviderModel.ProviderType, providerRegistrationDetail.ProviderTypeId);
        Assert.AreEqual(registeredProviderModel.LegalName, providerRegistrationDetail.LegalName);
    }
}
