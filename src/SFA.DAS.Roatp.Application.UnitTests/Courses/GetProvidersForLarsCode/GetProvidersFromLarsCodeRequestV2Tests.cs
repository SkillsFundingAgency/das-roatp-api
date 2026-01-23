using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForLarsCode;

[TestFixture]
public class GetProvidersFromLarsCodeRequestV2Tests
{
    [Test, MoqAutoData]
    public void ImplicitConversion_FromV1_ToV2_MapsAllFieldsAndLists(
        GetProvidersFromLarsCodeRequest v1)
    {
        // Act
        GetProvidersFromLarsCodeRequestV2 v2 = v1;

        // Assert
        v2.Should().BeEquivalentTo(v1, options => options
        .ExcludingMissingMembers());
    }

    [Test]
    [MoqInlineAutoData(null)]
    public void ImplicitConversion_FromV1_Null_ReturnsNull(GetProvidersFromLarsCodeRequest v1)
    {
        GetProvidersFromLarsCodeRequestV2 v2 = v1;
        v2.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void ImplicitConversion_FromV1_WithNullLists_ProducesNullListsInV2(GetProvidersFromLarsCodeRequest v1)
    {
        // Arrange
        v1.OrderBy = ProviderOrderBy.AchievementRate;
        v1.DeliveryModes = null;
        v1.EmployerProviderRatings = null;
        v1.ApprenticeProviderRatings = null;
        v1.Qar = null;
        v1.Page = null;
        v1.PageSize = null;
        v1.UserId = null;

        // Act
        GetProvidersFromLarsCodeRequestV2 v2 = v1;

        // Assert
        using (new AssertionScope())
        {
            v2.Should().NotBeNull();
            v2.DeliveryModes.Should().BeNull();
            v2.EmployerProviderRatings.Should().BeNull();
            v2.ApprenticeProviderRatings.Should().BeNull();
            v2.Qar.Should().BeNull();
            v2.Page.Should().BeNull();
            v2.PageSize.Should().BeNull();
            v2.UserId.Should().BeNull();
            v2.OrderBy.Should().Be(v1.OrderBy);
        }
    }
}
