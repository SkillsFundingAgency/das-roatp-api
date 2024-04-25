using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.Roatp.Jobs.Models;
using SFA.DAS.Roatp.Jobs.Services;
using SFA.DAS.Roatp.Jobs.UnitTests.Functions.ImportAchievementRatesFunctionTests;

namespace SFA.DAS.Roatp.Jobs.UnitTests.Services;

public class DataExtractorServiceTest
{
    const string OverallFileName = "app-narts-subject-and-level-detailed.csv";
    [Test]
    public void ThenExtractsAndDeserializesCsvDataFromZipFile()
    {
        DataExtractorService sut = new();
        List<OverallAchievementRateCsvModel> expected = AchievementRatesTestDataHelper.GetAllOverallRatingsRawData();
        using var zipStream = new MemoryStream();
        AddToZip(expected, OverallFileName, zipStream);

        //Act
        var actual = sut.DeserializeCsvDataFromZipStream<OverallAchievementRateCsvModel>(zipStream, OverallFileName);

        //Assert
        actual.Should().BeEquivalentTo(expected);
    }

    private static void AddToZip<T>(IEnumerable<T> data, string entryFileName, Stream zipStream)
    {
        using var archive = new ZipArchive(zipStream, ZipArchiveMode.Update, true);
        // Add a file entry to the zip archive
        var zipEntry = archive.CreateEntry(entryFileName);

        using var entryStream = zipEntry.Open();
        using var writer = new StreamWriter(entryStream, Encoding.UTF8, 1024, true);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        // Write records to the entry stream
        csv.WriteRecords(data);
    }
}
