using System.Collections.Generic;
using System.IO;

namespace SFA.DAS.Roatp.Jobs.Services;

public interface IDataExtractorService
{
    List<T> DeserializeCsvDataFromZipStream<T>(Stream stream, string fileName);
}
