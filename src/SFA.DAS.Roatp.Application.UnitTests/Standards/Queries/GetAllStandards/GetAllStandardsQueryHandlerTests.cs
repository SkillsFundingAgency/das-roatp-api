using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Standards.Queries.GetAllStandards;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Application.UnitTests.Standards.Queries.GetAllStandards
{
    [TestFixture]
    public class GetAllStandardsQueryHandlerTests
    {
        [Test]
        public async Task Handle_WithOutCourseTypeFilter_ReturnsListOfAllStandards()
        {
            var expectedStandards = new List<Standard>
            {
                new() { LarsCode = "1", Title = "standard 1" },
                new() { LarsCode = "2", Title = "standard 2" }
            };
            CourseType? courseTypeFilter = null;
            var repositoryMock = new Mock<IStandardsReadRepository>();
            repositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(expectedStandards);

            var sut = new GetAllStandardsQueryHandler(repositoryMock.Object, Mock.Of<ILogger<GetAllStandardsQueryHandler>>());
            var queryRequest = new GetAllStandardsQuery(courseTypeFilter);
            var result = await sut.Handle(queryRequest, CancellationToken.None);

            result.Standards.Should().BeEquivalentTo(new GetAllStandardsQueryResult(expectedStandards).Standards);
        }

        [Test]
        public async Task Handle_WithCourseType_FiltersStandards()
        {
            var expectedStandards = new List<Standard>
            {
                new() { LarsCode = "1", Title = "standard 1", CourseType = CourseType.Apprenticeship },
                new() { LarsCode = "2", Title = "short 1", CourseType = CourseType.ApprenticeshipUnit},
                new() { LarsCode = "3", Title = "short 2", CourseType = CourseType.ApprenticeshipUnit}
            };
            var courseTypeFilter = CourseType.ApprenticeshipUnit;

            var repositoryMock = new Mock<IStandardsReadRepository>();
            repositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(expectedStandards);

            var sut = new GetAllStandardsQueryHandler(repositoryMock.Object, Mock.Of<ILogger<GetAllStandardsQueryHandler>>());
            var queryRequest = new GetAllStandardsQuery(courseTypeFilter);
            var result = await sut.Handle(queryRequest, CancellationToken.None);

            result.Standards.Should().HaveCount(2);
            result.Standards.Should().OnlyContain(s => s.CourseType == courseTypeFilter);
        }

        [Test]
        public async Task Handle_WithCourseType_NoMatches_ReturnsEmptyListAndLogsZero()
        {
            var expectedStandards = new List<Standard>
            {
                new() { LarsCode = "1", Title = "standard 1", CourseType = CourseType.Apprenticeship },
                new() { LarsCode = "2", Title = "standard 2", CourseType = CourseType.Apprenticeship }
            };
            var courseTypeFilter = CourseType.ApprenticeshipUnit;
            var repositoryMock = new Mock<IStandardsReadRepository>();
            repositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(expectedStandards);

            var loggerMock = new Mock<ILogger<GetAllStandardsQueryHandler>>();
            var sut = new GetAllStandardsQueryHandler(repositoryMock.Object, loggerMock.Object);

            var queryRequest = new GetAllStandardsQuery(courseTypeFilter);
            var result = await sut.Handle(queryRequest, CancellationToken.None);

            result.Standards.Should().BeEmpty();

            loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Returning 0 standards")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
