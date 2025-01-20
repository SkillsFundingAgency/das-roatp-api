using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetCourses;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetCourses;

public sealed class CourseModelTests
{
    [Test]
    public void ImplicitOperator_ShouldConvertCourseInformation_ToCourseModel()
    {
        var courseInformation = new CourseInformation
        {
            LarsCode = 2,
            ProvidersCount = 10,
            TotalProvidersCount = 20
        };

        CourseModel courseModel = courseInformation;

        Assert.Multiple(() =>
        {
            Assert.That(courseModel, Is.Not.Null);
            Assert.That(courseModel.LarsCode, Is.EqualTo(courseInformation.LarsCode));
            Assert.That(courseModel.ProvidersCount, Is.EqualTo(courseInformation.ProvidersCount));
            Assert.That(courseModel.TotalProvidersCount, Is.EqualTo(courseInformation.TotalProvidersCount));
        });
    }
}
