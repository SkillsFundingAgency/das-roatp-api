using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class GetProviderDetailsForCourseQueryResultTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(
            ProviderAndCourseDetailsWithDistance providerAndCourseDetailsWithDistance)
        {
            var model = (GetProviderDetailsForCourseQueryResult)providerAndCourseDetailsWithDistance;

            Assert.That(model, Is.Not.Null);
            model.Should().BeEquivalentTo(providerAndCourseDetailsWithDistance,c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl));
            Assert.AreEqual(providerAndCourseDetailsWithDistance.LegalName, model.Name);
            Assert.AreEqual(providerAndCourseDetailsWithDistance.StandardContactUrl,model.ContactUrl);
        }
    }
}
