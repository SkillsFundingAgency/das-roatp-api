using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ProviderCourseLocations.Commands.BulkDelete;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.ProviderCourseLocations.Commands.BulkDelete
{
    [TestFixture]
    public class BulkDeleteProviderCourseLocationsCommandHandlerTests
    {
        [Test, AutoData]
        public async Task Handler_CallsRepository(BulkDeleteProviderCourseLocationsCommand request)
        {
            var repoMock = new Mock<IProviderCourseLocationDeleteRepository>();

            repoMock.Setup(r => r.BulkDelete(request.Ukprn, request.LarsCode, request.DeleteOptions == DeleteOptions.DeleteProviderLocations)).ReturnsAsync(1);

            var sut = new BulkDeleteProviderCourseLocationsCommandHandler(repoMock.Object);

            var result = await sut.Handle(request, new CancellationToken());

            result.Should().Be(1);
        }
    }
}
