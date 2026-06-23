using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Roatp.Domain.Entities;

namespace SFA.DAS.Roatp.Data.Configuration;

public class RestrictedCourseConfiguration : IEntityTypeConfiguration<RestrictedCourse>
{
    public void Configure(EntityTypeBuilder<RestrictedCourse> builder)
    {
        builder.ToTable(nameof(RestrictedCourse));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LarsCode).IsRequired();
        builder.Property(x => x.CreatedDate).IsRequired();
    }
}
