using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetAllProviderCourses;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderCourseModelTests
    {
        [TestCase(true)]
        [TestCase(false)]
        public void ProviderCourseOperator_ReturnsProviderCourseModel(bool hasPortableFlexiJobOption)
        {
            var course = new ProviderCourse() { LarsCode = 1, HasPortableFlexiJobOption = hasPortableFlexiJobOption };
            var model = (ProviderCourseModel)course;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.LarsCode, Is.EqualTo(course.LarsCode));
            Assert.That(model.HasPortableFlexiJobOption, Is.EqualTo(hasPortableFlexiJobOption));
        }

        [Test]
        public void ProviderCourseOperator_UpdateCourseInjectsExpectedValues()
        {
            var course = new ProviderCourse() { LarsCode = 1 };
            var model = (ProviderCourseModel)course;

            var standardLookup = new Standard
            {
                Title = "course title",
                Level = 1,
                Version = "1.1",
                ApprovalBody = "ABC",
                IsRegulatedForProvider = true
            };

            model.AttachCourseDetails(standardLookup.IfateReferenceNumber, standardLookup.Level, standardLookup.Title, standardLookup.Version, standardLookup.ApprovalBody, standardLookup.IsRegulatedForProvider);

            Assert.That(model, Is.Not.Null);
            standardLookup.Title.Should().Be(model.CourseName);
            standardLookup.Level.Should().Be(model.Level);
            standardLookup.Version.Should().Be(model.Version);
            standardLookup.ApprovalBody.Should().Be(model.ApprovalBody);
        }
    }
}
