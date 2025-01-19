namespace ModularMonolith.Core.RoleAggregate;

public readonly struct RoleName : IEquatable<RoleName>
{
    public readonly string Value { get; }

    private RoleName(string value)
    {
        Value = value;
    }

    public RoleName()
    {
        Value = string.Empty;
    }

    public static RoleName From(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Role name cannot be null or white space.", nameof(value));
        }

        return new(value);
    }

    public readonly bool Equals(RoleName other)
    {
        return Value == other.Value;
    }

    public readonly bool Equals(string primitive) => Value == primitive;

    public override readonly bool Equals(object? obj) => obj is RoleName other && Equals(other);

    public static bool operator ==(RoleName left, RoleName right) => left.Equals(right);

    public static bool operator !=(RoleName left, RoleName right) => !left.Equals(right);

    public static bool operator ==(RoleName left, string right) => left.Equals(right);

    public static bool operator !=(RoleName left, string right) => !left.Equals(right);

    public static bool operator ==(string left, RoleName right) => right.Equals(left);

    public static bool operator !=(string left, RoleName right) => !right.Equals(left);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value;
}
