using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ModularMonolith.Web.Configuration;

public static class DiagnosticsConfiguration
{
    public const string ServiceName = "modular-monolith-otel";

    public static readonly Meter Meter = new(ServiceName);

    public static readonly Counter<long> UsersCount = Meter.CreateCounter<long>("users.count");
    public static readonly Counter<long> UsersLoginCount = Meter.CreateCounter<long>("users.login_count");

    public static readonly ActivitySource Source = new(ServiceName);

    public static class Names
    {
        public const string UserId = "user_id";
    }
}
