using FluentAssertions;
using FluentAssertions.Execution;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V1;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProvidersFromLarsCode.V2;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForLarsCode;


[TestFixture]
public class GetProvidersForLarsCodeQueryResultTests
{
    [Test, MoqAutoData]
    public void ImplicitConversion_FromV2_ToV1_MapsProvidersAndTopLevelFields(GetProvidersForLarsCodeQueryResultV2 v2)
    {
        GetProvidersForLarsCodeQueryResult v1 = v2;

        using (new AssertionScope())
        {
            v1.Should().NotBeNull();
            v1.Page.Should().Be(v2.Page);
            v1.PageSize.Should().Be(v2.PageSize);
            v1.TotalPages.Should().Be(v2.TotalPages);
            v1.TotalCount.Should().Be(v2.TotalCount);
            v1.LarsCode.Should().Be(v2.LarsCode);
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

}
