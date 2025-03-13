using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace ModularMonolith.Web.Metrics;

public static class DiagnosticsConfiguration
{
    public const string ServiceName = "modular-monolith-otel";

    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> UsersCount = Meter.CreateCounter<long>("users.count");
    public static readonly Counter<long> UsersLoginCount = Meter.CreateCounter<long>("users.login_count");

    public static readonly Counter<long> PageSuccessCounter
        = Meter.CreateCounter<long>("page.success_count", description: "The number of page success execution");

    public static readonly Counter<long> PageFailCounter
        = Meter.CreateCounter<long>("page.fail_count", description: "The number of page fail execution");

    public static readonly Histogram<long> PageLatencyHistogram
        = Meter.CreateHistogram<long>("page.latency", "ms", "Page Latency");

    public static readonly Gauge<long> PageDuration = new(Meter, "page.duration", "ms", "Page Duration");

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
