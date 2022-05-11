using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Timers;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.RoatpV2Api.Models;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.StandardsApi;
using SFA.DAS.Roatp.Jobs.Infrastructure.ApiClients.StandardsApi.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions
{
    public class ReloadStandardsCacheFunctionTests
    {
        private Mock<IGetActiveStandardsApiClient> _standardsGetAllActiveApiClient;
        private Mock<IReloadStandardsApiClient> _roatpV2UpdateStandardDetailsApiClient;
        private Mock<ILogger<ReloadStandardsCacheFunction>> _logger;
        private ReloadStandardsCacheFunction _function;


        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<ReloadStandardsCacheFunction>>();
            _standardsGetAllActiveApiClient = new Mock<IGetActiveStandardsApiClient>();
            _roatpV2UpdateStandardDetailsApiClient = new Mock<IReloadStandardsApiClient>();
            _function = new ReloadStandardsCacheFunction(_standardsGetAllActiveApiClient.Object, _roatpV2UpdateStandardDetailsApiClient.Object);
        }

        [Test]
        public async Task Successful_LogsInformationMessage()
        {
            var standards = new List<Standard> { new Standard { StandardUid = "1", IfateReferenceNumber = "2", LarsCode = 3, Level = "4", Title = "course title 5", Version = "1.1" } };
            var standardList = new StandardList
            {
                Standards = standards
            };

            _standardsGetAllActiveApiClient.Setup(x => x.GetActiveStandards()).ReturnsAsync(standardList);
            _roatpV2UpdateStandardDetailsApiClient.Setup(x => x.ReloadStandardsDetails(It.IsAny<StandardsRequest>()))
                .ReturnsAsync(HttpStatusCode.OK);

            var timerInfo = new TimerInfo(new ConstantSchedule(TimeSpan.Zero), new ScheduleStatus(), true);
            await _function.Run(timerInfo, _logger.Object);
            _standardsGetAllActiveApiClient.Verify(x => x.GetActiveStandards(), Times.Once);
            _roatpV2UpdateStandardDetailsApiClient.Verify(x => x.ReloadStandardsDetails(It.IsAny<StandardsRequest>()), Times.Once);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Exactly(2));
        }

        [Test]
        public async Task Unsuccessful_LogsErrorMessage()
        {
            var standards = new List<Standard> { new Standard { StandardUid = "1", IfateReferenceNumber = "2", LarsCode = 3, Level = "4", Title = "course title 5", Version = "1.1" } };
            var standardList = new StandardList
            {
                Standards = standards
            };

            _standardsGetAllActiveApiClient.Setup(x => x.GetActiveStandards()).ReturnsAsync(standardList);
            _roatpV2UpdateStandardDetailsApiClient.Setup(x => x.ReloadStandardsDetails(It.IsAny<StandardsRequest>()))
                .ReturnsAsync(HttpStatusCode.BadRequest);

            var timerInfo = new TimerInfo(new ConstantSchedule(TimeSpan.Zero), new ScheduleStatus(), true);
            await _function.Run(timerInfo, _logger.Object);
            _standardsGetAllActiveApiClient.Verify(x => x.GetActiveStandards(), Times.Once);
            _roatpV2UpdateStandardDetailsApiClient.Verify(x => x.ReloadStandardsDetails(It.IsAny<StandardsRequest>()), Times.Once);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Test]
        public async Task Unsuccessful_StandardListNotReturned_LogsErrorMessage()
        {
            _standardsGetAllActiveApiClient.Setup(x => x.GetActiveStandards()).ReturnsAsync((StandardList)null);

            var timerInfo = new TimerInfo(new ConstantSchedule(TimeSpan.Zero), new ScheduleStatus(), true);
            await _function.Run(timerInfo, _logger.Object);
            _standardsGetAllActiveApiClient.Verify(x => x.GetActiveStandards(), Times.Once);
            _roatpV2UpdateStandardDetailsApiClient.Verify(x => x.ReloadStandardsDetails(It.IsAny<StandardsRequest>()), Times.Never);
            _logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
            _logger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
