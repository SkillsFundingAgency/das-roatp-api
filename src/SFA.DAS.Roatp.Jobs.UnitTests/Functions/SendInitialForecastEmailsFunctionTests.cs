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
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Configuration;
using SFA.DAS.Roatp.Jobs.Functions;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;

public class SendInitialForecastEmailsFunctionTests
{
    private Mock<IProviderCoursesReadRepository> _providerCoursesReadRepositoryMock;
    private List<ProviderCourse> _courses = [];
    private Mock<ICourseManagementOuterApiClient> _courseManagementOuterApiClientMock;
    private ForecastEmailConfiguration _forecastEmailConfiguration;

    [SetUp]
    public async Task Before_Each_Test()
    {
        Fixture fixture = new();
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        Mock<ILoggerFactory> loggerFactoryMock = new();
        loggerFactoryMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(Mock.Of<ILogger<SendInitialForecastEmailsFuntion>>());

        _providerCoursesReadRepositoryMock = new();
        _courses = fixture.CreateMany<ProviderCourse>().ToList();
        _providerCoursesReadRepositoryMock.Setup(r => r.GetShortCoursesAddedOnDate(It.IsAny<DateTime>(), It.IsAny<CancellationToken>())).ReturnsAsync(_courses);

        Mock<IOptions<ForecastEmailConfiguration>> forecastEmailConfigurationMock = new();
        _forecastEmailConfiguration = fixture.Create<ForecastEmailConfiguration>();
        forecastEmailConfigurationMock.Setup(m => m.Value).Returns(_forecastEmailConfiguration);

        _courseManagementOuterApiClientMock = new();

        SendInitialForecastEmailsFuntion sut = new(loggerFactoryMock.Object, _providerCoursesReadRepositoryMock.Object, _courseManagementOuterApiClientMock.Object, forecastEmailConfigurationMock.Object);

        await sut.Run(new TimerInfo(), default);
    }

    [Test]
    public void SendInitialForecastEmails_RetrievesProviderCourses()
    {
        _providerCoursesReadRepositoryMock.Verify(r => r.GetShortCoursesAddedOnDate(DateTime.UtcNow.AddDays(-1).Date, default), Times.Once);
    }

    [Test]
    public void SendInitialForecastEmails_CallsOuterApiForEachShortCourse()
    {
        foreach (var course in _courses)
        {
            _courseManagementOuterApiClientMock.Verify(c => c.Post<ProviderEmailModel, object>($"providers/{course.Provider.Ukprn}/email", It.Is<ProviderEmailModel>(m => VerifyModel(m, course))), Times.Once);
        }
    }

    private bool VerifyModel(ProviderEmailModel model, ProviderCourse course)
        => model.TemplateId == _forecastEmailConfiguration.InitialForecastEmailTemplateId
        && model.Tokens.ContainsKey("larscode") && model.Tokens["larscode"] == course.Standard.LarsCode
        && model.Tokens.ContainsKey("coursename") && model.Tokens["coursename"] == course.Standard.Title
        && model.Tokens.ContainsKey("ukprn") && model.Tokens["ukprn"] == course.Provider.Ukprn.ToString()
        && model.Tokens.ContainsKey("providercmweb") && model.Tokens["providercmweb"] == _forecastEmailConfiguration.CourseManagementWebUrl
        && model.Tokens.ContainsKey("provideraccountsweb") && model.Tokens["provideraccountsweb"] == _forecastEmailConfiguration.ProviderAccountsWebUrl;
}
