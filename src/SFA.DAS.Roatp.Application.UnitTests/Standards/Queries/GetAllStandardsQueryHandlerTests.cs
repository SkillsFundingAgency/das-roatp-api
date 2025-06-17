using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.Standards.Queries;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.Standards.Queries
{
    [TestFixture]
    public class GetAllStandardsQueryHandlerTests
    {
        [Test]
        public async Task Handle_ReturnsListOfStandards()
        {
            List<Standard> standards = new List<Standard>
            {
                new() { LarsCode = 1, Title = "standard 1" },
                new() { LarsCode = 2, Title = "standard 2" }
            };

            var repositoryMock = new Mock<IStandardsReadRepository>();
            repositoryMock.Setup(r => r.GetAllStandards()).ReturnsAsync(standards);
            var sut = new GetAllStandardsQueryHandler(repositoryMock.Object, Mock.Of<ILogger<GetAllStandardsQueryHandler>>());

            var result = await sut.Handle(It.IsAny<GetAllStandardsQuery>(), It.IsAny<CancellationToken>());

            result.Standards.Should().BeEquivalentTo(standards);
        }
    }
}
