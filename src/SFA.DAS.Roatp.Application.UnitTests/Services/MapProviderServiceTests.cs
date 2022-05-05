using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Services;
using SFA.DAS.Roatp.Application.StandardsCount;
using SFA.DAS.Roatp.Domain.ApiModels.Import;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.Services
{
    [TestFixture]
    public class MapProviderServiceTests
    {
        
        private  Mock<IStandardReadRepository> _repository;
        private  Mock<ILogger<MapProviderService>> _logger;
        private MapProviderService _service;
        private const int ukprn = 12344321;
        private const int larsCode = 123;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IStandardReadRepository>();
            _logger = new Mock<ILogger<MapProviderService>>();
            _service = new MapProviderService(_repository.Object,
                _logger.Object);
        }

        [Test]
        public async Task MapCdProvider_Successful()
        {
            var standardUid = "XXX_1.1";
            var version = "1.1";
            var cdProvider = new CdProvider { Ukprn = ukprn, Courses = new List<CdProviderCourse> {new CdProviderCourse {LarsCode = larsCode}}};
            _repository.Setup(x => x.GetStandard(larsCode))
                .ReturnsAsync(new Standard { StandardUId = standardUid, Version = version });
            var provider =await _service.MapProvider(cdProvider);
            Assert.AreEqual(ukprn,provider.Ukprn);
            Assert.AreEqual(1,provider.Courses.Count);
            Assert.AreEqual(1, provider.Courses.First().Versions.Count);
            Assert.AreEqual(standardUid,provider.Courses.First().Versions.First().StandardUId);
            Assert.AreEqual(version, provider.Courses.First().Versions.First().Version);
            _logger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Never);
        }

        [Test]
        public async Task MapCdProvider_NoMatchingStandardWarning()
        {
            var cdProvider = new CdProvider { Ukprn = ukprn, Courses = new List<CdProviderCourse> { new CdProviderCourse { LarsCode = larsCode } } };
            _repository.Setup(x => x.GetStandard(larsCode))
                .ReturnsAsync((Standard)null);
            var provider = await _service.MapProvider(cdProvider);
            Assert.AreEqual(ukprn, provider.Ukprn);
            Assert.AreEqual(1, provider.Courses.Count);
            Assert.AreEqual(1, provider.Courses.First().Versions.Count);
            Assert.AreEqual(null, provider.Courses.First().Versions.First().StandardUId);
            Assert.AreEqual(null, provider.Courses.First().Versions.First().Version);
            _logger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
