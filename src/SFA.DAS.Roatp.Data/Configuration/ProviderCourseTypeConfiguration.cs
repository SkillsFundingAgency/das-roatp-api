using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderCourseTypeConfiguration : IEntityTypeConfiguration<ProviderCourseType>
{
    [ExcludeFromCodeCoverage]
    public void Configure(EntityTypeBuilder<ProviderCourseType> builder)
    {
        builder.ToTable(nameof(ProviderCourseType));
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Ukprn).IsRequired();
        builder.Property(p => p.CourseType).IsRequired();
        builder.Property(p => p.CourseType).HasMaxLength(50);
        builder.Property(p => p.LearningType).IsRequired();
        builder.Property(p => p.LearningType).HasMaxLength(20);
    }
}