using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderAllowedCourses.Queries.GetProviderAllowedCourses;

public class ProviderAllowedCourseModelTests
{
    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_ReturnsExpectedModel(ProviderAllowedCourse providerAllowedCourse)
    {
        // Act
        ProviderAllowedCourseModel sut = providerAllowedCourse;
        // Assert
        sut.LarsCode.Should().Be(providerAllowedCourse.LarsCode);
        sut.Title.Should().Be(providerAllowedCourse.Standard.Title);
        sut.Level.Should().Be(providerAllowedCourse.Standard.Level);
    }
}
