using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.OverallNationalAchievementRates.Queries.GetOverallAchievementRates;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Application.UnitTests.OverallNationalAchievementRates.Queries
{
    [TestFixture]
    public class NationalAchievementRatesOverallModelTests
    {
        [Test, AutoData]
        public void Operator_TransformsToModel(NationalAchievementRateOverall source)
        {
            NationalAchievementRateOverallModel model = source;

            model.Should().BeEquivalentTo(source, s => s
                .Excluding(s => s.ApprenticeshipLevel)
                .Excluding(s => s.Id)
            );

            model.Level.Should().Be(source.ApprenticeshipLevel);
        }
    }
}

