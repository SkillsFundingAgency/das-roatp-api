using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Locations.Commands.CreateLocation;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Locations.Commands.CreateLocation
{
    [TestFixture]
    public class CreateProviderLocationCommandHandlerTests
    {
        private Mock<IProvidersReadRepository> _providerReadRepositoryMock;
        private Mock<IProviderLocationsWriteRepository> _providerLocationsWriteRepositoryMock;
        private CreateProviderLocationCommandHandler _sut;
        private CreateProviderLocationCommand _command;
        private Provider _provider;

        [SetUp]
        public async Task Before_Each_Test()
        {
            var fixture = new Fixture();
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _command = fixture.Create<CreateProviderLocationCommand>();
            _provider = fixture.Create<Provider>();
            var providerLocation = fixture.Create<ProviderLocation>();

            _providerReadRepositoryMock = new Mock<IProvidersReadRepository>();
            _providerReadRepositoryMock.Setup(r => r.GetByUkprn(_command.Ukprn)).ReturnsAsync(_provider);

            _providerLocationsWriteRepositoryMock = new Mock<IProviderLocationsWriteRepository>();
            _providerLocationsWriteRepositoryMock.Setup(r => r.Create(It.IsAny<ProviderLocation>())).ReturnsAsync(providerLocation);

            _sut = new CreateProviderLocationCommandHandler(_providerReadRepositoryMock.Object, _providerLocationsWriteRepositoryMock.Object, Mock.Of<ILogger<CreateProviderLocationCommandHandler>>());

            await _sut.Handle(_command, new CancellationToken());
        }

        [Test]
        public void ThenGetsProviderUringUkprn()
        {
            _providerReadRepositoryMock.Verify(r => r.GetByUkprn(_command.Ukprn));
        }

        [Test]
        public void ThenConvertsCommandToEntityAndCallsCreateRepository()
        {
            _providerLocationsWriteRepositoryMock.Verify(r => r.Create(It.Is<ProviderLocation>(p => p.ProviderId == _provider.Id && p.LocationName == _command.LocationName)));
        }
    }
}
