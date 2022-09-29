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
    public class ReloadNationalAcheivementRatesServiceTests
    {
        [Test]
        [MoqAutoData]
        public async Task ReloadNationalAcheivementRates_CallsRepositoryReloadMethod(
            [Frozen] Mock<INationalAchievementRatesImportWriteRepository> nationalAchievementRatesImportWriteRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesImportReadRepository> nationalAchievementRatesImportReadRepositoryMock,
            [Frozen] Mock<INationalAchievementRatesWriteRepository> nationalAchievementRatesWriteRepositoryMock,
            [Frozen] Mock<IImportAuditWriteRepository> auditRepositoryMock,
            [Greedy] ReloadNationalAcheivementRatesService sut,
            List<NationalAchievementRatesApiModel> nationalAchievementRatesImported)
        {
            await sut.ReloadNationalAcheivementRates(nationalAchievementRatesImported);

            nationalAchievementRatesImportWriteRepositoryMock.Verify(r => r.Reload(It.IsAny<List<NationalAchievementRateImport>>()));
            nationalAchievementRatesImportReadRepositoryMock.Verify(r => r.GetAllWithAchievementData());
            nationalAchievementRatesWriteRepositoryMock.Verify(r => r.Reload(It.IsAny<List<NationalAchievementRate>>()));
            auditRepositoryMock.Verify(x=>x.Insert(It.IsAny<ImportAudit>()));
        }
    }
}
