using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services;

public class ImportAnnualFeedbackSummariesServiceTests
{
    private ImportAnnualFeedbackSummariesService _sut;
    private Mock<IDateTimeProvider> _dateTimeProviderMock;
    private Mock<IProviderEmployerStarsReadRepository> _employerStarsReadRepositoryMock;
    private Mock<ICourseManagementOuterApiClient> _courseManagementOuterApiClientMock;

    [SetUp]
    public void Setup()
    {
        _dateTimeProviderMock = new();
        _employerStarsReadRepositoryMock = new();
        _courseManagementOuterApiClientMock = new();
        _sut = new ImportAnnualFeedbackSummariesService(
            Mock.Of<ILogger<ImportAnnualFeedbackSummariesService>>(),
            _dateTimeProviderMock.Object,
            _employerStarsReadRepositoryMock.Object,
            _courseManagementOuterApiClientMock.Object);
    }

    [Test]
    public void GetTimePeriod_ReturnsLastAcademicYear()
    {
        _dateTimeProviderMock.Setup(dp => dp.UtcNow).Returns(new DateTime(2025, 8, 1, 0, 0, 0, DateTimeKind.Utc));
        _sut.GetTimePeriod().Should().Be("AY2425");
    }

    [Test, AutoData]
    public async Task CheckIfDataExists_ReturnsTrue_WhenDataExists(string timePeriod)
    {
        // Arrange
        _employerStarsReadRepositoryMock.Setup(repo => repo.GetTimePeriods()).ReturnsAsync([timePeriod]);

        // Act
        var result = await _sut.CheckIfDataExists(timePeriod);

        // Assert
        result.Should().BeTrue();
    }

    [Test, AutoData]
    public async Task CheckIfDataExists_ReturnsFalse_WhenDataDoesNotExist(string timePeriod)
    {
        // Arrange
        _employerStarsReadRepositoryMock.Setup(repo => repo.GetTimePeriods()).ReturnsAsync([]);
        // Act
        var result = await _sut.CheckIfDataExists(timePeriod);
        // Assert
        result.Should().BeFalse();
    }

    [Test, AutoData]
    public async Task GetFeedbackSummaries_GetsDataFromOuterApi_ReturnsEntities(string timePeriod, AnnualSummariesFeedbackResponse expectedResult)
    {
        // Arrange
        var expectedUrl = $"feedback/lookup/annual-summarised?academicYear={timePeriod}";
        _courseManagementOuterApiClientMock
            .Setup(client => client.Get<AnnualSummariesFeedbackResponse>(expectedUrl))
            .ReturnsAsync((true, expectedResult));
        // Act
        var (apprenticeStarts, employerStars) = await _sut.GetFeedbackSummaries(timePeriod);
        // Assert
        _courseManagementOuterApiClientMock.Verify(client => client.Get<AnnualSummariesFeedbackResponse>(expectedUrl), Times.Once);
        apprenticeStarts.Should().BeEquivalentTo(expectedResult.ApprenticesFeedback);
        employerStars.Should().BeEquivalentTo(expectedResult.EmployersFeedback);
        apprenticeStarts.Should().OnlyContain(x => x.TimePeriod == timePeriod);
        employerStars.Should().OnlyContain(x => x.TimePeriod == timePeriod);
    }

    [Test, AutoData]
    public async Task GetFeedbackSummaries_ThrowsException_WhenOuterApiCallFails(string timePeriod)
    {
        // Arrange
        _courseManagementOuterApiClientMock
            .Setup(client => client.Get<AnnualSummariesFeedbackResponse>(timePeriod))
            .ReturnsAsync((false, null));

        // Act
        Func<Task> act = async () => await _sut.GetFeedbackSummaries(timePeriod);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Failed to retrieve annual feedback summaries for time period {timePeriod}");
    }
}
