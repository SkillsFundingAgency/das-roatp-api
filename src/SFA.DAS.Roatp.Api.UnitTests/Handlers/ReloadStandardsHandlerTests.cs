using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Api.Handlers;
using SFA.DAS.Roatp.Api.Models;
using SFA.DAS.Roatp.Domain.Interfaces;

namespace SFA.DAS.Roatp.Api.UnitTests.Handlers
{
    [TestFixture]
    public class ReloadStandardsHandlerTests
    {
        private  Mock<IReloadStandardsRepository> _reloadStandardsRepository;
        private ReloadStandardsHandler _handler;
        
        [SetUp]
        public void Before_each_test()
        { 
            _reloadStandardsRepository = new Mock<IReloadStandardsRepository>();
            _handler = new ReloadStandardsHandler(_reloadStandardsRepository.Object,
                Mock.Of<ILogger<ReloadStandardsHandler>>());
        }

        [Test]
        public async Task ReloadStandardsData_NoStandardsLoaded_ReturnFalse()
        {
            var standards = new List<Standard>();
            var request = new ReloadStandardsRequest { Standards = standards };
            var result = await _handler.Handle(request, new CancellationToken());
            Assert.IsFalse(result);
            _reloadStandardsRepository.Verify(x => x.ReloadStandards(It.IsAny<List<Domain.Entities.Standard>>()), Times.Never);
        }

        [Test]
        public async Task ReloadStandardsData_StandardsLoaded_ReturnsTrue()
        {
            var standards = new List<Standard>
            {
                new Standard { StandardUid = "1", IfateReferenceNumber = "2", LarsCode = 3, Level = "4", Title = "5", Version = "6" }
            };

            var request = new ReloadStandardsRequest { Standards = standards };
            _reloadStandardsRepository.Setup(x => x.ReloadStandards(It.IsAny<List<Domain.Entities.Standard>>())).ReturnsAsync(true);

            var result = await _handler.Handle(request, new CancellationToken());
            Assert.IsTrue(result);
            _reloadStandardsRepository.Verify(x=>x.ReloadStandards(It.IsAny<List<Domain.Entities.Standard>>()),Times.Once);
        }
    }
}
