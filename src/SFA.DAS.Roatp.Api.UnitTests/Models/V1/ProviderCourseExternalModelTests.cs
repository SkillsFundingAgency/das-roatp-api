using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models.V1;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Models.V1;

[TestFixture]
public class ProviderCourseExternalModelTests
{
    [Test, MoqAutoData]
    public void ImplicitConversion_WhenSourceIsNull_ReturnsNull()
    {
        ProviderCourseModelExternal source = null;

        ProviderCourseExternalModel result = source;

        result.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void ImplicitConversion_ParsesLarsCode_WhenValidString(ProviderCourseModelExternal source)
    {
        var cases = new (string input, int expected)[]
        {
            ("123", 123),
            ("001", 1),
            ("0", 0)
        };

        foreach (var (inputLarsCode, expected) in cases)
        {
            source.LarsCode = inputLarsCode;

            ProviderCourseExternalModel result = source;

            result.Should().BeEquivalentTo(source, options => options
                .Excluding(c => c.LarsCode)
                .Excluding(c => c.HasNationalDeliveryOption)
                .Excluding(c => c.HasHundredPercentEmployerDeliveryOption));
            result.LarsCode.Should().Be(expected);
        }
    }
}