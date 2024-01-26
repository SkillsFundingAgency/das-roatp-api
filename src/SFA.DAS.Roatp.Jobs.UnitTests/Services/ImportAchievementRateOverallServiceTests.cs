using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.NUnit3;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Models;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Roatp.Jobs.UnitTests.Extensions;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services;

public class ImportAchievementRateOverallServiceTests
{
    [Test, AutoData]
    public async Task ImportData_InvokesRespectiveRepository(string ageGroup)
    {
        Fixture fixture = new();
        var ssa1s = fixture.CreateMany<SectorSubjectAreaTier1Model>().ToList();
        var rawData = fixture
            .Build<OverallAchievementRateCsvModel>()
            .With(o => o.AgeGroup, "total")
            .With(o => o.ApprenticeshipLevel, "total")
            .With(o => o.OverallCohort, fixture.Create<int>().ToString())
            .With(o => o.OverallAchievementRate, fixture.Create<decimal>().ToString())
            .WithValues(o => o.SectorSubjectAreaTier1Desc, ssa1s.Select(s => s.SectorSubjectAreaTier1Desc).ToArray())
            .CreateMany();

        Mock<INationalAchievementRatesOverallImportWriteRepository> importRepo = new();
        Mock<INationalAchievementRatesOverallWriteRepository> liveRepo = new();
        Mock<IImportAuditWriteRepository> auditRepo = new();

        ImportNationalAchievementRateOverallService sut = new(importRepo.Object, liveRepo.Object, auditRepo.Object);

        // Act
        await sut.ImportData(rawData, ssa1s);

        using (new AssertionScope())
        {
            importRepo.Verify(i => i.Reload(It.IsAny<List<NationalAchievementRateOverallImport>>()), Times.Once);
            liveRepo.Verify(i => i.Reload(It.IsAny<List<NationalAchievementRateOverall>>()), Times.Once);
            auditRepo.Verify(a => a.Insert(It.Is<ImportAudit>(i => i.ImportType == ImportType.NationalAchievementRatesOverall)), Times.Once);
        }
    }
}

public class ImportNationalAchievementRateServiceTests
{
    [Test, AutoData]
    public async Task ImportData_InvokesRespectiveRepository(string ageGroup)
    {
        Fixture fixture = new();
        var ssa1s = fixture.CreateMany<SectorSubjectAreaTier1Model>().ToList();
        var rawData = fixture
            .Build<ProviderAchievementRateCsvModel>()
            .With(o => o.AgeGroup, "total")
            .With(o => o.ApprenticeshipLevel, "total")
            .With(o => o.OverallCohort, fixture.Create<int>().ToString())
            .With(o => o.OverallAchievementRate, fixture.Create<decimal>().ToString())
            .WithValues(o => o.SectorSubjectAreaTier1Desc, ssa1s.Select(s => s.SectorSubjectAreaTier1Desc).ToArray())
            .CreateMany();

        Mock<INationalAchievementRatesImportWriteRepository> importRepo = new();
        Mock<INationalAchievementRatesWriteRepository> liveRepo = new();
        Mock<IImportAuditWriteRepository> auditRepo = new();

        ImportNationalAchievementRateService sut = new(importRepo.Object, liveRepo.Object, auditRepo.Object);

        // Act
        await sut.ImportData(rawData, ssa1s);

        using (new AssertionScope())
        {
            importRepo.Verify(i => i.Reload(It.IsAny<List<NationalAchievementRateImport>>()), Times.Once);
            liveRepo.Verify(i => i.Reload(It.IsAny<List<NationalAchievementRate>>()), Times.Once);
            auditRepo.Verify(a => a.Insert(It.Is<ImportAudit>(i => i.ImportType == ImportType.NationalAchievementRates)), Times.Once);
        }
    }
}
