using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class AuditConfiguration : IEntityTypeConfiguration<Audit>
    {
        public void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.ToTable(nameof(Audit));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.EntityType).IsRequired();
            builder.Property(p => p.UserId).IsRequired();
            builder.Property(p => p.UserAction).IsRequired();
            builder.Property(p => p.AuditDate).IsRequired();
        }
    }
}