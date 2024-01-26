using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Extensions;
using SFA.DAS.Roatp.Jobs.Models;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Extensions;

public class OverallAchievementRateCsvModelExtensionsTests
{
    NationalAchievementRateOverallImport _actual;
    int _overallCohort;
    decimal _achievementRate;
    SectorSubjectAreaTier1Model _sectorSubjectAreaTier1Model;

    [SetUp]
    public void SetUp()
    {
        Fixture fixture = new();
        var ssa1s = fixture.CreateMany<SectorSubjectAreaTier1Model>().ToList();
        _overallCohort = fixture.Create<int>();
        _achievementRate = fixture.Create<decimal>();
        _sectorSubjectAreaTier1Model = ssa1s.First();

        OverallAchievementRateCsvModel source = new();
        source.AgeGroup = source.ApprenticeshipLevel = "total";
        source.OverallCohort = _overallCohort.ToString();
        source.OverallAchievementRate = _achievementRate.ToString();
        source.SectorSubjectAreaTier1Desc = _sectorSubjectAreaTier1Model.SectorSubjectAreaTier1Desc;

        _actual = source.ConvertToEntity(ssa1s);
    }

    [Test]
    public void ThenOverallCohortIsParsedToInt() => _actual.OverallCohort.Should().Be(_overallCohort);

    [Test]
    public void ThenOverallAchievementRateIsParsedToDecimal() => _actual.OverallAchievementRate.Should().Be(_achievementRate);

    [Test]
    public void ThenExtractsSsa1Code() => _actual.SectorSubjectAreaTier1.Should().Be(_sectorSubjectAreaTier1Model.SectorSubjectAreaTier1);

    [Test]
    public void ThenSetsRespectiveAgeGroup() => _actual.Age.Should().Be(Age.AllAges);

    [Test]
    public void ThenSetsRespectiveApprenticeshipLevel() => _actual.ApprenticeshipLevel.Should().Be(ApprenticeshipLevel.AllLevels);
}
