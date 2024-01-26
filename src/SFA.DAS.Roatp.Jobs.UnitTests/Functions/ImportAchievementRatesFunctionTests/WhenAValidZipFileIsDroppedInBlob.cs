using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.ApiClients;
using SFA.DAS.Roatp.Jobs.ApiModels.Lookup;
using SFA.DAS.Roatp.Jobs.Functions;
using SFA.DAS.Roatp.Jobs.Models;
using SFA.DAS.Roatp.Jobs.Services;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Functions.ImportAchievementRatesFunctionTests;

public class WhenAValidZipFileIsDroppedInBlob
{
    private Mock<IDataExtractorService> _dataExtractorServiceMock;
    private Mock<ICourseManagementOuterApiClient> _coursesApiClientMock;
    private Mock<IImportNationalAchievementRateOverallService> _importAchievementRateOverallServiceMock;
    private Mock<IImportNationalAchievementRateService> _importNationalAchievementRateServiceMock;
    private List<SectorSubjectAreaTier1Model> Ssa1s;

    [SetUp]
    public async Task SetUp()
    {
        //Arrange
        Fixture fixture = new();

        Ssa1s = fixture.CreateMany<SectorSubjectAreaTier1Model>().ToList();
        _dataExtractorServiceMock = new();
        _coursesApiClientMock = new();
        _importAchievementRateOverallServiceMock = new();
        _importNationalAchievementRateServiceMock = new();

        _coursesApiClientMock.Setup(c => c.Get<GetAllSectorSubjectAreaTier1Response>(It.IsAny<string>())).ReturnsAsync((true, new GetAllSectorSubjectAreaTier1Response(Ssa1s)));

        _dataExtractorServiceMock.Setup(d => d.DeserializeCsvDataFromZipStream<OverallAchievementRateCsvModel>(It.IsAny<Stream>(), ImportAchievementRatesFunction.OverallRatingsImportFileName)).Returns(AchievementRatesTestDataHelper.GetAllOverallRatingsRawData());

        _dataExtractorServiceMock.Setup(d => d.DeserializeCsvDataFromZipStream<ProviderAchievementRateCsvModel>(It.IsAny<Stream>(), ImportAchievementRatesFunction.ProviderRatingsImportFileName)).Returns(AchievementRatesTestDataHelper.GetAllProviderRatingsRawData());

        ImportAchievementRatesFunction sut = new(_dataExtractorServiceMock.Object, _coursesApiClientMock.Object, _importAchievementRateOverallServiceMock.Object, _importNationalAchievementRateServiceMock.Object);

        using var zipStream = new MemoryStream();

        //Act
        await sut.Run(zipStream, "filename.zip", Mock.Of<ILogger>());
    }

    [Test]
    public void ThenGetsSectorSubjectAreaTier1LookupData()
        => _coursesApiClientMock.Verify(c => c.Get<GetAllSectorSubjectAreaTier1Response>(It.IsAny<string>()), Times.Once());

    [Test]
    public void ThenExtractsOverallRatingsDataFromBlob()
        => _dataExtractorServiceMock.Verify(d => d.DeserializeCsvDataFromZipStream<OverallAchievementRateCsvModel>(It.IsAny<Stream>(), ImportAchievementRatesFunction.OverallRatingsImportFileName), Times.Once);

    [Test]
    public void ThenExtractsProviderRatingsDataFromBlob()
        => _dataExtractorServiceMock.Verify(d => d.DeserializeCsvDataFromZipStream<ProviderAchievementRateCsvModel>(It.IsAny<Stream>(), ImportAchievementRatesFunction.ProviderRatingsImportFileName), Times.Once);

    [Test]
    public void ThenSendsFilteredOverallRatingsDataForImport()
        => _importAchievementRateOverallServiceMock.Verify(i => i.ImportData(It.Is<IEnumerable<OverallAchievementRateCsvModel>>(l => l.All(x => x.SectorSubjectAreaTier1Desc == AchievementRatesTestDataHelper.Ssa1WithValidData)), Ssa1s), Times.Once);

    [Test]
    public void ThenSendsFilteredProviderRatingsDataForImport()
        => _importNationalAchievementRateServiceMock.Verify(i => i.ImportData(It.Is<IEnumerable<ProviderAchievementRateCsvModel>>(l => l.All(x => x.SectorSubjectAreaTier1Desc == AchievementRatesTestDataHelper.Ssa1WithValidData)), Ssa1s), Times.Once);
}
