using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries;

[TestFixture]
public class ProviderCourseModelTests
{
    [Test, RecursiveMoqAutoData]
    public void Operator_PopulatesModelFromEntity(Domain.Entities.ProviderCourse course, string larsCode)
    {
        course.LarsCode = larsCode;
        ProviderCourseModel model = course;

        model.Should().BeEquivalentTo(course, c => c.ExcludingMissingMembers());

        model.LarsCode.Should().Be(course.LarsCode);
    }
}