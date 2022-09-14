using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ImportAuditConfiguration : IEntityTypeConfiguration<ImportAudit>
    {
        public void Configure(EntityTypeBuilder<ImportAudit> builder)
        {
            builder.ToTable(nameof(ImportAudit));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.ImportType).IsRequired();
            builder.Property(p => p.RowsImported).IsRequired();
            builder.Property(p => p.TimeStarted).IsRequired();
            builder.Property(p => p.TimeFinished).IsRequired();
            builder.Property(e => e.ImportType)
                .HasConversion(
                e => e.ToString(),
                e => (ImportType)Enum.Parse(typeof(ImportType), e));
        }
    }
}