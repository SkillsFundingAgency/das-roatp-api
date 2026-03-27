using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Functions;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

public class SendForecastsReminderEmailsFunctionTests
{
    private Fixture _fixture;
    private Mock<ILogger<SendForecastsReminderEmailsFunction>> _loggerMock;
    private Mock<IProviderCourseTypesReadRepository> _providerCourseTypesReadRepository;
    private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepository;
    private Mock<IProviderCourseForecastRepository> _providerCourseForecastRepository;
    private Mock<ICourseManagementOuterApiClient> _courseManagementOuterApiClient;
    private IOptions<ForecastEmailConfiguration> _options;
    private SendForecastsReminderEmailsFunction _sut;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new Mock<ILogger<SendForecastsReminderEmailsFunction>>();
        _providerCourseTypesReadRepository = new Mock<IProviderCourseTypesReadRepository>();
        _providerCoursesReadRepository = new Mock<IProviderCoursesReadRepository>();
        _providerCourseForecastRepository = new Mock<IProviderCourseForecastRepository>();
        _courseManagementOuterApiClient = new Mock<ICourseManagementOuterApiClient>();

        _fixture = new();

        var cfg = _fixture.Create<ForecastEmailConfiguration>();
        _options = Options.Create(cfg);

        // default API client behaviour
        _courseManagementOuterApiClient.Setup(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>())).ReturnsAsync(true);

        _sut = new SendForecastsReminderEmailsFunction(
            _loggerMock.Object,
            _providerCoursesReadRepository.Object,
            _providerCourseForecastRepository.Object,
            _courseManagementOuterApiClient.Object,
            _options,
            _providerCourseTypesReadRepository.Object);
    }

    [Test]
    public async Task Run_ProviderHasUpToDateForecast_EmailNotSent()
    {
        const int ukprn = 10012002;
        const string larsCode = "ZSC00001";
        // Arrange
        var allowedProviders = new List<int> { ukprn };
        _providerCourseTypesReadRepository
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allowedProviders);

        List<UkprnLarsCodeModel> allShortCourses =
        [
            new UkprnLarsCodeModel ( ukprn, larsCode )
        ];

        _providerCoursesReadRepository
            .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allShortCourses);

        var recentlyUpdatedForecast = new List<ProviderCourseWithLastForecastDate>
        {
            new ProviderCourseWithLastForecastDate(ukprn, larsCode, DateTime.UtcNow)
        };

        _providerCourseForecastRepository
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentlyUpdatedForecast);

        // Act
        await _sut.Run(null!, CancellationToken.None);

        // Assert
        _courseManagementOuterApiClient.Verify(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()), Times.Never);
    }

    [Test]
    public async Task Run_ProvidersNeedingReminderForMultipleCourses_SendsOneEmailPerProvider()
    {
        // Arrange
        var allowedProviders = new List<int> { 1001, 1002, 1003 };
        _providerCourseTypesReadRepository
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allowedProviders);

        List<UkprnLarsCodeModel> allShortCourses =
        [
            new UkprnLarsCodeModel (1001, "L1" ),
            new UkprnLarsCodeModel (1001, "L2" ),
            new UkprnLarsCodeModel (1002, "L3" ),
            new UkprnLarsCodeModel (1002, "L2" )
        ];
        _providerCoursesReadRepository
            .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allShortCourses);


        _providerCourseForecastRepository
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _sut.Run(null!, CancellationToken.None);

        _courseManagementOuterApiClient.Verify(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()), Times.Exactly(2));
        _courseManagementOuterApiClient.Verify(c => c.Post(It.Is<string>(s => s.Equals("providers/1001/emails")), It.IsAny<ProviderEmailModel>()), Times.Once);
        _courseManagementOuterApiClient.Verify(c => c.Post(It.Is<string>(s => s.Equals("providers/1002/emails")), It.IsAny<ProviderEmailModel>()), Times.Once);
    }

    [Test]
    public async Task Run_ProvidersHasMultipleCourses_WithAtLeastOneOverdue_ReminderEmailIsSent()
    {
        const int ukprnDoesNotRequireReminder = 10012001;
        const int ukprnRequiresReminder = 10012002;
        const string larsCode1 = "ZSC00001";
        const string larsCode2 = "ZSC00002";

        // Arrange
        _providerCourseTypesReadRepository
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync([ukprnDoesNotRequireReminder, ukprnRequiresReminder]);

        List<UkprnLarsCodeModel> allShortCourses =
        [
            new UkprnLarsCodeModel (ukprnRequiresReminder, larsCode1 ),
            new UkprnLarsCodeModel (ukprnRequiresReminder, larsCode2 ),
            new UkprnLarsCodeModel (ukprnDoesNotRequireReminder, larsCode1 ),
            new UkprnLarsCodeModel (ukprnDoesNotRequireReminder, larsCode2 ),
        ];
        _providerCoursesReadRepository
            .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allShortCourses);

        List<ProviderCourseWithLastForecastDate> upToDateForecast =
        [
            new ProviderCourseWithLastForecastDate(ukprnDoesNotRequireReminder, larsCode1, DateTime.UtcNow),
            new ProviderCourseWithLastForecastDate(ukprnDoesNotRequireReminder, larsCode2, DateTime.UtcNow),
            new ProviderCourseWithLastForecastDate(ukprnRequiresReminder, larsCode1, DateTime.UtcNow)
        ];
        _providerCourseForecastRepository
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(upToDateForecast);

        // Act
        await _sut.Run(null!, CancellationToken.None);

        _courseManagementOuterApiClient.Verify(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()), Times.Once);
        _courseManagementOuterApiClient.Verify(c => c.Post(It.Is<string>(s => s.Equals("providers/10012002/emails")), It.IsAny<ProviderEmailModel>()), Times.Once);
    }

    [Test]
    public async Task Run_ProcessesBatches_WithDelayBetweenBatches()
    {
        const string larsCode = "ZSC00001";

        List<int> ukprns = [.. Enumerable.Range(10011001, ForecastEmailConfiguration.BatchSize + 1)];
        _providerCourseTypesReadRepository
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ukprns);

        _providerCoursesReadRepository
                   .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
                   .ReturnsAsync([.. ukprns.Select(u => new UkprnLarsCodeModel(u, larsCode))]);

        _providerCourseForecastRepository
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        var postCallTimes = new List<DateTime>();
        _courseManagementOuterApiClient
            .Setup(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()))
            .Returns<string, ProviderEmailModel>((uri, model) =>
            {
                postCallTimes.Add(DateTime.UtcNow);
                return Task.FromResult(true);
            });

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await _sut.Run(null!, CancellationToken.None);
        sw.Stop();

        // Assert - ensure all posts happened
        _courseManagementOuterApiClient.Verify(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()), Times.Exactly(ukprns.Count));
        postCallTimes.Should().HaveCount((ukprns.Count));
        // There should be a delay between the last call of the first batch (index of batch size -1) and the first call of the second batch (index of batch size )
        var gap = (postCallTimes[ForecastEmailConfiguration.BatchSize] - postCallTimes[ForecastEmailConfiguration.BatchSize - 1]).TotalMilliseconds;
        Assert.That(gap, Is.GreaterThanOrEqualTo(ForecastEmailConfiguration.EmailThrottlingInSeconds - 100), $"Expected gap >= 900ms between batches but was {gap}ms");
    }
}
