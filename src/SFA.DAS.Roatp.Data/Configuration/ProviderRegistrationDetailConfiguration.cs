using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderRegistrationDetailConfiguration : IEntityTypeConfiguration<ProviderRegistrationDetail>
    {
        public void Configure(EntityTypeBuilder<ProviderRegistrationDetail> builder)
        {
            builder.ToTable(nameof(ProviderRegistrationDetail));
            builder.HasKey(p => p.Ukprn);
        }
    }
}
