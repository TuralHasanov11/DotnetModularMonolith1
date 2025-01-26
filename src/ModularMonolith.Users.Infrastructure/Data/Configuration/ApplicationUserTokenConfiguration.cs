using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularMonolith.Users.Core.UserAggregate;

namespace ModularMonolith.Users.Infrastructure.Data.Configuration;

public class ApplicationUserTokenConfiguration : IEntityTypeConfiguration<ApplicationUserToken>
{
    private const string TableName = "ApplicationUserTokens";

    public void Configure(EntityTypeBuilder<ApplicationUserToken> builder)
    {
        builder.ToTable(TableName);
    }
}
