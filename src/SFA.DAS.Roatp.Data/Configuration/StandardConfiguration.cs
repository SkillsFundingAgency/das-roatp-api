using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
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
        }
    }
}
