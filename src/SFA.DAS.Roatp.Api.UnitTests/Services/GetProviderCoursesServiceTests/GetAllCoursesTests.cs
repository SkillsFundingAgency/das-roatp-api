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
    public class GetAllCoursesTests
    {
        private readonly List<ProviderCourse> _providerCourses = new List<ProviderCourse>
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

        private readonly List<Standard> _standardsLookUp = new List<Standard>
        {
            new Standard
            {
                LarsCode = 1,
                Level = 1,
                IfateReferenceNumber = "ST1001",
                Title = "Test training-1"
            },
            new Standard
            {
                LarsCode = 2,
                Level = 2,
                IfateReferenceNumber = "ST1002",
                Title = "Test training-2"
            },
        };

        private Mock<IProviderCourseReadRepository> _mockProviderCourseRepository;
        private Mock<IProviderReadRepository> _mockProviderReadRepository;
        private Mock<IStandardReadRepository> _mockStandardRepository;


        [SetUp]
        public void Setup()
        {
            _mockProviderCourseRepository = new Mock<IProviderCourseReadRepository>();
            _mockProviderReadRepository = new Mock<IProviderReadRepository>();
            _mockStandardRepository = new Mock<IStandardReadRepository>();
        }

        [Test]
        public async Task GetAllCourses_ProviderFound_ReturnsListOfCourseModel()
        {
            _mockProviderReadRepository.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(new Provider());

            _mockProviderCourseRepository.Setup(m => m.GetAllProviderCourses(It.IsAny<int>())).ReturnsAsync(_providerCourses);

            _mockStandardRepository.Setup(c => c.GetAllCourses()).ReturnsAsync(_standardsLookUp);

            var sut = new GetProviderCoursesService(_mockProviderCourseRepository.Object, _mockProviderReadRepository.Object, _mockStandardRepository.Object, Mock.Of<ILogger<GetProviderCoursesService>>());

            var courses = await sut.GetAllCourses(ukprn: 1);

            Assert.AreEqual(2, courses.Count);
            Assert.IsNotEmpty(courses[0].CourseName);
            Assert.IsNotEmpty(courses[0].IfateReferenceNumber);
            Assert.IsTrue(courses[0].Level > 0);
            Assert.IsNotEmpty(courses[1].CourseName);
            Assert.IsNotEmpty(courses[1].IfateReferenceNumber);
            Assert.IsTrue(courses[1].Level > 0);
        }

        [Test]
        public async Task GetAllCourses_ProviderNotFound_ReturnsEmptyList()
        {
            var sut = new GetProviderCoursesService(_mockProviderCourseRepository.Object, _mockProviderReadRepository.Object, _mockStandardRepository.Object, Mock.Of<ILogger<GetProviderCoursesService>>());

            var courses = await sut.GetAllCourses(ukprn: 1);

            Assert.AreEqual(0, courses.Count);
        }
    }
}
