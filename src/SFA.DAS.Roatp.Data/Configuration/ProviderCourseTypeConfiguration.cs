using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using SFA.DAS.Roatp.Domain.Models;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderCourseTypeConfiguration : IEntityTypeConfiguration<ProviderCourseType>
{
    public void Configure(EntityTypeBuilder<ProviderCourseType> builder)
    {
        builder.ToTable(nameof(ProviderCourseType));
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Ukprn).IsRequired();
        builder.Property(p => p.CourseType).IsRequired().HasMaxLength(50)
            .HasConversion(
                v => v.ToString(),
                v => (CourseType)Enum.Parse(typeof(CourseType), v)
            );
    }
}