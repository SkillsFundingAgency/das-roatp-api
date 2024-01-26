using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.Extensions;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Extensions.NationalAchievementRatesExtensionsTests;

public class ToAgeEnumTests
{
    [TestCase(null, Age.Unknown)]
    [TestCase("", Age.Unknown)]
    [TestCase("  ", Age.Unknown)]
    [TestCase("rubbish", Age.Unknown)]
    [TestCase("16-18", Age.SixteenToEighteen)]
    [TestCase(" 16-18 ", Age.SixteenToEighteen)]
    [TestCase("19-23", Age.NineteenToTwentyThree)]
    [TestCase("24+", Age.TwentyFourPlus)]
    [TestCase("total", Age.AllAges)]
    [TestCase("TOTAL", Age.AllAges)]
    public void ConvertsStringToAgeEnum(string value, Age expected)
    {
        var actual = value.ToAgeEnum();

        actual.Should().Be(expected);
    }
}
