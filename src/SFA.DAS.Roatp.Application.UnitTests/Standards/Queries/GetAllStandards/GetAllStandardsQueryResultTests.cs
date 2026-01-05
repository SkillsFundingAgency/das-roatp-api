using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Standards.Queries.GetAllStandards
{
    [TestFixture]
    public class GetAllStandardsQueryResultTests
    {
        [Test]
        public void Constructor_WithNullList_ProducesEmptyStandardsList()
        {
            var sut = new GetAllStandardsQueryResult(null);

            sut.Standards.Should().NotBeNull();
            sut.Standards.Should().BeEmpty();
        }

        [Test]
        public void Constructor_MapsAllStandardProperties_ToStandardModel()
        {
            var source = new Standard
            {
                StandardUId = "STD-UID-1",
                LarsCode = "LARS-123",
                IfateReferenceNumber = "IFATE-1",
                Level = 4,
                Title = "Test Standard",
                ApprovalBody = "Some Body",
                IsRegulatedForProvider = true,
                Route = "Test Route",
                ApprenticeshipType = ApprenticeshipType.Apprenticeship,
                CourseType = CourseType.Apprenticeship
            };

            var sut = new GetAllStandardsQueryResult(new List<Standard> { source });

            sut.Standards.Should().HaveCount(1);
            var model = sut.Standards[0];

            model.StandardUId.Should().Be(source.StandardUId);
            model.LarsCode.Should().Be(source.LarsCode);
            model.IfateReferenceNumber.Should().Be(source.IfateReferenceNumber);
            model.Level.Should().Be(source.Level);
            model.Title.Should().Be(source.Title);
            model.ApprovalBody.Should().Be(source.ApprovalBody);
            model.IsRegulatedForProvider.Should().Be(source.IsRegulatedForProvider);
            model.Route.Should().Be(source.Route);
            model.ApprenticeshipType.Should().Be(source.ApprenticeshipType);
            model.CourseType.Should().Be(source.CourseType);
        }
    }
}