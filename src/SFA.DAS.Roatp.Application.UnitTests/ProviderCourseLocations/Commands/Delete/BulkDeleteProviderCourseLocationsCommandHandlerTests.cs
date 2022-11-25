using AutoFixture.NUnit3;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.Delete;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.Delete
{
    [TestFixture]
    public class DeleteProviderCourseLocationCommandHandlerTests
    {
        private Mock<IProviderCourseLocationsWriteRepository> _providerCourseLocationsWriteRepositoryMock;
        private DeleteProviderCourseLocationCommandHandler _sut;

        [SetUp]
        public void Before_Each_Test()
        {
            _providerCourseLocationsWriteRepositoryMock = new Mock<IProviderCourseLocationsWriteRepository>();
            _sut = new DeleteProviderCourseLocationCommandHandler(_providerCourseLocationsWriteRepositoryMock.Object, Mock.Of<ILogger<DeleteProviderCourseLocationCommandHandler>>());
        }

        [Test, AutoData]
        public async Task Handler_NoCourseLocations_ReturnsZero(DeleteProviderCourseLocationCommand request)
        {
            var result = await _sut.Handle(request, new CancellationToken());

            result.Should().Be(Unit.Value);
            _providerCourseLocationsWriteRepositoryMock.Verify(d => d.Delete(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
    }
}
