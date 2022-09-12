﻿using AutoFixture.NUnit3;
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
        private Mock<IProviderCourseLocationsBulkRepository> _providerCourseLocationsBulkRepositoryMock;
        private Mock<IProviderCourseLocationsReadRepository> _providerCourseLocationsReadRepositoryMock;
        private BulkDeleteProviderCourseLocationsCommandHandler _sut;

        private readonly ProviderCourseLocation _regionalLocation = new ProviderCourseLocation { Id = 1, Location = new ProviderLocation { LocationType = LocationType.Regional } };
        private readonly ProviderCourseLocation _nationalLocation = new ProviderCourseLocation { Id = 2, Location = new ProviderLocation { LocationType = LocationType.National } };
        private readonly ProviderCourseLocation _providerLocation = new ProviderCourseLocation { Id = 3, Location = new ProviderLocation { LocationType = LocationType.Provider } };


        [SetUp]
        public void Before_Each_Test()
        {
            _providerCourseLocationsBulkRepositoryMock = new Mock<IProviderCourseLocationsBulkRepository>();
            _providerCourseLocationsReadRepositoryMock = new Mock<IProviderCourseLocationsReadRepository>();
            _sut = new BulkDeleteProviderCourseLocationsCommandHandler(_providerCourseLocationsBulkRepositoryMock.Object, _providerCourseLocationsReadRepositoryMock.Object, Mock.Of<ILogger<BulkDeleteProviderCourseLocationsCommandHandler>>());
        }

        [Test, AutoData]
        public async Task Handler_NoCourseLocations_ReturnsZero(BulkDeleteProviderCourseLocationsCommand request)
        {
            _providerCourseLocationsReadRepositoryMock
                .Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode))
                .ReturnsAsync(new List<ProviderCourseLocation>());

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(0);
            _providerCourseLocationsBulkRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Handler_NoProviderLocations_ReturnsZero(int ukprn, int larsCode, string userId)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteProviderCourseLocationOption.DeleteProviderLocations, userId);
            var locations = new List<ProviderCourseLocation>() 
            {
                _nationalLocation,
                _regionalLocation
            };
            _providerCourseLocationsReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(0);
            _providerCourseLocationsBulkRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Handler_NoEmployerLocations_ReturnsZero(int ukprn, int larsCode, string userId)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteProviderCourseLocationOption.DeleteEmployerLocations, userId);
            var locations = new List<ProviderCourseLocation>()
            {
                _providerLocation
            };
            _providerCourseLocationsReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(0);
            _providerCourseLocationsBulkRepositoryMock.Verify(d => d.BulkDelete(It.IsAny<IEnumerable<int>>()), Times.Never);
        }

        [Test, AutoData]
        public async Task Handler_DeleteEmployerLocations_CallsRepositoryWithEmployerLocationIds(int ukprn, int larsCode, string userId)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteProviderCourseLocationOption.DeleteEmployerLocations, userId);
            var locations = new List<ProviderCourseLocation>()
            {
                _nationalLocation,
                _regionalLocation,
                _providerLocation
            };
            _providerCourseLocationsReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(2);
            var expectedList = new [] { _nationalLocation.Id, _regionalLocation.Id };
            _providerCourseLocationsBulkRepositoryMock.Verify(d => d.BulkDelete(It.Is<IEnumerable<int>>(l => !l.Except(expectedList).Any())));
        }

        [Test, AutoData]
        public async Task Handler_DeleteEmployerLocations_CallsRepositoryWithProviderLocationIds(int ukprn, int larsCode,string userId)
        {
            var request = new BulkDeleteProviderCourseLocationsCommand(ukprn, larsCode, DeleteProviderCourseLocationOption.DeleteProviderLocations, userId);
            var locations = new List<ProviderCourseLocation>()
            {
                _nationalLocation,
                _regionalLocation,
                _providerLocation
            };
            _providerCourseLocationsReadRepositoryMock.Setup(r => r.GetAllProviderCourseLocations(request.Ukprn, request.LarsCode)).ReturnsAsync(locations);

            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(1);
            var expectedList = new[] { _providerLocation.Id };
            _providerCourseLocationsBulkRepositoryMock.Verify(d => d.BulkDelete(It.Is<IEnumerable<int>>(l => !l.Except(expectedList).Any())));
        }
    }
}
