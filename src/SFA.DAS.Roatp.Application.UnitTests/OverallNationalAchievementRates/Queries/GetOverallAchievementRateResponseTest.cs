using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Queries.ProviderLocations
{
    [TestFixture]
    public class GetOverallAchievementRateResponseTest
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(NationalAchievementRateOverall source)
        {
            var destination = (GetOverallAchievementRateResponse)source;

            destination.Age.Should().Be(source.Age.ToString());
            destination.SectorSubjectArea.Should().Be(source.SectorSubjectArea);
            destination.Level.Should().Be(source.ApprenticeshipLevel.ToString());
            destination.OverallCohort.Should().Be(source.OverallCohort);
            destination.OverallAchievementRate.Should().Be(source.OverallAchievementRate);
        }
    }
}