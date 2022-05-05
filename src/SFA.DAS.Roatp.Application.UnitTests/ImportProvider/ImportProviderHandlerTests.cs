using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ImportProvider;
using SFA.DAS.Roatp.Application.Services;
using SFA.DAS.Roatp.Domain.Interfaces;
using Provider = SFA.DAS.Roatp.Domain.ApiModels.Import.Provider;

namespace SFA.DAS.Roatp.Application.UnitTests.ImportProvider
{
    [TestFixture]
    public class ImportProviderHandlerTests
    {
        private Mock<IProviderImportRepository> _importRepository;
        private Mock<IMapProviderService> _mapProviderService;
        private ImportProviderHandler _handler;
        private const int ukprn = 12344321;

        [SetUp]
        public void Before_each_test()
        {
            _importRepository = new Mock<IProviderImportRepository>();
            _mapProviderService = new Mock<IMapProviderService>();
            _handler = new ImportProviderHandler(_importRepository.Object,_mapProviderService.Object,
                Mock.Of<ILogger<ImportProviderHandler>>());
        }

        [Test]
        public async Task ImportProvider_courseInserted()
        {
            var cdProvider = new Provider();
            var request = new ImportProviderRequest { Provider = cdProvider };
            var provider = new Domain.Entities.Provider {Ukprn =ukprn};
            _mapProviderService.Setup(x => x.MapProvider(cdProvider)).ReturnsAsync(provider);
            _importRepository.Setup(x => x.ImportProviderDetails(provider)).ReturnsAsync(true);
            var result = await _handler.Handle(request, new CancellationToken());
            Assert.IsTrue(result);
            _mapProviderService.Verify(x=>x.MapProvider(cdProvider),Times.Once);
            _importRepository.Verify(x => x.ImportProviderDetails(provider), Times.Once);
        }

        [Test]
        public async Task ImportProvider_mappingFailed_courseNotInserted()
        {
            var cdProvider = new Provider();
            var request = new ImportProviderRequest { Provider = cdProvider };
            var provider = new Domain.Entities.Provider { Ukprn = ukprn };
            _mapProviderService.Setup(x => x.MapProvider(cdProvider)).ReturnsAsync((Domain.Entities.Provider)null);
            var result = await _handler.Handle(request, new CancellationToken());
            Assert.IsFalse(result);
            _mapProviderService.Verify(x => x.MapProvider(cdProvider), Times.Once);
            _importRepository.Verify(x => x.ImportProviderDetails(provider), Times.Never);
        }

        [Test]
        public async Task ImportProvider_courseNotInserted()
        {
            var cdProvider = new Provider();
            var request = new ImportProviderRequest{ Provider = cdProvider };
            var provider = new Domain.Entities.Provider { Ukprn = ukprn };
            _mapProviderService.Setup(x => x.MapProvider(cdProvider)).ReturnsAsync(provider);
            _importRepository.Setup(x => x.ImportProviderDetails(provider)).ReturnsAsync(false);
            var result = await _handler.Handle(request, new CancellationToken());
            Assert.IsFalse(result);
            _mapProviderService.Verify(x => x.MapProvider(cdProvider), Times.Once);
            _importRepository.Verify(x => x.ImportProviderDetails(provider), Times.Once);
        }
    }
}