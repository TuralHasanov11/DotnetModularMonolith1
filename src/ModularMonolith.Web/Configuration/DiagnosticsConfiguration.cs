using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ModularMonolith.Web.Configuration;

public static class DiagnosticsConfiguration
{
    public const string ServiceName = "modular-monolith-otel";

    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> UsersCount = Meter.CreateCounter<long>("users.count");
    public static readonly Counter<long> UsersLoginCount = Meter.CreateCounter<long>("users.login_count");

    public static readonly ActivitySource Source = new(ServiceName);

    public static void EnrichWithUser(this Activity activity, ClaimsPrincipal user)
    {
        activity.SetTag("user.id", user.FindFirstValue(JwtRegisteredClaimNames.Sub));
        activity.SetTag("user.email", user.FindFirstValue(JwtRegisteredClaimNames.Email));
        activity.SetTag("user.name", user.FindFirstValue(JwtRegisteredClaimNames.Name));
    }

    public static Activity? StartActivityWithTags(
        this ActivitySource source,
        string name,
        IReadOnlyCollection<KeyValuePair<string, object?>> tags)
    {
        return source.StartActivity(
            name,
            ActivityKind.Internal,
            Activity.Current?.Context ?? default,
            tags);
    }

    public static class Names
    {
        public const string UserId = "user_id";
    }
}
