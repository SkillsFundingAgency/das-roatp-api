using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.StandardsCount;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Application.UnitTests.StandardsCount
{
    [TestFixture]
    public class Tests
    {
        private Mock<IStandardsReadRepository> _repository;
        private StandardsCountHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IStandardsReadRepository>();
            _handler = new StandardsCountHandler(_repository.Object,
                Mock.Of<ILogger<StandardsCountHandler>>());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10000)]
        public async Task StandardsCount_ReturnsExpectedCount(int standardsCount)
        {
            _repository.Setup(x => x.GetStandardsCount()).ReturnsAsync(standardsCount);
            var result = await _handler.Handle(new StandardsCountRequest(), new CancellationToken());
            result.Should().Be(standardsCount);
            _repository.Verify(x => x.GetStandardsCount(), Times.Once);
        }
    }
}
