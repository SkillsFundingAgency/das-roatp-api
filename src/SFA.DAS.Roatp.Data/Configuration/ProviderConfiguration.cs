using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.ToTable(nameof(Provider));
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Ukprn).IsRequired();
            builder.Property(p => p.LegalName).IsRequired().HasMaxLength(1000);
            builder.Property(p => p.TradingName).HasMaxLength(1000);
            builder.Property(p => p.Email).HasMaxLength(300);
            builder.Property(p => p.Phone).HasMaxLength(50);
            builder.Property(p => p.Website).HasMaxLength(500);
            builder.Property(p => p.EmployerSatisfaction).HasColumnType("decimal");
            builder.Property(p => p.LearnerSatisfaction).HasColumnType("decimal");
            builder.Property(p => p.IsImported).IsRequired();

            builder.HasMany(p => p.Locations)
                .WithOne(p => p.Provider)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(p => p.ProviderId);

            builder.HasMany(p => p.Courses)
                .WithOne(c => c.Provider)
                .HasPrincipalKey(p => p.Id)
                .HasForeignKey(p => p.ProviderId);

            builder.HasMany(p => p.NationalAchievementRates)
               .WithOne(c => c.Provider)
               .HasPrincipalKey(p => p.Id)
               .HasForeignKey(p => p.ProviderId);
        }
    }
}
