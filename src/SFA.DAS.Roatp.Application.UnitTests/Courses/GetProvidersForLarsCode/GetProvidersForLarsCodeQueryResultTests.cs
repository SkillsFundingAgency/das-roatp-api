using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForLarsCode;

[TestFixture]
public class GetProvidersForLarsCodeQueryResultTests
{
    [Test, MoqAutoData]
    public void ImplicitConversion_FromV2ToV1_MapsProvidersAndTopLevelFields(GetProvidersForLarsCodeQueryResultV2 v2)
    {
        GetProvidersForLarsCodeQueryResult v1 = v2;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Page.Should().Be(v2.Page);
            v1.PageSize.Should().Be(v2.PageSize);
            v1.TotalPages.Should().Be(v2.TotalPages);
            v1.TotalCount.Should().Be(v2.TotalCount);
            v1.LarsCode.Should().Be(int.TryParse(v2.LarsCode, out var larsCode) ? larsCode : 0);
            v1.StandardName.Should().Be(v2.StandardName);
            v1.QarPeriod.Should().Be(v2.QarPeriod);
            v1.ReviewPeriod.Should().Be(v2.ReviewPeriod);

            v1.Providers.Should().NotBeNull().And.HaveCount(v2.Providers.Count);
            v1.Providers.Should().BeEquivalentTo(v2.Providers, options => options
                .ExcludingMissingMembers());
        }
    }

    [Test]
    public void ImplicitConversion_FromV2_Null_ReturnsNull()
    {
        GetProvidersForLarsCodeQueryResultV2 v2 = null;

        GetProvidersForLarsCodeQueryResult v1 = v2;

        v1.Should().BeNull();
    }

    [Test]
    public void ImplicitConversion_FromV2_WithEmptyProviders_MapsToEmptyList()
    {
        const string LarsCode = "123";
        var v2 = new GetProvidersForLarsCodeQueryResultV2
        {
            Page = 1,
            PageSize = 10,
            TotalPages = 1,
            TotalCount = 0,
            LarsCode = LarsCode,
            StandardName = "Std",
            QarPeriod = "2022/23",
            ReviewPeriod = "2023/24",
            Providers = new List<ProviderDataV2>()
        };

        GetProvidersForLarsCodeQueryResult v1 = v2;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Providers.Should().NotBeNull().And.BeEmpty();
            v1.LarsCode.Should().Be(int.TryParse(LarsCode, out var larsCode) ? larsCode : 0);
        }
    }

    [Test]
    public void ImplicitConversion_FromV2_MapsProviderItemFieldsCorrectly()
    {
        var shortlistId = Guid.NewGuid();
        var v2 = new GetProvidersForLarsCodeQueryResultV2
        {
            Page = 2,
            PageSize = 5,
            TotalPages = 3,
            TotalCount = 7,
            LarsCode = "999",
            StandardName = "My Standard",
            QarPeriod = "2021/22",
            ReviewPeriod = "2022/23",
            Providers = new List<ProviderDataV2>
            {
                new ProviderDataV2
                {
                    Ordering = 42,
                    Ukprn = 12345678,
                    ProviderName = "Provider X",
                    HasOnlineDeliveryOption = true, // should be ignored by V1
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
            }
        };

        GetProvidersForLarsCodeQueryResult v1 = v2;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Page.Should().Be(v2.Page);
            v1.PageSize.Should().Be(v2.PageSize);
            v1.TotalPages.Should().Be(v2.TotalPages);
            v1.TotalCount.Should().Be(v2.TotalCount);
            v1.LarsCode.Should().Be(int.TryParse(v2.LarsCode, out var larsCode) ? larsCode : 0);
            v1.StandardName.Should().Be(v2.StandardName);
            v1.QarPeriod.Should().Be(v2.QarPeriod);
            v1.ReviewPeriod.Should().Be(v2.ReviewPeriod);

            var v1provider = v1.Providers.Single();
            var v2provider = v2.Providers.Single();
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


    [Test]
    public void ImplicitConversion_FromV2_WithNullProviders_LeavesProvidersNull()
    {
        var v2 = new GetProvidersForLarsCodeQueryResultV2
        {
            Page = 1,
            PageSize = 20,
            TotalPages = 1,
            TotalCount = 0,
            LarsCode = "321",
            StandardName = "Std",
            QarPeriod = "2020/21",
            ReviewPeriod = "2021/22",
            Providers = null
        };

        GetProvidersForLarsCodeQueryResult v1 = v2;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Providers.Should().BeNull("null input Providers should remain null as the converter uses the null-conditional operator without defaulting to an empty list");
        }
    }

    [Test]
    public void ImplicitConversion_FromV2_WithNonNumericLarsCode_SetsLarsCodeToZero()
    {
        var v2 = new GetProvidersForLarsCodeQueryResultV2
        {
            Page = 1,
            PageSize = 10,
            TotalPages = 1,
            TotalCount = 1,
            LarsCode = "ABC123", // non-numeric
            StandardName = "Standard",
            QarPeriod = "2022/23",
            ReviewPeriod = "2023/24",
            Providers = new List<ProviderDataV2>()
        };

        GetProvidersForLarsCodeQueryResult v1 = v2;

        v1.LarsCode.Should().Be(0);
    }

    [Test]
    public void ImplicitConversion_FromV2_WithProviderNullFields_PreservesNulls()
    {
        var v2 = new GetProvidersForLarsCodeQueryResultV2
        {
            Page = 1,
            PageSize = 10,
            TotalPages = 1,
            TotalCount = 1,
            LarsCode = "100",
            StandardName = null,
            QarPeriod = null,
            ReviewPeriod = null,
            Providers = new List<ProviderDataV2>
            {
                new ProviderDataV2
                {
                    Ordering = 0,
                    Ukprn = 10000001,
                    ProviderName = null,
                    ShortlistId = null,
                    Locations = null,
                    Leavers = null,
                    AchievementRate = null,
                    EmployerReviews = null,
                    EmployerStars = null,
                    EmployerRating = ProviderRating.Excellent,
                    ApprenticeReviews = null,
                    ApprenticeStars = null,
                    ApprenticeRating = ProviderRating.Good
                }
            }
        };

        GetProvidersForLarsCodeQueryResult v1 = v2;

        using (new AssertionScope())
        {
            v1.StandardName.Should().BeNull();
            v1.QarPeriod.Should().BeNull();
            v1.ReviewPeriod.Should().BeNull();

            v1.Providers.Should().NotBeNull().And.HaveCount(v1.Providers.Count);
            var p = v1.Providers.First();
            p.ProviderName.Should().BeNull();
            p.ShortlistId.Should().BeNull();
            p.Locations.Should().BeNull();
            p.Leavers.Should().BeNull();
            p.AchievementRate.Should().BeNull();
            p.EmployerReviews.Should().BeNull();
            p.EmployerStars.Should().BeNull();
            p.EmployerRating.Should().Be(ProviderRating.Excellent);
            p.ApprenticeReviews.Should().BeNull();
            p.ApprenticeStars.Should().BeNull();
            p.ApprenticeRating.Should().Be(ProviderRating.Good);
        }
    }
}
