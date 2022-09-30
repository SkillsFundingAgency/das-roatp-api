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
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services
{
    [TestFixture]
    public class ReloadNationalAcheivementRatesOverallServiceTests
    {
        [Test]
        [MoqAutoData]
        public async Task ReloadNationalAcheivementRatesOverall_CallsRepositoryReloadMethod(
            [Frozen] Mock<INationalAchievementRatesOverallImportWriteRepository> nationalAchievementRatesOverallImportWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesOverallImportReadRepository> nationalAchievementRatesOverallImportReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesOverallWriteRepository> nationalAchievementRatesOverallWriteRepositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Greedy] ReloadNationalAcheivementRatesOverallService sut,
            List<NationalAchievementRatesOverallApiModel> OverallAchievementRatesImported)
        {
            await sut.ReloadNationalAcheivementRatesOverall(OverallAchievementRatesImported);

            nationalAchievementRatesOverallImportWriteRepositoryMock.Verify(r => r.Reload(It.IsAny<List<NationalAchievementRateOverallImport>>()));
            nationalAchievementRatesOverallImportReadRepositoryMock.Verify(r => r.GetAllWithAchievementData());
            nationalAchievementRatesOverallWriteRepositoryMock.Verify(r => r.Reload(It.IsAny<List<NationalAchievementRateOverall>>()));
            auditRepositoryMock.Verify(x=>x.Insert(It.IsAny<ImportAudit>()));
        }

        [Test]
        [MoqAutoData]
        public async Task ReloadNationalAcheivementRatesOverall_ExceptionReturned_LoggedInCatchBlock(
            [Frozen] Mock<INationalAchievementRatesOverallImportWriteRepository> nationalAchievementRatesOverallImportWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesOverallImportReadRepository> nationalAchievementRatesOverallImportReadRepositoryMock,
            [Frozen] Mock<ILogger<ReloadNationalAcheivementRatesOverallService>> loggerMock,
            [Greedy] ReloadNationalAcheivementRatesOverallService sut,
            List<NationalAchievementRatesOverallApiModel> OverallAchievementRatesImported)
        {
            nationalAchievementRatesOverallImportWriteRepositoryMock.Setup(c => c.Reload(It.IsAny<List<NationalAchievementRateOverallImport>>())).ThrowsAsync(new Exception());

            nationalAchievementRatesOverallImportReadRepositoryMock.Setup(c => c.GetAllWithAchievementData()).ThrowsAsync(new Exception());

            await sut.ReloadNationalAcheivementRatesOverall(OverallAchievementRatesImported);

            loggerMock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.AtLeastOnce);
        }
    }
}
