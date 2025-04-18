﻿using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Courses.Queries.GetProviderDetailsForCourse;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Courses.GetProvidersForCourse
{
    [TestFixture]
    public class ProviderSummaryTests
    {
        [Test]
        public void Operator_PopulatesModelFromSummaryModelNull()
        {
            var model = (ProviderSummation)(ProviderCourseSummaryModel)null;
            Assert.That(model, Is.Null);
        }

        [Test, RecursiveMoqAutoData]
        public void Operator_PopulatesModelFromSummaryModel(ProviderCourseSummaryModel summaryModel)
        {
            var model = (ProviderSummation)summaryModel;

            Assert.That(model, Is.Not.Null);

            model.Should().BeEquivalentTo(summaryModel, c => c
                .Excluding(s => s.LegalName)
                .Excluding(s => s.Distance)
                .Excluding(s => s.ProviderId)
            );
            summaryModel.LegalName.Should().Be(model.Name);
            summaryModel.Distance.Should().Be((double?)model.ProviderHeadOfficeDistanceInMiles);
        }
    }
}
