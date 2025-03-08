namespace ModularMonolith.Users.Core.UserAggregate;

public readonly struct IdentityId : IEquatable<IdentityId>
{
    public IdentityId()
    {
        Value = Guid.CreateVersion7();
    }

    private IdentityId(Guid value)
    {
        Value = value;
    }

    public readonly Guid Value { get; }

    public static implicit operator Guid(IdentityId identityId) => identityId.Value;

    public static bool operator ==(IdentityId left, IdentityId right) => left.Equals(right);

    public static bool operator !=(IdentityId left, IdentityId right) => !left.Equals(right);

    public static bool operator ==(IdentityId left, string right) => left.Equals(right);

    public static bool operator !=(IdentityId left, string right) => !left.Equals(right);

    public static bool operator ==(string left, IdentityId right) => right.Equals(left);

    public static bool operator !=(string left, IdentityId right) => !right.Equals(left);


    public static IdentityId From(Guid value)
    {
        if (value == Guid.Empty)
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

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value.ToString();
}
