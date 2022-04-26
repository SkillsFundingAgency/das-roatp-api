using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Services;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.UnitTests.Services.GetStandardsServiceTests
{
    [TestFixture]
    public class GetStandardsServiceTests
    {
        private Mock<IGetStandardsRepository> _getStandardsRepository;
        private GetStandardsService _service;

        [SetUp]
        public void Before_each_test()
        {
            _getStandardsRepository = new Mock<IGetStandardsRepository>();
            _service = new GetStandardsService(_getStandardsRepository.Object,
                Mock.Of<ILogger<GetStandardsService>>());
        }

        [Test]
        public async Task GetStandards_ReturnsStandardsPresent()
        {
            var standardUid = "1";
            var standards = new List<Domain.Entities.Standard>
            {
                new Domain.Entities.Standard { StandardUId = standardUid, IfateReferenceNumber = "2", LarsCode = 3, Level = 4, Title = "5", Version = "6" }
            };

            _getStandardsRepository.Setup(x => x.GetStandards()).ReturnsAsync(standards);

            var result = await _service.GetStandards();
           
            var standardReturned = result.First();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(standardUid,standardReturned.StandardUid);
            _getStandardsRepository.Verify(x => x.GetStandards(), Times.Once);
        }
    }
}
