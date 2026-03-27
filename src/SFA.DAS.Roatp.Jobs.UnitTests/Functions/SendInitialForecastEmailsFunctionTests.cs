using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Functions;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

public class SendInitialForecastEmailsFunctionTests
{
    private Fixture _fixture;
    private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepositoryMock;
    private List<ProviderCourse> _courses = new();
    private Mock<ICourseManagementOuterApiClient> _courseManagementOuterApiClientMock;
    private Mock<IOptions<ForecastEmailConfiguration>> _forecastEmailConfigurationMock;
    private ForecastEmailConfiguration _forecastEmailConfiguration;

    [SetUp]
    public void Before_Each_Test()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _providerCoursesReadRepositoryMock = new Mock<IProviderCoursesReadRepository>();
        _courses = _fixture.CreateMany<ProviderCourse>().ToList();
        _providerCoursesReadRepositoryMock.Setup(r => r.GetShortCoursesAddedOnDate(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(_courses);

        _forecastEmailConfigurationMock = new Mock<IOptions<ForecastEmailConfiguration>>();
        _forecastEmailConfiguration = _fixture.Create<ForecastEmailConfiguration>();
        _forecastEmailConfigurationMock.Setup(m => m.Value).Returns(_forecastEmailConfiguration);

        _courseManagementOuterApiClientMock = new Mock<ICourseManagementOuterApiClient>();

        _courseManagementOuterApiClientMock.Setup(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>())).ReturnsAsync(true);
    }

    [Test]
    public async Task SendInitialForecastEmails_RetrievesProviderCourses()
    {
        var sut = new SendInitialForecastEmailsFuntion(Mock.Of<ILogger<SendInitialForecastEmailsFuntion>>(), _providerCoursesReadRepositoryMock.Object, _courseManagementOuterApiClientMock.Object, _forecastEmailConfigurationMock.Object);

        await sut.Run(new TimerInfo(), CancellationToken.None);

        _providerCoursesReadRepositoryMock.Verify(r => r.GetShortCoursesAddedOnDate(DateTime.UtcNow.AddDays(-1).Date, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public async Task SendInitialForecastEmails_CallsOuterApiForEachShortCourse()
    {
        var sut = new SendInitialForecastEmailsFuntion(Mock.Of<ILogger<SendInitialForecastEmailsFuntion>>(), _providerCoursesReadRepositoryMock.Object, _courseManagementOuterApiClientMock.Object, _forecastEmailConfigurationMock.Object);

        // Act
        await sut.Run(new TimerInfo(), CancellationToken.None);

        foreach (var course in _courses)
        {
            _courseManagementOuterApiClientMock.Verify(c => c.Post<ProviderEmailModel>($"providers/{course.Provider.Ukprn}/emails", It.Is<ProviderEmailModel>(m => VerifyModel(m, course))), Times.Once);
        }
    }

    [Test]
    public async Task Run_ProcessesCoursesInBatches_CallsPostForAllCourses()
    {
        var courses = _fixture.CreateMany<ProviderCourse>(25).ToList(); // 3 batches (10,10,5)
        _providerCoursesReadRepositoryMock.Setup(r => r.GetShortCoursesAddedOnDate(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(courses);

        var sut = new SendInitialForecastEmailsFuntion(Mock.Of<ILogger<SendInitialForecastEmailsFuntion>>(), _providerCoursesReadRepositoryMock.Object, _courseManagementOuterApiClientMock.Object, _forecastEmailConfigurationMock.Object);

        // Act
        await sut.Run(new TimerInfo(), CancellationToken.None);

        // Assert
        _courseManagementOuterApiClientMock.Verify(c => c.Post<ProviderEmailModel>(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()), Times.Exactly(courses.Count));
    }

    [Test]
    public async Task Run_ProcessesBatches_WithDelayBetweenBatches()
    {
        var courses = _fixture.CreateMany<ProviderCourse>(ForecastEmailConfiguration.BatchSize + 2).ToList(); // 2 batches (10,2)
        _providerCoursesReadRepositoryMock.Setup(r => r.GetShortCoursesAddedOnDate(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(courses);

        var postCallTimes = new List<DateTime>();
        _courseManagementOuterApiClientMock
            .Setup(c => c.Post<ProviderEmailModel>(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()))
            .Returns<string, ProviderEmailModel>((uri, model) =>
            {
                postCallTimes.Add(DateTime.UtcNow);
                return Task.FromResult(true);
            });

        var sut = new SendInitialForecastEmailsFuntion(Mock.Of<ILogger<SendInitialForecastEmailsFuntion>>(), _providerCoursesReadRepositoryMock.Object, _courseManagementOuterApiClientMock.Object, _forecastEmailConfigurationMock.Object);

        // Act
        var sw = System.Diagnostics.Stopwatch.StartNew();
        await sut.Run(new TimerInfo(), CancellationToken.None);
        sw.Stop();

        // Assert - ensure all posts happened
        _courseManagementOuterApiClientMock.Verify(c => c.Post(It.IsAny<string>(), It.IsAny<ProviderEmailModel>()), Times.Exactly(courses.Count));
        postCallTimes.Should().HaveCount((courses.Count));
        // There should be a delay between the last call of the first batch (index of batch size -1) and the first call of the second batch (index of batch size )
        var gap = (postCallTimes[ForecastEmailConfiguration.BatchSize] - postCallTimes[ForecastEmailConfiguration.BatchSize - 1]).TotalMilliseconds;
        Assert.That(gap, Is.GreaterThanOrEqualTo(ForecastEmailConfiguration.EmailThrottlingInSeconds), $"Expected gap >= 900ms between batches but was {gap}ms");
    }

    private bool VerifyModel(ProviderEmailModel model, ProviderCourse course)
        => model.TemplateId == _forecastEmailConfiguration.InitialForecastEmailTemplateId
        && model.Tokens.ContainsKey("larscode") && model.Tokens["larscode"] == course.Standard.LarsCode
        && model.Tokens.ContainsKey("coursename") && model.Tokens["coursename"] == course.Standard.Title
        && model.Tokens.ContainsKey("ukprn") && model.Tokens["ukprn"] == course.Provider.Ukprn.ToString()
        && model.Tokens.ContainsKey("providercmweb") && model.Tokens["providercmweb"] == _forecastEmailConfiguration.CourseManagementWebUrl
        && model.Tokens.ContainsKey("provideraccountsweb") && model.Tokens["provideraccountsweb"] == _forecastEmailConfiguration.ProviderAccountsWebUrl;
}
