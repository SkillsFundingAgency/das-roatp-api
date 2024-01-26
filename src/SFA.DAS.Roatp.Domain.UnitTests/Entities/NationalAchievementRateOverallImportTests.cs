using System;
using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [Obsolete]
    [TestFixture]
    public class NationalAchievementRateOverallImportTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ConstructsObject(NationalAchievementRatesOverallApiModel source)
        {
            source.Age = Age.SixteenToEighteen;
            source.ApprenticeshipLevel = ApprenticeshipLevel.Two;

            var destination = (NationalAchievementRateOverallImport)source;

            destination.Id.Should().Be(0);
            destination.Age.Should().Be(source.Age);
            destination.ApprenticeshipLevel.Should().Be(source.ApprenticeshipLevel);
            destination.OverallCohort.Should().Be(source.OverallCohort);
            destination.OverallAchievementRate.Should().Be(source.OverallAchievementRate);
        }
    }
}
