using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace ModularMonolith.Users.Infrastructure.Data.Configuration;

public class AuditEntryConfiguration : IEntityTypeConfiguration<AuditEntry>
{
    public void Configure(EntityTypeBuilder<AuditEntry> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Metadata)
            .IsRequired();

        builder.Property(x => x.StartTimeUtc)
            .IsRequired();

        builder.Property(x => x.EndTimeUtc)
            .IsRequired();

        builder.Property(x => x.Succeeded)
            .IsRequired();

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(255)
            .IsRequired(false);

        builder.HasIndex(x => x.StartTimeUtc);
    }
}
