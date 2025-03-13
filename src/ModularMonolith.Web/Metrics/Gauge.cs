using System.Diagnostics.Metrics;
using System.Runtime.InteropServices;

namespace ModularMonolith.Web.Metrics;

public class Gauge<T> where T : struct
{
    private readonly Lock _lock = new();
    private readonly Dictionary<string, Measurement<T>> _measurements = new();

    public Gauge(Meter meter, string name, string? unit = null, string? description = null)
    {
        meter.CreateObservableGauge(name, () => _measurements.Values, unit, description);
    }

    public void SetValue(T value, KeyValuePair<string, object?>? tag = null)
    {
        lock (_lock)
        {
            var key = tag.ToString() ?? string.Empty;
            var tags = tag is null ? [] : new[] { tag.Value };

            ref Measurement<T> measurement = ref CollectionsMarshal.GetValueRefOrAddDefault(_measurements, key, out bool _);
            measurement = new(value, tags);
        }
    }
}
