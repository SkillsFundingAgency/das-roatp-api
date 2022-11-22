﻿using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourse.Queries.GetProviderCourse;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.Roatp.Application.UnitTests.Providers.Queries.GetCourse
{
    [TestFixture]
    public class CourseQueryHandlerTests
    {
        [Test, RecursiveMoqAutoData()]
        public async Task Handle_ReturnsResult(
            Domain.Entities.ProviderCourse course,
            [Frozen] Mock<IProviderCoursesReadRepository> providerCoursesReadRepositoryMock,
            [Frozen] Mock<IStandardsReadRepository> standardsReadRepositoryMock,
            GetCourseQuery query,
            GetCourseQueryHandler sut,
            CancellationToken cancellationToken)
        {
            providerCoursesReadRepositoryMock.Setup(r => r.GetProviderCourseByUkprn(query.Ukprn, query.LarsCode)).ReturnsAsync(course);
            var standard =  new Standard { LarsCode = course.LarsCode };
            standardsReadRepositoryMock.Setup(r => r.GetStandard(course.LarsCode)).ReturnsAsync(standard);
            var result = await sut.Handle(query, cancellationToken);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Course.LarsCode, Is.EqualTo(course.LarsCode));
        }
    }
}
