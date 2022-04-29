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
            var course = new ProviderCourse() { LarsCode = 1, IfateReferenceNumber = "ST1001" };
            var model = (ProviderCourseModel) course;

            Assert.That(model, Is.Not.Null);
            Assert.That(model.LarsCode, Is.EqualTo(course.LarsCode));
            Assert.That(model.DeliveryModels, Contains.Item(DeliveryModel.Regular));
            Assert.That(model.DeliveryModels, Contains.Item(DeliveryModel.PortableFlexiJob));
        }
    }
}
