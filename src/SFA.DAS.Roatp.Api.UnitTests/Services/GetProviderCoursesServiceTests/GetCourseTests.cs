using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.UnitTests.Services.GetProviderCoursesServiceTests
{
    [TestFixture]
    public class GetCourseTests
    {
        private Mock<IProviderCourseReadRepository> _mockProviderCourseRepository;
        private Mock<IProviderReadRepository> _mockProviderReadRepository;
        private Mock<ICourseReadRepository> _mockCourseRepository;

        [SetUp]
        public void Setup()
        {
            _mockProviderCourseRepository = new Mock<IProviderCourseReadRepository>();
            _mockProviderReadRepository = new Mock<IProviderReadRepository>();
            _mockCourseRepository = new Mock<ICourseReadRepository>();
        }

        [Test]
        public async Task GetCourse_ProviderFound_ReturnsCourseModel()
        {
            var provider = new Provider() { Id = 123, Ukprn = 10012002 };
            _mockProviderReadRepository.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);

            var providerCourses = new ProviderCourse
            {
                LarsCode = 1,
                IfateReferenceNumber = "ST1001",
                IsImported = true
            };
            List<Course> coursesLookUp = new List<Course>
            {
                new Course
                {
                    LarsCode = 1,
                    Level = 1,
                    IfateReferenceNumber = "ST1001",
                    Title = "Test training-1"
                },
                new Course
                {
                    LarsCode = 2,
                    Level = 2,
                    IfateReferenceNumber = "ST1002",
                    Title = "Test training-2"
                },
            };

            _mockProviderCourseRepository.Setup(m => m.GetProviderCourse(provider.Id, providerCourses.LarsCode)).ReturnsAsync(providerCourses);

            _mockCourseRepository.Setup(c => c.GetAllCourses()).ReturnsAsync(coursesLookUp);

            var sut = new GetProviderCoursesService(_mockProviderCourseRepository.Object, _mockProviderReadRepository.Object, _mockCourseRepository.Object, Mock.Of<ILogger<GetProviderCoursesService>>());

            var model = await sut.GetCourse(provider.Ukprn, providerCourses.LarsCode);

            Assert.IsNotNull(model);
            Assert.IsNotEmpty(model.CourseName);
            Assert.IsNotEmpty(model.IfateReferenceNumber);
            Assert.IsTrue(model.Level > 0);
        }

        [Test]
        public async Task GetCourse_ProviderNotFound_ReturnsNull()
        {
            var sut = new GetProviderCoursesService(_mockProviderCourseRepository.Object, _mockProviderReadRepository.Object, _mockCourseRepository.Object, Mock.Of<ILogger<GetProviderCoursesService>>());

            var model = await sut.GetCourse(ukprn: 1, larsCode: 2);

            Assert.IsNull(model);
        }
    }
}
