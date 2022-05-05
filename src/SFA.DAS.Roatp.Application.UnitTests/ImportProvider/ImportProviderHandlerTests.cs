using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ImportProvider;
using SFA.DAS.Roatp.Application.Services;
using SFA.DAS.Roatp.Domain.ApiModels.Import;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.ImportProvider
{
    [TestFixture]
    public class ImportProviderHandlerTests
    {
        private Mock<IImportCourseDetailsRepository> _importRepository;
        private Mock<IMapProviderService> _mapProviderService;
        private ImportProviderHandler _handler;
        private const int ukprn = 12344321;

        [SetUp]
        public void Before_each_test()
        {
            _importRepository = new Mock<IImportCourseDetailsRepository>();
            _mapProviderService = new Mock<IMapProviderService>();
            _handler = new ImportProviderHandler(_importRepository.Object,_mapProviderService.Object,
                Mock.Of<ILogger<ImportProviderHandler>>());
        }

        [Test]
        public async Task ImportProvider_courseInserted()
        {
            var cdProvider = new CdProvider();
            var request = new ImportProviderRequest() { CdProvider = cdProvider };
            var provider = new Provider {Ukprn =ukprn};
            _mapProviderService.Setup(x => x.MapProvider(cdProvider)).ReturnsAsync(provider);
            _importRepository.Setup(x => x.ImportCourseDetails(provider)).ReturnsAsync(true);
            var result = await _handler.Handle(request, new CancellationToken());
            Assert.IsTrue(result);
            _mapProviderService.Verify(x=>x.MapProvider(cdProvider),Times.Once);
            _importRepository.Verify(x => x.ImportCourseDetails(provider), Times.Once);
        }

        [Test]
        public async Task ImportProvider_courseNotInserted()
        {
            var cdProvider = new CdProvider();
            var request = new ImportProviderRequest() { CdProvider = cdProvider };
            var provider = new Provider { Ukprn = ukprn };
            _mapProviderService.Setup(x => x.MapProvider(cdProvider)).ReturnsAsync(provider);
            _importRepository.Setup(x => x.ImportCourseDetails(provider)).ReturnsAsync(false);
            var result = await _handler.Handle(request, new CancellationToken());
            Assert.IsFalse(result);
            _mapProviderService.Verify(x => x.MapProvider(cdProvider), Times.Once);
            _importRepository.Verify(x => x.ImportCourseDetails(provider), Times.Once);
        }
    }
}