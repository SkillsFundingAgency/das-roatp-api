using System.Collections.Generic;
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
            model.Should().BeEquivalentTo(providerCourseDetailsModel,c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s=>s.Distance));
            Assert.AreEqual(providerCourseDetailsModel.LegalName, model.Name);
            Assert.AreEqual(providerCourseDetailsModel.StandardContactUrl,model.ContactUrl);
            Assert.AreEqual(providerCourseDetailsModel.Distance,model.ProviderHeadOfficeDistanceInMiles);
        }

         [Test]
         public void Operator_ReturnsNullIfNullEntity()
         {
             var details = (ProviderCourseDetailsModel)null;
             var model = (GetProviderDetailsForCourseQueryResult)details;
        
             Assert.IsNull(model);
         }
         
         [TestCase(false, false, false, false, "")]
         [TestCase(true, false, false, false, "BlockRelease")]
         [TestCase(false, true, false, false, "DayRelease")]
         [TestCase(true, true, false, false, "BlockRelease|DayRelease")]
         [TestCase(false, false, true, false, "100PercentEmployer")]
         [TestCase(false, false, false, true, "100PercentEmployer")]
         [TestCase(false, false, true, true, "100PercentEmployer")]
         [TestCase(true, false, true, false, "BlockRelease|100PercentEmployer")]
         [TestCase(true, false, false, true, "BlockRelease|100PercentEmployer")]
         [TestCase(true, false, true, true, "BlockRelease|100PercentEmployer")]
         [TestCase(false, true, false, true, "DayRelease|100PercentEmployer")]
         [TestCase(false, true, true, false, "DayRelease|100PercentEmployer")]
         [TestCase(false, true, true, true, "DayRelease|100PercentEmployer")]
         [TestCase(true, true, true, true, "BlockRelease|DayRelease|100PercentEmployer")]
         public void DeliveryModes_Expected(bool blockRelease, bool dayRelease, bool regional, bool national, string expectedResult)
         {
             var deliveryModes = new List<DeliveryModel>();
             if (blockRelease)
             {
                 deliveryModes.Add(new DeliveryModel {DeliveryModeType = DeliveryModeType.BlockRelease});
             }

             if (dayRelease)
             {
                 deliveryModes.Add(new DeliveryModel { DeliveryModeType = DeliveryModeType.DayRelease });
             }

             if (regional)
             {
                 deliveryModes.Add(new DeliveryModel { DeliveryModeType = DeliveryModeType.Workplace });
             }

             if (national)
             {
                 deliveryModes.Add(new DeliveryModel { DeliveryModeType = DeliveryModeType.Workplace });
             }

            var details = new GetProviderDetailsForCourseQueryResult
            {
                DeliveryModels = deliveryModes
            };



            Assert.AreEqual(expectedResult, details.DeliveryModes);
         }
    }
}
