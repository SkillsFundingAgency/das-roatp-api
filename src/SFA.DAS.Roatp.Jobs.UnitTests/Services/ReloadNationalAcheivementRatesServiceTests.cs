using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services
{
    [TestFixture]
    public class ReloadNationalAcheivementRatesServiceTests
    {
        [Test]
        [MoqAutoData]
        public async Task ReloadNationalAcheivementRates_CallsRepositoryReloadMethod(
            [Frozen] Mock<INationalAchievementRatesImportWriteRepository> nationalAchievementRatesImportWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesImportReadRepository> nationalAchievementRatesImportReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesWriteRepository> nationalAchievementRatesWriteRepositoryMock,
            [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Greedy] ReloadNationalAcheivementRatesService sut,
            List<NationalAchievementRatesApiModel> nationalAchievementRatesImported)
        {
            nationalAchievementRatesImportReadRepositoryMock.Setup(c => c.GetAllWithAchievementData()).ReturnsAsync(new List<NationalAchievementRateImport> { new NationalAchievementRateImport { Ukprn = nationalAchievementRatesImported.FirstOrDefault().Ukprn } });

            providersReadRepositoryMock.Setup(c => c.GetAllProviders()).ReturnsAsync(new List<Provider> {new Provider { Ukprn = nationalAchievementRatesImported .FirstOrDefault().Ukprn} });

            await sut.ReloadNationalAcheivementRates(nationalAchievementRatesImported);

            nationalAchievementRatesImportWriteRepositoryMock.Verify(r => r.Reload(It.IsAny<List<NationalAchievementRateImport>>()));
            nationalAchievementRatesImportReadRepositoryMock.Verify(r => r.GetAllWithAchievementData());
            nationalAchievementRatesWriteRepositoryMock.Verify(r => r.Reload(It.IsAny<List<NationalAchievementRate>>()));
            auditRepositoryMock.Verify(x=>x.Insert(It.IsAny<ImportAudit>()));
        }

        [Test]
        [MoqAutoData]
        public async Task ReloadNationalAcheivementRates_ExceptionReturned_LoggedInCatchBlock(
            [Frozen] Mock<INationalAchievementRatesImportWriteRepository> nationalAchievementRatesImportWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesImportReadRepository> nationalAchievementRatesImportReadRepositoryMock,
            [Frozen] Mock<IProvidersReadRepository> providersReadRepositoryMock,
            [Frozen] Mock<ILogger<ReloadNationalAcheivementRatesService>> loggerMock,
            [Greedy] ReloadNationalAcheivementRatesService sut,
            List<NationalAchievementRatesApiModel> nationalAchievementRatesImported)
        {
            nationalAchievementRatesImportWriteRepositoryMock.Setup(c => c.Reload(It.IsAny<List<NationalAchievementRateImport>>())).ThrowsAsync(new Exception());

            nationalAchievementRatesImportReadRepositoryMock.Setup(c => c.GetAllWithAchievementData()).ThrowsAsync( new Exception());

            providersReadRepositoryMock.Setup(c => c.GetAllProviders()).ReturnsAsync(new List<Provider> { new Provider { Ukprn = nationalAchievementRatesImported.FirstOrDefault().Ukprn } });

            await sut.ReloadNationalAcheivementRates(nationalAchievementRatesImported);
            
            loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }
    }
}
