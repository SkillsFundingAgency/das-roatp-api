using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Api.UnitTests.Models;

[TestFixture]
public class GetProvidersForLarsCodeResultV1ModelTests
{
    [Test, MoqAutoData]
    public void ImplicitConversion_FromV2ToV1_MapsProvidersAndTopLevelFields(GetProvidersForLarsCodeQueryResult source)
    {
        GetProvidersForLarsCodeResultModel v1 = source;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Page.Should().Be(source.Page);
            v1.PageSize.Should().Be(source.PageSize);
            v1.TotalPages.Should().Be(source.TotalPages);
            v1.TotalCount.Should().Be(source.TotalCount);
            v1.LarsCode.Should().Be(int.TryParse(source.LarsCode, out var larsCode) ? larsCode : 0);
            v1.StandardName.Should().Be(source.StandardName);
            v1.QarPeriod.Should().Be(source.QarPeriod);
            v1.ReviewPeriod.Should().Be(source.ReviewPeriod);

            v1.Providers.Should().NotBeNull().And.HaveCount(source.Providers.Count);
            v1.Providers.Should().BeEquivalentTo(source.Providers, options => options
                .Excluding(p => p.HasOnlineDeliveryOption));
        }
    }

    [Test]
    public void ImplicitConversion_FromV2_Null_ReturnsNull()
    {
        GetProvidersForLarsCodeQueryResult v2 = null;

        GetProvidersForLarsCodeResultModel v1 = v2;

        v1.Should().BeNull();
    }

    [Test, MoqAutoData]
    public void ImplicitConversion_FromV2_WithEmptyProviders_MapsToEmptyList(GetProvidersForLarsCodeQueryResult source)
    {
        const string LarsCode = "123";
        source.LarsCode = LarsCode;
        source.Providers = new List<Domain.Models.ProviderData>();

        GetProvidersForLarsCodeResultModel v1 = source;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Providers.Should().NotBeNull().And.BeEmpty();
            v1.LarsCode.Should().Be(int.TryParse(LarsCode, out var larsCode) ? larsCode : 0);
        }
    }

    [Test, MoqAutoData]
    public void ImplicitConversion_FromV2_MapsProviderItemFieldsCorrectly(GetProvidersForLarsCodeQueryResult source)
    {
        var shortlistId = Guid.NewGuid();
        source.Providers = new List<Domain.Models.ProviderData>
            {
                new Domain.Models.ProviderData
                {
                    Ordering = 42,
                    Ukprn = 12345678,
                    ProviderName = "Provider X",
                    HasOnlineDeliveryOption = true,
                    ShortlistId = shortlistId,
                    Locations = null,
                    Leavers = "10",
                    AchievementRate = "80%",
                    EmployerReviews = "5",
                    EmployerStars = "4.0",
                    EmployerRating = ProviderRating.Good,
                    ApprenticeReviews = "8",
                    ApprenticeStars = "3.9",
                    ApprenticeRating = ProviderRating.Excellent
                }
            };

        GetProvidersForLarsCodeResultModel v1 = source;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Page.Should().Be(source.Page);
            v1.PageSize.Should().Be(source.PageSize);
            v1.TotalPages.Should().Be(source.TotalPages);
            v1.TotalCount.Should().Be(source.TotalCount);
            v1.LarsCode.Should().Be(int.TryParse(source.LarsCode, out var larsCode) ? larsCode : 0);
            v1.StandardName.Should().Be(source.StandardName);
            v1.QarPeriod.Should().Be(source.QarPeriod);
            v1.ReviewPeriod.Should().Be(source.ReviewPeriod);

            var v1provider = v1.Providers.Single();
            var v2provider = source.Providers.Single();
            v1provider.Ordering.Should().Be(v2provider.Ordering);
            v1provider.Ukprn.Should().Be(v2provider.Ukprn);
            v1provider.ProviderName.Should().Be(v2provider.ProviderName);
            v1provider.ShortlistId.Should().Be(v2provider.ShortlistId);
            v1provider.Locations.Should().BeEquivalentTo(v2provider.Locations);
            v1provider.Leavers.Should().Be(v2provider.Leavers);
            v1provider.AchievementRate.Should().Be(v2provider.AchievementRate);
            v1provider.EmployerReviews.Should().Be(v2provider.EmployerReviews);
            v1provider.EmployerStars.Should().Be(v2provider.EmployerStars);
            v1provider.EmployerRating.Should().Be(v2provider.EmployerRating);
            v1provider.ApprenticeReviews.Should().Be(v2provider.ApprenticeReviews);
            v1provider.ApprenticeStars.Should().Be(v2provider.ApprenticeStars);
            v1provider.ApprenticeRating.Should().Be(ProviderRating.Excellent);
        }
    }


    [Test, MoqAutoData]
    public void ImplicitConversion_FromV2_WithNullProviders_LeavesProvidersNull(GetProvidersForLarsCodeQueryResult source)
    {
        source.Providers = new List<Domain.Models.ProviderData>();

        GetProvidersForLarsCodeResultModel v1 = source;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Providers.Should().NotBeNull().And.BeEmpty();
        }
    }

    [Test]
    public void ImplicitConversion_FromV2_WithNonNumericLarsCode_SetsLarsCodeToZero()
    {
        var v2 = new GetProvidersForLarsCodeQueryResult
        {
            Page = 1,
            PageSize = 10,
            TotalPages = 1,
            TotalCount = 1,
            LarsCode = "ABC123",
            StandardName = "Standard",
            QarPeriod = "2022/23",
            ReviewPeriod = "2023/24",
            Providers = new List<Domain.Models.ProviderData>()
        };

        GetProvidersForLarsCodeResultModel v1 = v2;

        v1.LarsCode.Should().Be(0);
    }
}
