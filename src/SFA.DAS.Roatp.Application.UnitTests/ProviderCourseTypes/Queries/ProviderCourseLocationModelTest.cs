using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseTypes.Queries.GetProviderCourseTypes;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseTypes.Queries
{
    [TestFixture]
    public class ProviderCourseTypeModelTest
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(ProviderCourseType courseType)
        {
            var model = (ProviderCourseTypeModel)courseType;
            model.CourseType.Should().Be(courseType.CourseType);
        }
    }
}