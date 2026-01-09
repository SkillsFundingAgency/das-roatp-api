using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class StandardConfiguration : IEntityTypeConfiguration<Standard>
{
    public void Configure(EntityTypeBuilder<Standard> builder)
    {
        builder.ToTable(nameof(Standard));
        builder.HasKey(p => p.StandardUId);
        builder.Property(p => p.LarsCode).IsRequired();
        builder.Property(p => p.IfateReferenceNumber).IsRequired().HasMaxLength(10);
        builder.Property(p => p.Level).IsRequired();
        builder.Property(p => p.Title).IsRequired().HasMaxLength(1000);
        builder.Property(p => p.Version).IsRequired();
        builder.Property(e => e.CourseType).HasConversion(
            v => v.ToString(),
            v => (CourseType)Enum.Parse(typeof(CourseType), v));
        builder.Property(e => e.ApprenticeshipType).HasConversion(
            v => v.ToString(),
            v => (ApprenticeshipType)Enum.Parse(typeof(ApprenticeshipType), v));
    }
}