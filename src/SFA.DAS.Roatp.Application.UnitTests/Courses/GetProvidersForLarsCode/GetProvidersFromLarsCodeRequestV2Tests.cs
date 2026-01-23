using System.Collections.Generic;
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

    [Test]
    public void ImplicitConversion_MapsDeliveryModes_WithNulls_AndPreservesOrder()
    {
        var v1 = new GetProvidersFromLarsCodeRequest
        {
            DeliveryModes = new List<DeliveryModeV1?> { DeliveryModeV1.Provider, DeliveryModeV1.Workplace, null }
        };

        // Act
        GetProvidersFromLarsCodeRequestV2 v2 = v1;

        // Assert
        using (new AssertionScope())
        {
            v2.DeliveryModes.Should().NotBeNull().And.HaveCount(3);
            v2.DeliveryModes[0].Should().Be((DeliveryModeV2?)DeliveryModeV2.Provider);
            v2.DeliveryModes[1].Should().Be((DeliveryModeV2?)DeliveryModeV2.Workplace);
            v2.DeliveryModes[2].Should().BeNull();
        }
    }

    [Test]
    public void ImplicitConversion_EmptyLists_MapToEmptyLists_NotNull()
    {
        var v1 = new GetProvidersFromLarsCodeRequest
        {
            DeliveryModes = new List<DeliveryModeV1?>(),
            EmployerProviderRatings = new List<ProviderRating?>(),
            ApprenticeProviderRatings = new List<ProviderRating?>(),
            Qar = new List<QarRating?>()
        };

        // Act
        GetProvidersFromLarsCodeRequestV2 v2 = v1;

        // Assert
        using (new AssertionScope())
        {
            v2.DeliveryModes.Should().NotBeNull().And.BeEmpty();
            v2.EmployerProviderRatings.Should().NotBeNull().And.BeEmpty();
            v2.ApprenticeProviderRatings.Should().NotBeNull().And.BeEmpty();
            v2.Qar.Should().NotBeNull().And.BeEmpty();
        }
    }

    [Test]
    public void ImplicitConversion_CreatesIndependentLists_ModifyingSourceAfterConversionDoesNotAffectTarget()
    {
        var v1 = new GetProvidersFromLarsCodeRequest
        {
            DeliveryModes = new List<DeliveryModeV1?> { DeliveryModeV1.DayRelease },
            EmployerProviderRatings = new List<ProviderRating?> { ProviderRating.Good },
            ApprenticeProviderRatings = new List<ProviderRating?> { ProviderRating.NotYetReviewed },
            Qar = new List<QarRating?> { QarRating.Good }
        };

        // Act
        GetProvidersFromLarsCodeRequestV2 v2 = v1;

        // Mutate source lists after conversion
        v1.DeliveryModes.Add(DeliveryModeV1.Workplace);
        v1.EmployerProviderRatings.Add(ProviderRating.Excellent);
        v1.ApprenticeProviderRatings.Add(ProviderRating.Poor);
        v1.Qar.Add(QarRating.Excellent);

        // Assert - v2 lists are unchanged (operator created new lists)
        using (new AssertionScope())
        {
            v2.DeliveryModes.Should().HaveCount(1);
            v2.EmployerProviderRatings.Should().HaveCount(1);
            v2.ApprenticeProviderRatings.Should().HaveCount(1);
            v2.Qar.Should().HaveCount(1);

            v2.DeliveryModes[0].Should().Be((DeliveryModeV2?)DeliveryModeV2.DayRelease);
            v2.EmployerProviderRatings[0].Should().Be(ProviderRating.Good);
            v2.ApprenticeProviderRatings[0].Should().Be(ProviderRating.NotYetReviewed);
            v2.Qar[0].Should().Be(QarRating.Good);
        }
    }
}
