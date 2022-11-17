using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class GetProviderDetailsForCourseQueryResultTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromEntity(
            ProviderCourseDetailsModel providerCourseDetailsModel)
        {
            var model = (GetProviderDetailsForCourseQueryResult)providerCourseDetailsModel;

            Assert.That(model, Is.Not.Null);
            model.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
                .Excluding(s=>s.Ukprn));
            Assert.AreEqual(providerCourseDetailsModel.LegalName, model.Name);
            Assert.AreEqual(providerCourseDetailsModel.StandardContactUrl, model.ContactUrl);
            Assert.AreEqual(providerCourseDetailsModel.Distance, model.ProviderHeadOfficeDistanceInMiles);
        }

        [Test]
        public void Operator_ReturnsNullIfNullEntity()
        {
            var details = (ProviderCourseDetailsModel)null;
            var model = (GetProviderDetailsForCourseQueryResult)details;

            Assert.IsNull(model);
        }
    }
}