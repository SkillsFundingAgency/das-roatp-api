using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.Delete
{
    [TestFixture]
    public class DeleteProviderCourseLocationCommandHandlerTests
    {
        private Mock<IProviderCourseLocationsDeleteRepository> _providerCourseLocationDeleteRepositoryMock;
        private Mock<IProviderCourseLocationReadRepository> _providerCourseLocationReadRepositoryMock;
        private DeleteProviderCourseLocationCommandHandler _sut;

        private readonly ProviderCourseLocation _regionalLocation = new ProviderCourseLocation { Id = 1, Location = new ProviderLocation { LocationType = LocationType.Regional } };
        private readonly ProviderCourseLocation _nationalLocation = new ProviderCourseLocation { Id = 2, Location = new ProviderLocation { LocationType = LocationType.National } };
        private readonly ProviderCourseLocation _providerLocation = new ProviderCourseLocation { Id = 3, Location = new ProviderLocation { LocationType = LocationType.Provider } };


        [SetUp]
        public void Before_Each_Test()
        {
            _providerCourseLocationDeleteRepositoryMock = new Mock<IProviderCourseLocationsDeleteRepository>();
            _providerCourseLocationReadRepositoryMock = new Mock<IProviderCourseLocationReadRepository>();
            _sut = new DeleteProviderCourseLocationCommandHandler(_providerCourseLocationDeleteRepositoryMock.Object, Mock.Of<ILogger<DeleteProviderCourseLocationCommandHandler>>());
        }

        [Test, AutoData]
        public async Task Handler_NoCourseLocations_ReturnsZero(DeleteProviderCourseLocationCommand request)
        {
            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(Unit.Value);
            _providerCourseLocationDeleteRepositoryMock.Verify(d => d.Delete(It.IsAny<int>()), Times.Once);
        }
    }
}
