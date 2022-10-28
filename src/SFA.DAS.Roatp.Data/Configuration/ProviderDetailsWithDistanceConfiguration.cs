using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration
{
    [ExcludeFromCodeCoverage]
    public class ProviderDetailsWithDistanceConfiguration: IEntityTypeConfiguration<ProviderAndCourseDetailsWithDistance>
    {
        public void Configure(EntityTypeBuilder<ProviderAndCourseDetailsWithDistance> builder)
        {
            builder.HasNoKey();
        }
    }
}
