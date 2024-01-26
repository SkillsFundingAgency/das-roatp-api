using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using CsvHelper;

namespace SFA.DAS.Roatp.Jobs.Services;

public class DataExtractorService : IDataExtractorService
{
    public List<T> DeserializeCsvDataFromZipStream<T>(Stream stream, string fileName)
    {
        using (var zip = new ZipArchive(stream, ZipArchiveMode.Read, true))
        {
            var entry = zip.Entries.FirstOrDefault(m => m.Name.Equals(fileName, StringComparison.CurrentCultureIgnoreCase));

            if (entry == null) return new();

            using (var reader = new StreamReader(entry.Open()))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                return csv.GetRecords<T>().ToList();
            }
        }
    }
}
