using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.UnitTests.Services
{
    [TestFixture]
    public class ProviderServiceTests
    {
        private Mock<IProviderReadRepository> _mockproviderRepo;

        [SetUp]
        public void Setup()
        {
            _mockproviderRepo = new Mock<IProviderReadRepository>();
        }

        [Test]
        public async Task GetProvider_ProviderFound_ReturnsProviderModel()
        {
            var provider = new Provider() { Id = 123, Ukprn = 10012002, LegalName ="Test" };
            _mockproviderRepo.Setup(m => m.GetProvider(provider.Ukprn)).ReturnsAsync(provider);

            var sut = new GetProviderService(_mockproviderRepo.Object, Mock.Of<ILogger<GetProviderService>>());

            var model = await sut.GetProvider(provider.Ukprn);

            Assert.IsNotNull(model);
        }

        [Test]
        public async Task GetProvider_ProviderNotFound_ReturnsNull()
        {
            var sut = new GetProviderService(_mockproviderRepo.Object, Mock.Of<ILogger<GetProviderService>>());

            var model = await sut.GetProvider(ukprn: 1);

            Assert.IsNull(model);
        }
    }
}
