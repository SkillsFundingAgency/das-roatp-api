using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Standards.Queries.GetAllStandards;

[TestFixture]
public class GetAllStandardsQueryResultTests
{
    [Test]
    public void Constructor_WithNullList_ProducesEmptyStandardsList()
    {
        var sut = new GetAllStandardsQueryResult(null);

        sut.Standards.Should().NotBeNull();
        sut.Standards.Should().BeEmpty();
    }

    [Test, RecursiveMoqAutoData]
    public void Constructor_MapsAllStandardProperties_ToStandardModel(Standard source)
    {
        var sut = new GetAllStandardsQueryResult([source]);

        sut.Standards.Should().HaveCount(1);
        var model = sut.Standards[0];

        model.Should().BeEquivalentTo(source, c => c
            .Excluding(s => s.Version)
            .Excluding(s => s.SectorSubjectAreaTier1)
            .Excluding(s => s.ProviderCourses)
            .Excluding(s => s.Duration)
            .Excluding(s => s.DurationUnits)
            .Excluding(s => s.ProviderCoursesTimelines)
        );
    }
}