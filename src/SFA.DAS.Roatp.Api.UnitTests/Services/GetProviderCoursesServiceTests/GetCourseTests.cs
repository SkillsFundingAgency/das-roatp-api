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
        private Mock<IStandardReadRepository> _mockCourseRepository;

        [SetUp]
        public void Setup()
        {
            _mockProviderCourseRepository = new Mock<IProviderCourseReadRepository>();
            _mockProviderReadRepository = new Mock<IProviderReadRepository>();
            _mockCourseRepository = new Mock<IStandardReadRepository>();
        }

        [Test]
        public async Task GetCourse_ProviderFound_ReturnsCourseModel()
        {
            var provider = new Provider() { Id = 123, Ukprn = 10012002 };
            _mockProviderReadRepository.Setup(p => p.GetByUkprn(It.IsAny<int>())).ReturnsAsync(provider);

            var providerCourses = new ProviderCourse
            {
                LarsCode = 1,
                IsImported = true
            };
            var standardLookUp = new Standard
            {
                LarsCode = 1,
                Level = 1,
                Title = "Test training-1",
                ApprovalBody = "approval body 1"
            };

            _mockProviderCourseRepository.Setup(m => m.GetProviderCourse(provider.Id, providerCourses.LarsCode)).ReturnsAsync(providerCourses);

            _mockCourseRepository.Setup(c => c.GetStandard(providerCourses.LarsCode)).ReturnsAsync(standardLookUp);

            var sut = new GetProviderCoursesService(_mockProviderCourseRepository.Object, _mockProviderReadRepository.Object, _mockCourseRepository.Object, Mock.Of<ILogger<GetProviderCoursesService>>());

            var model = await sut.GetCourse(provider.Ukprn, providerCourses.LarsCode);

            Assert.IsNotNull(model);
            Assert.IsNotEmpty(model.CourseName);
            Assert.IsNotEmpty(model.IfateReferenceNumber);
            Assert.IsTrue(model.Level > 0);
            Assert.AreEqual(standardLookUp.IfateReferenceNumber, model.IfateReferenceNumber);
            Assert.AreEqual(standardLookUp.Level, model.Level);
            Assert.AreEqual(standardLookUp.Title, model.CourseName);
            Assert.AreEqual(standardLookUp.ApprovalBody, model.ApprovalBody);
            Assert.AreEqual(standardLookUp.Version,model.Version);
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
