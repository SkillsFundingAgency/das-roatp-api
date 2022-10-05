﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class NationalAchievementRateImportConfiguration : IEntityTypeConfiguration<NationalAchievementRateImport>
    {
        public void Configure(EntityTypeBuilder<NationalAchievementRateImport> builder)
        {
            builder.ToTable(nameof(NationalAchievementRateImport));
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(x => x.Ukprn).IsRequired(); 
            builder.Property(x => x.Age).IsRequired().HasConversion<int>(); 
            builder.Property(x => x.SectorSubjectArea).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.ApprenticeshipLevel).IsRequired().HasConversion<int>(); 
            builder.Property(x => x.OverallCohort).IsRequired(false);
            builder.Property(x => x.OverallAchievementRate).IsRequired(false);
        }
    }
}