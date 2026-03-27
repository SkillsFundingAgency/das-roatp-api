using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Domain.Models;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

public class SendForecastsReminderEmailsFunctionTests
{
    private Fixture _fixture;
    private Mock<ILogger<SendForecastsReminderEmailsFunction>> _loggerMock;
    private Mock<IProviderCourseTypesReadRepository> _providerCourseTypesReadRepositoryMock;
    private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepositoryMock;
    private Mock<IProviderCourseForecastRepository> _providerCourseForecastRepositoryMock;
    private Mock<IProviderEmailProcessingService> _providerEmailProcessingServiceMock;
    private ForecastEmailConfiguration _forecastEmailConfiguration;
    private SendForecastsReminderEmailsFunction _sut;

    [SetUp]
    public void SetUp()
    {
        _loggerMock = new();
        _providerCourseTypesReadRepositoryMock = new();
        _providerCoursesReadRepositoryMock = new();
        _providerCourseForecastRepositoryMock = new();
        _providerEmailProcessingServiceMock = new();

        _fixture = new();

        _forecastEmailConfiguration = _fixture.Create<ForecastEmailConfiguration>();

        _sut = new SendForecastsReminderEmailsFunction(
            _loggerMock.Object,
            _providerCoursesReadRepositoryMock.Object,
            _providerCourseForecastRepositoryMock.Object,
            _providerEmailProcessingServiceMock.Object,
            Options.Create(_forecastEmailConfiguration),
            _providerCourseTypesReadRepositoryMock.Object);
    }

    [Test]
    public async Task SendForecastsReminderEmailsFunction_ProviderHasUpToDateForecast_EmailNotSent()
    {
        const int ukprn = 10012002;
        const string larsCode = "ZSC00001";
        // Arrange
        var allowedProviders = new List<int> { ukprn };
        _providerCourseTypesReadRepositoryMock
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allowedProviders);

        List<UkprnLarsCodeModel> allShortCourses =
        [
            new UkprnLarsCodeModel ( ukprn, larsCode )
        ];

        _providerCoursesReadRepositoryMock
            .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allShortCourses);

        var recentlyUpdatedForecast = new List<ProviderCourseWithLastForecastDate>
        {
            new ProviderCourseWithLastForecastDate(ukprn, larsCode, DateTime.UtcNow)
        };

        _providerCourseForecastRepositoryMock
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(recentlyUpdatedForecast);

        // Act
        await _sut.Run(new TimerInfo(), CancellationToken.None);

        // Assert
        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.IsAny<IEnumerable<ProviderEmailModel>>()), Times.Never);
    }

    [Test]
    public async Task SendForecastsReminderEmailsFunction_ProvidersNeedingReminderForMultipleCourses_SendsOneEmailPerProvider()
    {
        // Arrange
        var allowedProviders = new List<int> { 1001, 1002, 1003 };
        _providerCourseTypesReadRepositoryMock
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allowedProviders);

        List<UkprnLarsCodeModel> allShortCourses =
        [
            new UkprnLarsCodeModel (1001, "L1" ),
            new UkprnLarsCodeModel (1001, "L2" ),
            new UkprnLarsCodeModel (1002, "L3" ),
            new UkprnLarsCodeModel (1002, "L2" )
        ];
        _providerCoursesReadRepositoryMock
            .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allShortCourses);


        _providerCourseForecastRepositoryMock
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _sut.Run(new TimerInfo(), CancellationToken.None);

        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => models.Count() == 2)), Times.Once);
        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => VerifyProviderEmailModelsIncludesUkprns(models, "1001", "1002"))), Times.Once);
    }

    private static bool VerifyProviderEmailModelsIncludesUkprns(IEnumerable<ProviderEmailModel> models, params string[] ukprns)
        => models.All(m => ukprns.Contains(m.Tokens[ProviderEmailTokens.Ukprn]));

    [Test]
    public async Task SendForecastsReminderEmailsFunction_ProvidersHasMultipleCourses_WithAtLeastOneOverdue_ReminderEmailIsSent()
    {
        const int ukprnDoesNotRequireReminder = 10012001;
        const int ukprnRequiresReminder = 10012002;
        const string larsCode1 = "ZSC00001";
        const string larsCode2 = "ZSC00002";

        // Arrange
        _providerCourseTypesReadRepositoryMock
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync([ukprnDoesNotRequireReminder, ukprnRequiresReminder]);

        List<UkprnLarsCodeModel> allShortCourses =
        [
            new UkprnLarsCodeModel (ukprnRequiresReminder, larsCode1 ),
            new UkprnLarsCodeModel (ukprnRequiresReminder, larsCode2 ),
            new UkprnLarsCodeModel (ukprnDoesNotRequireReminder, larsCode1 ),
            new UkprnLarsCodeModel (ukprnDoesNotRequireReminder, larsCode2 ),
        ];
        _providerCoursesReadRepositoryMock
            .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync(allShortCourses);

        List<ProviderCourseWithLastForecastDate> upToDateForecast =
        [
            new ProviderCourseWithLastForecastDate(ukprnDoesNotRequireReminder, larsCode1, DateTime.UtcNow),
            new ProviderCourseWithLastForecastDate(ukprnDoesNotRequireReminder, larsCode2, DateTime.UtcNow),
            new ProviderCourseWithLastForecastDate(ukprnRequiresReminder, larsCode1, DateTime.UtcNow)
        ];
        _providerCourseForecastRepositoryMock
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(upToDateForecast);

        // Act
        await _sut.Run(new TimerInfo(), CancellationToken.None);

        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => models.Count() == 1)), Times.Once);
        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => VerifyProviderEmailModelsIncludesUkprns(models, ukprnRequiresReminder.ToString()))), Times.Once);
    }

    [Test]
    public async Task SendForecastsReminderEmailsFunction_ConvertProviderCourseToEmailModel()
    {
        const int ukprnRequiresReminder = 10012002;
        const string larsCode1 = "ZSC00001";

        // Arrange
        _providerCourseTypesReadRepositoryMock
            .Setup(r => r.GetAllProvidersWithShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync([ukprnRequiresReminder]);

        _providerCoursesReadRepositoryMock
            .Setup(r => r.GetAllShortCourses(It.IsAny<CancellationToken>()))
            .ReturnsAsync([new UkprnLarsCodeModel(ukprnRequiresReminder, larsCode1)]);

        _providerCourseForecastRepositoryMock
            .Setup(r => r.GetProviderCoursesWithRecentForecasts(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync([]);

        // Act
        await _sut.Run(new TimerInfo(), CancellationToken.None);

        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => models.Count() == 1)), Times.Once);
        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => VerifyModel(models.First(), ukprnRequiresReminder.ToString()))), Times.Once);
    }


    private bool VerifyModel(ProviderEmailModel model, string ukprn)
        => model.TemplateId == _forecastEmailConfiguration.ForecastPeriodicalReminderEmailTemplateId
        && model.Tokens[ProviderEmailTokens.Ukprn] == ukprn
        && model.Tokens[ProviderEmailTokens.CourseManagementWebUrl] == _forecastEmailConfiguration.CourseManagementWebUrl
        && model.Tokens[ProviderEmailTokens.ProviderAccountWebUrl] == _forecastEmailConfiguration.ProviderAccountsWebUrl;
}
