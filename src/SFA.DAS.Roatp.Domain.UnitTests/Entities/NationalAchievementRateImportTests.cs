﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [TestFixture]
    public class NationalAchievementRateImportTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ConstructsObject(NationalAchievementRatesApiImport source)
        {
            source.Age = Age.SixteenToEighteen;
            source.ApprenticeshipLevel = ApprenticeshipLevel.Two;

            var destination = (NationalAchievementRateImport)source;

            destination.Id.Should().Be(0);
            destination.Ukprn.Should().Be(source.Ukprn);
            destination.Age.Should().Be((int)source.Age);
            destination.SectorSubjectArea.Should().Be(source.SectorSubjectArea);
            destination.ApprenticeshipLevel.Should().Be((int)source.ApprenticeshipLevel);
            destination.OverallCohort.Should().Be(source.OverallCohort);
            destination.OverallAchievementRate.Should().Be(source.OverallAchievementRate);
        }
    }
}
