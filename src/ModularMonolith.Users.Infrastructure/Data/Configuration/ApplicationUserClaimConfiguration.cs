using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ModularMonolith.Users.Core.UserAggregate;

namespace ModularMonolith.Users.Infrastructure.Data.Configuration;

public class ApplicationUserClaimConfiguration : IEntityTypeConfiguration<ApplicationUserClaim>
{
    private const string TableName = "ApplicationUserClaims";

    public void Configure(EntityTypeBuilder<ApplicationUserClaim> builder)
    {
        builder.ToTable(TableName);
    }
}
