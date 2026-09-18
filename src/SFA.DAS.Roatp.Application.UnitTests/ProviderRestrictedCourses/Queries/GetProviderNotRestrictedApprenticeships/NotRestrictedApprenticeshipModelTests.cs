using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderRestrictedCourses.Queries.GetProviderNotRestrictedApprenticeships;

public class NotRestrictedApprenticeshipModelTests
{
    [Test, RecursiveMoqAutoData]
    public void ImplicitConversion_ReturnsExpectedModel(Standard standard)
    {
        // Act
        NotRestrictedApprenticeshipModel sut = standard;

        // Assert
        sut.LarsCode.Should().Be(standard.LarsCode);
        sut.Title.Should().Be(standard.Title);
        sut.Level.Should().Be(standard.Level);
    }
}
