using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Models;
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

            var course = new ProviderCourse
            {
                LarsCode = 1,
                IfateReferenceNumber = "ST1001"
            };

            _mockProviderCourseRepository.Setup(m => m.GetProviderCourse(provider.Id, course.LarsCode)).ReturnsAsync(course);

            var sut = new GetProviderCoursesService(_mockProviderCourseRepository.Object, _mockProviderReadRepository.Object, _mockCourseRepository.Object);

            var model = await sut.GetCourse(provider.Ukprn, course.LarsCode);

            Assert.IsNotNull(model);
        }

        [Test]
        public async Task GetCourse_ProviderNotFound_ReturnsNull()
        {
            var sut = new GetProviderCoursesService(_mockProviderCourseRepository.Object, _mockProviderReadRepository.Object, _mockCourseRepository.Object);

            var model = await sut.GetCourse(ukprn: 1, larsCode: 2);

            Assert.IsNull(model);
        }
    }
}
