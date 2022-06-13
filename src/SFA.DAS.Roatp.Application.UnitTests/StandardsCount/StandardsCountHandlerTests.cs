using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.StandardsCount;
using SFA.DAS.Roatp.Domain.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Application.UnitTests.StandardsCount
{
    [TestFixture]
    public class Tests
    {
        private Mock<IGetStandardsCountRepository> _repository;
        private StandardsCountHandler _handler;

        [SetUp]
        public void Before_each_test()
        {
            _repository = new Mock<IGetStandardsCountRepository>();
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
            Assert.AreEqual(standardsCount,result);
            _repository.Verify(x => x.GetStandardsCount(), Times.Once);
        }
    }
}
