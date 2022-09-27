using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Extensions;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [TestFixture]
    public class NationalAchievementRateOverall_ImportTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ConstructsObject(NationalAchievementRatesOverallApiImport source)
        {
            source.Age = "SixteenToEighteen";
            source.ApprenticeshipLevel = "Two";
            var destination = (NationalAchievementRateOverall_Import)source;
            destination.Age.Should().Be(source.Age.ToAge());
            destination.SectorSubjectArea.Should().Be(source.SectorSubjectArea);
            destination.ApprenticeshipLevel.Should().Be(source.ApprenticeshipLevel.ToApprenticeshipLevel());
            destination.OverallCohort.Should().Be(source.OverallCohort);
            destination.OverallAchievementRate.Should().Be(source.OverallAchievementRate);
        }
    }
}
