using System;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Interfaces;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions;
public class ImportAnnualFeedbackSummariesFunctionTests
{
    private ImportAnnualFeedbackSummariesFunction _sut;
    private Mock<IImportAnnualFeedbackSummariesService> _serviceMock;
    private Mock<IImportFeedbackSummariesRepository> _repositoryMock;

    [SetUp]
    public void Setup()
    {
        _repositoryMock = new();
        _serviceMock = new();
        _serviceMock.Setup(s => s.CheckIfDataExists(It.IsAny<string>())).ReturnsAsync(false);

        // Initialize the ImportAnnualFeedbackSummariesFunction instance
        _sut = new ImportAnnualFeedbackSummariesFunction(Mock.Of<ILogger<ImportAnnualFeedbackSummariesFunction>>(), _serviceMock.Object, _repositoryMock.Object);
    }

    [Test]
    public async Task ThenGetsTimePeriod()
    {
        await _sut.Run(new());

        _serviceMock.Verify(s => s.GetTimePeriod(), Times.Once);
    }

    [Test]
    public async Task ThenExitFunctionWhenDataAlreadyExists()
    {
        _serviceMock.Setup(s => s.CheckIfDataExists(It.IsAny<string>())).ReturnsAsync(true);

        await _sut.Run(new());

        _serviceMock.Verify(s => s.GetFeedbackSummaries(It.IsAny<string>()), Times.Never, "Import should not be called if data already exists.");
    }

    [Test, AutoData]
    public async Task ThenCallsGetFeedbackSummaries(string timePeriod)
    {
        _serviceMock.Setup(s => s.GetTimePeriod()).Returns(timePeriod);
        _serviceMock.Setup(s => s.CheckIfDataExists(timePeriod)).ReturnsAsync(false);
        await _sut.Run(new());
        _serviceMock.Verify(s => s.GetFeedbackSummaries(timePeriod), Times.Once, "GetFeedbackSummaries should be called with the correct time period.");
    }

    [Test, AutoData]
    public async Task ThenCallsImport(string timePeriod, ProviderApprenticeStars[] apprenticeStars, ProviderEmployerStars[] employerStars)
    {
        _serviceMock.Setup(s => s.GetTimePeriod()).Returns(timePeriod);
        _serviceMock.Setup(s => s.CheckIfDataExists(timePeriod)).ReturnsAsync(false);
        _serviceMock.Setup(s => s.GetFeedbackSummaries(timePeriod)).ReturnsAsync((apprenticeStars, employerStars));
        await _sut.Run(new());
        _repositoryMock.Verify(s => s.Import(It.IsAny<DateTime>(), apprenticeStars, employerStars), Times.Once, "Import should be called after getting feedback summaries.");
    }
}
