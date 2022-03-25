using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.UnitTests.Services.GetProviderCoursesServiceTests
{
    [TestFixture]
    public class GetAllCoursesTests
    {
        private readonly List<ProviderCourse> _courses = new List<ProviderCourse>
        {
            new ProviderCourse
            {
                LarsCode = 1,
                IfateReferenceNumber = "ST1001"
            },
            new ProviderCourse
            {
                LarsCode = 2,
                IfateReferenceNumber = "ST1002"
            }
        };

        private Mock<IProviderCourseReadRepository> _mockCourseRepository;
        private Mock<IProviderReadRepository> _mockProviderReadRepository;


        [SetUp]
        public void Setup()
        {
            _mockCourseRepository = new Mock<IProviderCourseReadRepository>();
            _mockProviderReadRepository = new Mock<IProviderReadRepository>();
        }

        [Test]
        public async Task GetAllCourses_ProviderFound_ReturnsListOfCourseModel()
        {
            _mockProviderReadRepository.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

            _mockCourseRepository.Setup(m => m.GetAllProviderCourses(It.IsAny<int>())).ReturnsAsync(_courses);

            var sut = new GetProviderCoursesService(_mockCourseRepository.Object, _mockProviderReadRepository.Object);

            var courses = await sut.GetAllCourses(ukprn: 1);

            Assert.AreEqual(2, courses.Count);
        }

        [Test]
        public async Task GetAllCourses_ProviderNotFound_ReturnsEmptyList()
        {
            var sut = new GetProviderCoursesService(_mockCourseRepository.Object, _mockProviderReadRepository.Object);

            var courses = await sut.GetAllCourses(ukprn: 1);

            Assert.AreEqual(0, courses.Count);
        }
    }
}
