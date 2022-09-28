using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
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
    public class ReloadNationalAcheivementRatesServiceTests
    {
        [Test]
        [MoqAutoData]
        public async Task ReloadProviderRegistrationDetails_OnApiError_ThrowsInvalidOperationException(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Greedy] ReloadNationalAcheivementRatesService sut)
        {
            apiClientMock.Setup(a => a.Get<NationalAchievementRatesLookup>("lookup/national-achievement-rates")).ReturnsAsync((false, null));
            
            Func<Task> action = () => sut.ReloadNationalAcheivementRates();

            await action.Should().ThrowAsync<InvalidOperationException>();
        }

        [Test]
        [MoqAutoData]
        public async Task ReloadProviderRegistrationDetails_OnApiSuccess_CallsRepositoryReloadMethod(
            [Frozen] Mock<ICourseManagementOuterApiClient> apiClientMock,
            [Frozen] Mock<INationalAchievementRatesImportWriteRepository> nationalAchievementRatesImportWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesImportReadRepository> nationalAchievementRatesImportReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesWriteRepository> nationalAchievementRatesWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesOverallImportWriteRepository> nationalAchievementRatesOverallImportWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesOverallImportReadRepository> nationalAchievementRatesOverallImportReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesOverallWriteRepository> nationalAchievementRatesOverallWriteRepositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Greedy] ReloadNationalAcheivementRatesService sut,
            NationalAchievementRatesLookup data)
        {

            apiClientMock.Setup(a => a.Get<NationalAchievementRatesLookup>("lookup/national-achievement-rates")).ReturnsAsync((true, data));

            await sut.ReloadNationalAcheivementRates();

            nationalAchievementRatesImportWriteRepositoryMock.Verify(r => r.DeleteAll());
            nationalAchievementRatesImportWriteRepositoryMock.Verify(r => r.InsertMany(It.IsAny<List<NationalAchievementRateImport>>()));
            nationalAchievementRatesImportReadRepositoryMock.Verify(r => r.GetAllWithAchievementData());
            nationalAchievementRatesWriteRepositoryMock.Verify(r => r.DeleteAll());
            nationalAchievementRatesWriteRepositoryMock.Verify(r => r.InsertMany(It.IsAny<List<NationalAchievementRate>>()));

            nationalAchievementRatesOverallImportWriteRepositoryMock.Verify(r => r.DeleteAll());
            nationalAchievementRatesOverallImportWriteRepositoryMock.Verify(r => r.InsertMany(It.IsAny<List<NationalAchievementRateOverallImport>>()));
            nationalAchievementRatesOverallImportReadRepositoryMock.Verify(r => r.GetAllWithAchievementData());
            nationalAchievementRatesOverallWriteRepositoryMock.Verify(r => r.DeleteAll());
            nationalAchievementRatesOverallWriteRepositoryMock.Verify(r => r.InsertMany(It.IsAny<List<NationalAchievementRateOverall>>()));
            auditRepositoryMock.Verify(x=>x.Insert(It.IsAny<ImportAudit>()));
        }
    }
}
