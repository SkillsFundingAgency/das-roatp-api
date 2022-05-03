using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Application.ReloadStandards;
using SFA.DAS.Roatp.Application.StandardsCount;
using SFA.DAS.Roatp.Domain.Interfaces;

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
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
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
