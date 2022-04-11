using System.Threading.Tasks;
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
        private Mock<IProviderRepository> _mockproviderRepo;

        [SetUp]
        public void Setup()
        {
            _mockproviderRepo = new Mock<IProviderRepository>();
        }

        [Test]
        public async Task GetProvider_ProviderFound_ReturnsCourseModel()
        {
            var provider = new Provider() { Id = 123, Ukprn = 10012002, LegalName ="Test" };
            _mockproviderRepo.Setup(m => m.GetProvider(provider.Ukprn)).ReturnsAsync(provider);

            var sut = new ProviderService(_mockproviderRepo.Object);

            var model = await sut.GetProvider(provider.Ukprn);

            Assert.IsNotNull(model);
        }

        [Test]
        public async Task GetProvider_ProviderNotFound_ReturnsNull()
        {
            var sut = new ProviderService(_mockproviderRepo.Object);

            var model = await sut.GetProvider(ukprn: 1);

            Assert.IsNull(model);
        }

        [Test]
        public async Task UpdateProvider_ChangeConfirmedDetails_ReturnProvider()
        {
            var provider = new Provider() { Id = 123, Ukprn = 10012002, LegalName = "Test", HasConfirmedDetails = true, ConfirmedDetailsOn = System.DateTime.Now};
            _mockproviderRepo.Setup(m => m.UpdateProvider(provider.Ukprn, true)).ReturnsAsync(provider);

            var sut = new ProviderService(_mockproviderRepo.Object);

            var model = await sut.UpdateProvider(provider.Ukprn, true);

            Assert.IsNotNull(model);
        }
    }
}
