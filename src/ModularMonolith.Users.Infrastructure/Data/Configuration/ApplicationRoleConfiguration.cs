using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularMonolith.Users.Core.RoleAggregate;
using ModularMonolith.Users.Core.UserAggregate;

namespace ModularMonolith.Users.Infrastructure.Data.Configuration;

public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    private const string TableName = "ApplicationRoles";

    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Id)
            .HasConversion(
                id => id.Value,
                value => IdentityId.From(value));

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.HasMany(r => r.RoleClaims)
            .WithOne(rc => rc.Role)
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();
    }
}
