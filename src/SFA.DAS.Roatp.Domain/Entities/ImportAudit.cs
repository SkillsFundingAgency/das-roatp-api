﻿using System;

namespace SFA.DAS.Roatp.Domain.Entities
{
    public class ImportAudit
    {
        public ImportAudit(DateTime timeStarted, int rowsImported, ImportType importType)
        {
            TimeStarted = timeStarted;
            RowsImported = rowsImported;
            TimeFinished = DateTime.UtcNow;
            ImportType = importType;
        }


        public int Id { get; set; }
        public DateTime TimeStarted { get; set; }
        public DateTime TimeFinished { get; set; }
        public int RowsImported { get; set; }
        public ImportType ImportType { get; set; }
    }

    public enum ImportType
    {
        CourseDirectory, 
        ProviderRegistrationDetails, 
        Standards
    }

    public static class ImportTypes
    {
        public static string CourseDirectory => ImportType.CourseDirectory.ToString();
        public static string ProviderRegistrationDetails => ImportType.ProviderRegistrationDetails.ToString();
        public static string Standards => ImportType.Standards.ToString();
    }
}
