using AutoFixture.NUnit3;
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
        public void MainProvider_PopulatesModelFromEntity_Property_ReturnsTrue(ProviderRegistrationDetail source)
        {
            source.ProviderTypeId = (int) ProviderType.Main;
            source.StatusId = (int) ProviderStatusType.Active;
            var model = (ProviderSummary)source;

            model.CanAccessApprenticeshipService.Should().BeTrue();
        }

        [Test, RecursiveMoqAutoData]
        public void EmployerProvider_PopulatesModelFromEntity_Property_ReturnsTrue(ProviderRegistrationDetail source)
        {
            source.ProviderTypeId = (int)ProviderType.Employer;
            source.StatusId = (int)ProviderStatusType.Active;
            var model = (ProviderSummary)source;

            model.CanAccessApprenticeshipService.Should().BeTrue();
        }

        [Test, RecursiveMoqAutoData]
        public void SupportingProvider_PopulatesModelFromEntity_Property_ReturnsFalse(ProviderRegistrationDetail source)
        {
            source.ProviderTypeId = (int)ProviderType.Supporting;
            source.StatusId = (int)ProviderStatusType.Active;
            var model = (ProviderSummary)source;

            model.CanAccessApprenticeshipService.Should().BeFalse();
        }

        [Test, RecursiveMoqAutoData]
        public void Status_Onboarding_PopulatesModelFromEntity_Property_ReturnsFalse(ProviderRegistrationDetail source)
        {
            source.ProviderTypeId = (int)ProviderType.Supporting;
            source.StatusId = (int)ProviderStatusType.Onboarding;
            var model = (ProviderSummary)source;

            model.CanAccessApprenticeshipService.Should().BeFalse();
        }

        [Test, RecursiveMoqAutoData]
        public void Status_ActiveButNotTakingOnApprentices_PopulatesModelFromEntity_Property_ReturnsFalse(ProviderRegistrationDetail source)
        {
            source.ProviderTypeId = (int)ProviderType.Main;
            source.StatusId = (int)ProviderStatusType.ActiveButNotTakingOnApprentices;
            var model = (ProviderSummary)source;

            model.CanAccessApprenticeshipService.Should().BeFalse();
        }
    }
}
