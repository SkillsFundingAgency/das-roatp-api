using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Api.UnitTests.Models
{
    [TestFixture]
    public class ProviderCourseModelTests
    {
        [Test]
        public void ProviderCourseOperator_ReturnsProviderCourseModel()
        {
            var course = new ProviderCourse() { LarsCode = 1};
            var model = (ProviderCourseModel) course;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.LarsCode, Is.EqualTo(course.LarsCode));
            Assert.That(model.DeliveryModels, Contains.Item(DeliveryModel.Regular));
            Assert.That(model.DeliveryModels, Contains.Item(DeliveryModel.PortableFlexiJob));
        }

        [Test]
        public void ProviderCourseOperator_UpdateCourseInjectsExpectedValues()
        {
            var ifateReferenceNumber = "ST1001";
            var course = new ProviderCourse() { LarsCode = 1, IfateReferenceNumber =  "ST002"};
            var model = (ProviderCourseModel)course;

            var standardLookup = new Standard
            {
                IfateReferenceNumber = ifateReferenceNumber,
                Title = "course title",
                Level = 1,
                Version = "1.1",
                ApprovalBody = "ABC"
            };
            
            model.UpdateCourseDetails(standardLookup.IfateReferenceNumber, standardLookup.Level,standardLookup.Title, standardLookup.Version,standardLookup.ApprovalBody);

            Assert.That(model, Is.Not.Null);
            Assert.AreEqual(ifateReferenceNumber,model.IfateReferenceNumber);
            Assert.AreEqual(standardLookup.Title,model.CourseName);
            Assert.AreEqual(standardLookup.Level,model.Level);
            Assert.AreEqual(standardLookup.Version, model.Version); 
            Assert.AreEqual(standardLookup.ApprovalBody, model.ApprovalBody);
        }
    }
}
