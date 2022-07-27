using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderAllCourses;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourse
{
    [TestFixture]
    public class ProviderCourseModelTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(Domain.Entities.ProviderCourse course)
        {
            var model = (ProviderCourseModel)course;

            model.Should().BeEquivalentTo(course, c => c
                .Excluding(s => s.Id)
                .Excluding(s=>s.ProviderId)
                .Excluding(s=>s.Provider)
                .Excluding(s=>s.Locations)
                .Excluding(s=>s.Versions)
            );
        }
    }
}
