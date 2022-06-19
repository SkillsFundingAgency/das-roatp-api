using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.BulkDelete
{
    [TestFixture]
    public class BulkDeleteProviderCourseLocationsCommandHandlerTests
    {
        private Mock<IProviderCourseLocationDeleteRepository> _providerCourseLocationDeleteRepositoryMock;
        private Mock<IProviderCourseLocationReadRepository> _providerCourseLocationReadRepositoryMock;
        private BulkDeleteProviderCourseLocationsCommandHandler _sut;

        private readonly ProviderCourseLocation _regionalLocation = new ProviderCourseLocation { Id = 1, Location = new ProviderLocation { LocationType = LocationType.Regional } };
        private readonly ProviderCourseLocation _nationalLocation = new ProviderCourseLocation { Id = 2, Location = new ProviderLocation { LocationType = LocationType.National } };
        private readonly ProviderCourseLocation _providerLocation = new ProviderCourseLocation { Id = 3, Location = new ProviderLocation { LocationType = LocationType.Provider } };


        [SetUp]
        public void Before_Each_Test()
        {
            _providerCourseLocationDeleteRepositoryMock = new Mock<IProviderCourseLocationDeleteRepository>();
            _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationReadRepository>();
            _sut = new BulkDeleteProviderCourseLocationsCommandHandler(_providerCourseLocationDeleteRepositoryMock.Object, _providerCourseLocationReadRepositoryMock.Object, Mock.Of<ILogger<BulkDeleteProviderCourseLocationsCommandHandler>>());
        }

        [Test, AutoData]
        public async Task Handler_NoCourseLocations_ReturnsZero(BulkDeleteProviderCourseLocationsCommand request)
        {
            _providerCourseLocationReadRepositoryMock
                .Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode))
                .ReturnsAsync(new List<ProviderCourseLocation>());

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(0);
            _providerCourseLocationDeleteRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Handler_NoProviderLocations_ReturnsZero(int ukprn, int larsCode)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteOptions.DeleteProviderLocations);
            var locations = new List<ProviderCourseLocation>() 
            {
                _nationalLocation,
                _regionalLocation
            };
            _providerCourseLocationReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(0);
            _providerCourseLocationDeleteRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Handler_NoEmployerLocations_ReturnsZero(int ukprn, int larsCode)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteOptions.DeleteEmployerLocations);
            var locations = new List<ProviderCourseLocation>()
            {
                _providerLocation
            };
            _providerCourseLocationReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(0);
            _providerCourseLocationDeleteRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Handler_DeleteEmployerLocations_CallsRepositoryWithEmployerLocationIds(int ukprn, int larsCode)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteOptions.DeleteEmployerLocations);
            var locations = new List<ProviderCourseLocation>()
            {
                _nationalLocation,
                _regionalLocation,
                _providerLocation
            };
            _providerCourseLocationReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(2);
            var expectedList = new [] { _nationalLocation.Id, _regionalLocation.Id };
            _providerCourseLocationDeleteRepositoryMock.Verify(d => d.BulkDelete(It.Is<IEnumerable<int>>(l => !l.Except(expectedList).Any())));
        }

        [Test, AutoData]
        public async Task Handler_DeleteEmployerLocations_CallsRepositoryWithProviderLocationIds(int ukprn, int larsCode)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteOptions.DeleteProviderLocations);
            var locations = new List<ProviderCourseLocation>()
            {
                _nationalLocation,
                _regionalLocation,
                _providerLocation
            };
            _providerCourseLocationReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(1);
            var expectedList = new[] { _providerLocation.Id };
            _providerCourseLocationDeleteRepositoryMock.Verify(d => d.BulkDelete(It.Is<IEnumerable<int>>(l => !l.Except(expectedList).Any())));
        }
    }
}
