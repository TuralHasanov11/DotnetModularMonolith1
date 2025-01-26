namespace ModularMonolith.Users.Core.UserAggregate;

public readonly struct Email : IEquatable<Email>
{
    public readonly string Value { get; }

    private Email(string value)
    {
        Value = value;
    }

    public Email()
    {
        Value = string.Empty;
    }

    public static Email From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!value.Contains('@'))
        {
            throw new ArgumentException("Invalid email address", nameof(value));
        }

        return new(value);
    }

    public readonly bool Equals(Email other)
    {
        return Value == other.Value;
    }

    public readonly bool Equals(string primitive) => Value == primitive;

    public override readonly bool Equals(object? obj) => obj is Email other && Equals(other);

    public static bool operator ==(Email left, Email right) => left.Equals(right);

    public static bool operator !=(Email left, Email right) => !left.Equals(right);

    public static bool operator ==(Email left, string right) => left.Equals(right);

    public static bool operator !=(Email left, string right) => !left.Equals(right);

    public static bool operator ==(string left, Email right) => right.Equals(left);

    public static bool operator !=(string left, Email right) => !right.Equals(left);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}
