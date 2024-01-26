using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [TestFixture]
    public class NationalAchievementRateTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ConstructsObject(NationalAchievementRateImport source)
        {
            var destination = (NationalAchievementRate)source;

            destination.Id.Should().Be(0);
            destination.ProviderId.Should().Be(0);
            destination.Age.Should().Be(source.Age);
            destination.ApprenticeshipLevel.Should().Be(source.ApprenticeshipLevel);
            destination.OverallCohort.Should().Be(source.OverallCohort);
            destination.OverallAchievementRate.Should().Be(source.OverallAchievementRate);
            destination.Provider.Should().BeNull();
            destination.SectorSubjectAreaTier1.Should().Be(source.SectorSubjectAreaTier1);
        }
    }
}
