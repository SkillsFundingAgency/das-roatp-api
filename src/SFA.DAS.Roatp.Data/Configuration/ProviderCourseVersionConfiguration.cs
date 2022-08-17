using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderCourseVersionConfiguration : IEntityTypeConfiguration<ProviderCourseVersion>
    {
        public void Configure(EntityTypeBuilder<ProviderCourseVersion> builder)
        {
            builder.ToTable(nameof(ProviderCourseVersion));
            builder.HasKey(p => p.Id);
            builder.HasIndex(p => new { p.ProviderCourseId, p.StandardUId }).IsUnique();
        }
    }
}
