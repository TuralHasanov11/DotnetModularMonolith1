namespace ModularMonolith.Users.Core.UserAggregate;

public readonly struct FullName : IEquatable<FullName>
{
    public readonly string Value { get; }

    private FullName(FirstName firstName, LastName lastName)
    {
        Value = $"{firstName} {lastName}";
    }

    public static FullName From(FirstName firstName, LastName lastName)
    {
        return new(firstName, lastName);
    }

    public readonly bool Equals(FullName other)
    {
        return Value == other.Value;
    }

    public readonly bool Equals(string primitive) => Value == primitive;

    public override readonly bool Equals(object? obj) => obj is FullName other && Equals(other);

    public static bool operator ==(FullName left, FullName right) => left.Equals(right);

    public static bool operator !=(FullName left, FullName right) => !left.Equals(right);

    public static bool operator ==(FullName left, string right) => left.Equals(right);

    public static bool operator !=(FullName left, string right) => !left.Equals(right);

    public static bool operator ==(string left, FullName right) => right.Equals(left);

    public static bool operator !=(string left, FullName right) => !right.Equals(left);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value;

    public static implicit operator string(FullName fullName) => fullName.Value;
}