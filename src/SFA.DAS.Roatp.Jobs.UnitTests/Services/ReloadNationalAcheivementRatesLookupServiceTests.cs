using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services
{
    [TestFixture]
    public class ReloadNationalAcheivementRatesLookupServiceTests
    {
        [Test]
        [MoqAutoData]
        public async Task ReloadNationalAcheivementRates_OnApiError_ThrowsInvalidOperationException(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Greedy] ReloadNationalAcheivementRatesLookupService sut)
        {
            apiClientMock.Setup(a => a.Get<NationalAchievementRatesLookup>("lookup/national-achievement-rates")).ReturnsAsync((false, null));
            
            Func<Task> action = () => sut.ReloadNationalAcheivementRates();

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        [MoqAutoData]
        public async Task ReloadNationalAcheivementRates_OnApiSuccess_CallsRepositoryReloadMethod(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<IReloadNationalAcheivementRatesService> reloadNationalAcheivementRatesServiceMock,
            [Frozen] Mock<IReloadNationalAcheivementRatesOverallService> reloadNationalAcheivementRatesOverallServiceMock,
            [Greedy] ReloadNationalAcheivementRatesLookupService sut,
            NationalAchievementRatesLookup data)
        {

            apiClientMock.Setup(a => a.Get<NationalAchievementRatesLookup>("lookup/national-achievement-rates")).ReturnsAsync((true, data));

            await sut.ReloadNationalAcheivementRates();

            reloadNationalAcheivementRatesServiceMock.Verify(r => r.ReloadNationalAcheivementRates(It.IsAny<List<NationalAchievementRatesApiImport>>()));

            reloadNationalAcheivementRatesOverallServiceMock.Verify(r => r.ReloadNationalAcheivementRatesOverall(It.IsAny<List<NationalAchievementRatesOverallApiImport>>()));
        }
    }
}
