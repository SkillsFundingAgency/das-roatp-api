using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Extensions;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Domain.UnitTests.Extensions
{
    [TestFixture]
    public class EnumExtensionsTests
    {
        [TestCase("SixteenToEighteen", Age.SixteenToEighteen)]
        [TestCase("NineteenToTwentyThree", Age.NineteenToTwentyThree)]
        [TestCase("TwentyFourPlus", Age.TwentyFourPlus)]
        [TestCase("AllAges", Age.AllAges)]
        [TestCase("15", Age.Unknown)]
        public void ToAge_ReturnsAgeEnum(string value, Age age)
        {
            var result = EnumExtensions.ToAge(value);

            result.Should().Be((int)age);
        }

        [TestCase("Two", ApprenticeshipLevel.Two)]
        [TestCase("Three", ApprenticeshipLevel.Three)]
        [TestCase("FourPlus", ApprenticeshipLevel.FourPlus)]
        [TestCase("AllLevels", ApprenticeshipLevel.AllLevels)]
        [TestCase("1", ApprenticeshipLevel.Unknown)]
        public void ToApprenticeshipLevel_ReturnsApprenticeshipLevelEnum(string value, ApprenticeshipLevel apprenticeshipLevel)
        {
            var result = EnumExtensions.ToApprenticeshipLevel(value);

            result.Should().Be((int)apprenticeshipLevel);
        }
    }
}
