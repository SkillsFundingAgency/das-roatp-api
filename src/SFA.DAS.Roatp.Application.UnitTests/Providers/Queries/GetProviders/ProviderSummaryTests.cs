using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Providers.Queries.GetProviders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetProviders
{
    [TestFixture]
    public class ProviderSummaryTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(ProviderRegistrationDetail source)
        {
            var model = (ProviderSummary)source;

            model.Ukprn.Should().Be(source.Ukprn);
            model.Name.Should().Be(source.LegalName);
            model.ProviderTypeId.Should().Be(source.ProviderTypeId);
            model.StatusId.Should().Be(source.StatusId);
            model.TradingName.Should().Be(source.Provider.TradingName);
            model.Email.Should().Be(source.Provider.Email);
            model.Phone.Should().Be(source.Provider.Phone);
            model.ContactUrl.Should().Be(source.Provider.Website);
        }

        [Test, RecursiveMoqAutoData]
        public void Operator_Populates_Address_Model_From_Entity(ProviderRegistrationDetail source)
        {
            var sut = (ProviderSummary)source;
            Assert.Multiple(() =>
            {
                Assert.That(sut.Address, Is.Not.Null);
                Assert.That(sut.Address.AddressLine1, Is.EqualTo(source.Provider.ProviderAddress.AddressLine1));
                Assert.That(sut.Address.AddressLine2, Is.EqualTo(source.Provider.ProviderAddress.AddressLine2));
                Assert.That(sut.Address.AddressLine3, Is.EqualTo(source.Provider.ProviderAddress.AddressLine3));
                Assert.That(sut.Address.AddressLine4, Is.EqualTo(source.Provider.ProviderAddress.AddressLine4));
                Assert.That(sut.Address.Town, Is.EqualTo(source.Provider.ProviderAddress.Town));
                Assert.That(sut.Address.Postcode, Is.EqualTo(source.Provider.ProviderAddress.Postcode));
                Assert.That(sut.Address.Latitude, Is.EqualTo(source.Provider.ProviderAddress.Latitude));
                Assert.That(sut.Address.Longitude, Is.EqualTo(source.Provider.ProviderAddress.Longitude));
            });
        }

        [TestCase(ProviderType.Supporting, ProviderStatusType.Active, false)]
        [TestCase(ProviderType.Main, ProviderStatusType.Active, true)]
        [TestCase(ProviderType.Main, ProviderStatusType.OnBoarding, true)]
        [TestCase(ProviderType.Main, ProviderStatusType.ActiveNoStarts, true)]
        [TestCase(ProviderType.Main, ProviderStatusType.Removed, false)]
        [TestCase(ProviderType.Employer, ProviderStatusType.Active, true)]
        [TestCase(ProviderType.Employer, ProviderStatusType.OnBoarding, true)]
        [TestCase(ProviderType.Employer, ProviderStatusType.ActiveNoStarts, true)]
        [TestCase(ProviderType.Employer, ProviderStatusType.Removed, false)]
        public void CanAccessApprenticeshipService_ReturnsExpectedValue(ProviderType providerType, ProviderStatusType providerStatusType, bool expected)
        {
            ProviderRegistrationDetail source = new()
            {
                Ukprn = 12345678,
                LegalName = "Test Provider",
                ProviderTypeId = (int)providerType,
                StatusId = (int)providerStatusType,
                Provider = new Provider
                {
                    TradingName = "Test Trading Name",
                    Email = "prov@test.com",
                    Phone = "0123456789",
                    Website = "http://www.testprovider.com",
                },
            };

            ProviderSummary sut = source;

            sut.CanAccessApprenticeshipService.Should().Be(expected);
        }
    }
}
