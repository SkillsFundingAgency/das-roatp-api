using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class ProviderDetailsTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromSummaryModel(ProviderCourseSummaryModel summaryModel)
        {
            var model = (ProviderDetails)summaryModel;

            Assert.That(model, Is.Not.Null);

            model.Should().BeEquivalentTo(summaryModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.Distance)
                .Excluding(s=>s.ProviderId)
                .Excluding(s=>s.IsApprovedByRegulator)
            );

            Assert.AreEqual(summaryModel.LegalName, model.Name);
            Assert.AreEqual(summaryModel.Distance, model.ProviderHeadOfficeDistanceInMiles);
        }

        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromProviderCourseDetailsModel(ProviderCourseDetailsModel providerCourseDetailsModel)
        {
            var model = (ProviderDetails)providerCourseDetailsModel;

            Assert.That(model, Is.Not.Null);

            model.Should().BeEquivalentTo(providerCourseDetailsModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
            );
            Assert.AreEqual(providerCourseDetailsModel.LegalName, model.Name);
            Assert.AreEqual(providerCourseDetailsModel.StandardContactUrl, model.ContactUrl);
            Assert.AreEqual(providerCourseDetailsModel.Distance, model.ProviderHeadOfficeDistanceInMiles);
            Assert.AreEqual(providerCourseDetailsModel.MarketingInfo,model.MarketingInfo);
        }

        [Test]
        public void Operator_PopulatesModelFromProviderCourseDetailsModelNull()
        {
            var model = (ProviderDetails)(ProviderCourseDetailsModel)null;
            Assert.That(model, Is.Null);
        }
    }
}
