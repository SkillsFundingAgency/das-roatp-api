﻿using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Domain.UnitTests.Entities
{
    [TestFixture]
    public class NationalAchievementRateOverallTests
    {
        [Test, AutoData]
        public void ImplicitOperator_ConstructsObject(NationalAchievementRateOverall_Import source)
        {
            var destination = (NationalAchievementRateOverall)source;
            destination.Should().BeEquivalentTo(source, c => c.Excluding(s => s.Id));

        }
    }
}
