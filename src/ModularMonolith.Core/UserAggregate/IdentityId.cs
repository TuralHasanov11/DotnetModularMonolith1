namespace ModularMonolith.Core.UserAggregate;

public readonly struct IdentityId : IEquatable<IdentityId>
{
    public readonly Guid Value { get; }

    public IdentityId()
    {
        Value = Guid.NewGuid();
    }

    private IdentityId(Guid value)
    {
        Value = value;
    }

    public static IdentityId From(Guid value)
    {
        if (Guid.Empty == value)
        {
            throw new ArgumentException("IdentityId cannot be empty", nameof(value));
        }

        return new(value);
    }

    public readonly bool Equals(IdentityId other)
    {
        return Value == other.Value;
    }

    public readonly bool Equals(Guid primitive) => Value == primitive;

    public override readonly bool Equals(object? obj) => obj is IdentityId other && Equals(other);

    public static bool operator ==(IdentityId left, IdentityId right) => left.Equals(right);

    public static bool operator !=(IdentityId left, IdentityId right) => !left.Equals(right);

    public static bool operator ==(IdentityId left, string right) => left.Equals(right);

    public static bool operator !=(IdentityId left, string right) => !left.Equals(right);

    public static bool operator ==(string left, IdentityId right) => right.Equals(left);

    public static bool operator !=(string left, IdentityId right) => !right.Equals(left);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value.ToString();

    public static implicit operator Guid(IdentityId identityId) => identityId.Value;
}