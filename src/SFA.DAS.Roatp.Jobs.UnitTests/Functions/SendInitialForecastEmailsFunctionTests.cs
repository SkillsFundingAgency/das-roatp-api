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
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

public class SendInitialForecastEmailsFunctionTests
{
    private Fixture _fixture;
    private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepositoryMock;
    private Mock<IProviderEmailProcessingService> _providerEmailProcessingServiceMock;
    private ForecastEmailConfiguration _forecastEmailConfiguration;

    [SetUp]
    public void Before_Each_Test()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();

        _forecastEmailConfiguration = _fixture.Create<ForecastEmailConfiguration>();

        _providerEmailProcessingServiceMock = new();
    }

    [Test]
    public async Task SendInitialForecastEmails_RetrievesProviderCourses()
    {
        var courses = _fixture.CreateMany<ProviderCourse>().ToList();
        _providerCoursesReadRepositoryMock.Setup(r => r.GetShortCoursesAddedOnDate(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(courses);

        var sut = new SendInitialForecastEmailsFunction(Mock.Of<ILogger<SendInitialForecastEmailsFunction>>(), _providerCoursesReadRepositoryMock.Object, _providerEmailProcessingServiceMock.Object, Options.Create(_forecastEmailConfiguration));

        await sut.Run(new TimerInfo(), CancellationToken.None);

        _providerCoursesReadRepositoryMock.Verify(r => r.GetShortCoursesAddedOnDate(DateTime.UtcNow.AddDays(-1).Date, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendInitialForecastEmails_ConvertsCourseToEmailModel()
    {
        var course = _fixture.Create<ProviderCourse>();
        _providerCoursesReadRepositoryMock.Setup(r => r.GetShortCoursesAddedOnDate(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync([course]);

        var sut = new SendInitialForecastEmailsFunction(Mock.Of<ILogger<SendInitialForecastEmailsFunction>>(), _providerCoursesReadRepositoryMock.Object, _providerEmailProcessingServiceMock.Object, Options.Create(_forecastEmailConfiguration));

        // Act
        await sut.Run(new TimerInfo(), CancellationToken.None);

        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => models.Count() == 1)), Times.Once);
        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => VerifyModel(models.First(), course))), Times.Once);
    }

    [Test]
    public async Task Run_ProcessesCoursesInBatches_CallsPostForAllCourses()
    {
        var courses = _fixture.CreateMany<ProviderCourse>(25).ToList(); // 3 batches (10,10,5)
        _providerCoursesReadRepositoryMock.Setup(r => r.GetShortCoursesAddedOnDate(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(courses);

        var sut = new SendInitialForecastEmailsFunction(Mock.Of<ILogger<SendInitialForecastEmailsFunction>>(), _providerCoursesReadRepositoryMock.Object, _providerEmailProcessingServiceMock.Object, Options.Create(_forecastEmailConfiguration));

        // Act
        await sut.Run(new TimerInfo(), CancellationToken.None);

        // Assert
        _providerEmailProcessingServiceMock.Verify(c => c.SendEmailsInBatches(It.Is<IEnumerable<ProviderEmailModel>>(models => models.Count() == courses.Count)), Times.Once);
    }

    private bool VerifyModel(ProviderEmailModel model, ProviderCourse course)
        => model.TemplateId == _forecastEmailConfiguration.InitialForecastEmailTemplateId
        && model.Tokens[ProviderEmailTokens.LarsCode] == course.Standard.LarsCode
        && model.Tokens[ProviderEmailTokens.CourseName] == course.Standard.Title
        && model.Tokens[ProviderEmailTokens.Ukprn] == course.Provider.Ukprn.ToString()
        && model.Tokens[ProviderEmailTokens.CourseManagementWebUrl] == _forecastEmailConfiguration.CourseManagementWebUrl
        && model.Tokens[ProviderEmailTokens.ProviderAccountWebUrl] == _forecastEmailConfiguration.ProviderAccountsWebUrl;
}
