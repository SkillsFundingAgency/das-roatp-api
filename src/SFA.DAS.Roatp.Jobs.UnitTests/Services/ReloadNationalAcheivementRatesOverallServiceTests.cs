using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Testing.AutoFixture;
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
    }
}
