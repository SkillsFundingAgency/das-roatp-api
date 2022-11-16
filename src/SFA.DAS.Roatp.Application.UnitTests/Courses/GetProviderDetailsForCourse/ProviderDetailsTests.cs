using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProviderDetailsForCourse
{
    [TestFixture]
    public class ProviderDetailsTests
    {
        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromModelBase(ProviderCourseDetailsModelBase modelBase)
        {
            var model = (ProviderDetails)modelBase;

            Assert.That(model, Is.Not.Null);
            
            model.Should().BeEquivalentTo(modelBase, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
            );
            Assert.AreEqual(modelBase.LegalName, model.Name);
            Assert.AreEqual(modelBase.StandardContactUrl, model.ContactUrl); 
            Assert.AreEqual(modelBase.Distance, model.ProviderHeadOfficeDistanceInMiles);
        }

        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromSummaryModel(ProviderCourseDetailsSummaryModel summaryModel)
        {
            var model = (ProviderDetails)summaryModel;

            Assert.That(model, Is.Not.Null);

            model.Should().BeEquivalentTo(summaryModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.StandardContactUrl)
                .Excluding(s => s.Distance)
                .Excluding(s=>s.ProviderId)
            );
            Assert.AreEqual(summaryModel.LegalName, model.Name);
            Assert.AreEqual(summaryModel.StandardContactUrl, model.ContactUrl);
            Assert.AreEqual(summaryModel.Distance, model.ProviderHeadOfficeDistanceInMiles);
        }

        [Test]
        public void Operator_PopulatesModelFromSummaryModelNull()
        { 
            var model = (ProviderDetails)(ProviderCourseDetailsSummaryModel)null;
            Assert.That(model, Is.Null);
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
