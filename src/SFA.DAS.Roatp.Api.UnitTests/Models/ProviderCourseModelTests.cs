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
            var model = (ProviderCourseModel) new ProviderCourse() { LarsCode = 1, IfateReferenceNumber = "ST1001" };

            Assert.That(model, Is.Not.Null);
            Assert.That(model.LarsCode, Is.EqualTo(1));
            Assert.That(model.IfateReferenceNumber, Is.EqualTo("ST1001"));
            Assert.That(model.DeliveryModels, Contains.Item(DeliveryModel.Regular));
            Assert.That(model.DeliveryModels, Contains.Item(DeliveryModel.PortableFlexiJob));
        }
    }
}
