using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

[ExcludeFromCodeCoverage]
public class ProviderLocationDetailsWithDistanceConfiguration : IEntityTypeConfiguration<ProviderLocationDetailsWithDistance>
{
    public void Configure(EntityTypeBuilder<ProviderLocationDetailsWithDistance> builder)
    {
        builder.HasNoKey();
    }
}