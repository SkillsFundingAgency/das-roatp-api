using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.Extensions;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Extensions.NationalAchievementRatesExtensionsTests;

public class ToApprenticeshipLevelEnumTests
{
    [TestCase(null, ApprenticeshipLevel.Unknown)]
    [TestCase("", ApprenticeshipLevel.Unknown)]
    [TestCase(" ", ApprenticeshipLevel.Unknown)]
    [TestCase("rubbish", ApprenticeshipLevel.Unknown)]
    [TestCase("advanced level", ApprenticeshipLevel.Three)]
    [TestCase("Advanced Level", ApprenticeshipLevel.Three)]
    [TestCase(" advanced level ", ApprenticeshipLevel.Three)]
    [TestCase("higher level", ApprenticeshipLevel.FourPlus)]
    [TestCase("intermediate level", ApprenticeshipLevel.Two)]
    [TestCase("total", ApprenticeshipLevel.AllLevels)]
    public void ConvertsStringToApprenticeshipLevelEnum(string value, ApprenticeshipLevel expected)
    {
        var actual = value.ToApprenticeshipLevelEnum();

        actual.Should().Be(expected);
    }
}
