using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularMonolith.Users.Core.UserAggregate;

namespace ModularMonolith.Users.Infrastructure.Data.Configuration;

public class ApplicationUserRoleConfiguration : IEntityTypeConfiguration<ApplicationUserRole>
{
    private const string TableName = "ApplicationUserRoles";

    public void Configure(EntityTypeBuilder<ApplicationUserRole> builder)
    {
        builder.ToTable(TableName);
    }
}
