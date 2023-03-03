using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderRegistrationDetailConfiguration : IEntityTypeConfiguration<ProviderRegistrationDetail>
    {
        public void Configure(EntityTypeBuilder<ProviderRegistrationDetail> builder)
        {
            builder.ToTable(nameof(ProviderRegistrationDetail));
            builder.HasKey(r => r.Ukprn);
            builder.HasOne(r => r.Provider).WithOne(p => p.ProviderRegistrationDetail).HasForeignKey<Provider>(p => p.Ukprn);
        }
    }
}
