using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.UnitTests.Services.GetStandardsCountServiceTests
{
    [TestFixture]
    public class GetStandardsCountServiceTests
    {
        private Mock<IGetStandardsCountRepository> _getStandardsRepository;
        private GetStandardsCountService _countService;

        [SetUp]
        public void Before_each_test()
        {
            _getStandardsRepository = new Mock<IGetStandardsCountRepository>();
            _countService = new GetStandardsCountService(_getStandardsRepository.Object,
                Mock.Of<ILogger<GetStandardsCountService>>());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(100000)]
        public async Task GetStandardsCount_ReturnsStandardsCountValue(int result)
        {
            _getStandardsRepository.Setup(x => x.GetStandardsCount()).ReturnsAsync(result);

            var actualResult = await _countService.GetStandardsCount();
            
            Assert.AreEqual(result, actualResult);
            _getStandardsRepository.Verify(x => x.GetStandardsCount(), Times.Once);
        }
    }
}
