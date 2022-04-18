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
        private Mock<IProviderCourseReadRepository> _mockCoursesRepo;
        private Mock<IProviderReadRepository> _mockProviderReadRepository;

        [SetUp]
        public void Setup()
        {
            _mockCoursesRepo = new Mock<IProviderCourseReadRepository>();
            _mockProviderReadRepository = new Mock<IProviderReadRepository>();
        }

        [Test]
        public async Task GetCourse_ProviderFound_ReturnsCourseModel()
        {
            var provider = new Provider() { Id = 123, Ukprn = 10012002 };
            _mockProviderReadRepository.Setup(p => p.GetProvider(It.IsAny<int>())).ReturnsAsync(provider);

            var course = new ProviderCourse
            {
                LarsCode = 1,
                IfateReferenceNumber = "ST1001"
            };

            _mockCoursesRepo.Setup(m => m.GetProviderCourse(provider.Id, course.LarsCode)).ReturnsAsync(course);

            var sut = new GetProviderCoursesService(_mockCoursesRepo.Object, _mockProviderReadRepository.Object);

            var model = await sut.GetCourse(provider.Ukprn, course.LarsCode);

            Assert.IsNotNull(model);
        }

        [Test]
        public async Task GetCourse_ProviderNotFound_ReturnsNull()
        {
            var sut = new GetProviderCoursesService(_mockCoursesRepo.Object, _mockProviderReadRepository.Object);

            var model = await sut.GetCourse(ukprn: 1, larsCode: 2);

            Assert.IsNull(model);
        }
    }
}
