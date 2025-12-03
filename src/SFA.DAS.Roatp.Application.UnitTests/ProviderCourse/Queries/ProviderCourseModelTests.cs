using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.ExternalRead.GetProviderCourse;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse.Queries
{
    [TestFixture]
    public class ProviderCourseModelTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(Domain.Entities.ProviderCourse course, int larsCode)
        {
            course.LarsCode = larsCode.ToString();
            var model = (ProviderCourseModelExternal)course;

            model.Should().BeEquivalentTo(course, c => c
                .Excluding(s => s.Id)
                .Excluding(s => s.ProviderId)
                .Excluding(s => s.Locations)
                .Excluding(s => s.Provider)
                .Excluding(s => s.Versions)
                .Excluding(s => s.Standard)
                .Excluding(s => s.LarsCode)
            );

            model.LarsCode.ToString().Should().Be(course.LarsCode);
        }
    }
}
