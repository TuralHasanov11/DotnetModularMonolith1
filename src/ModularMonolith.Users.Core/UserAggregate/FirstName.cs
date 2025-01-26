using System.Text.RegularExpressions;

namespace ModularMonolith.Users.Core.UserAggregate;

public readonly partial struct FirstName : IEquatable<FirstName>
{
    public readonly string Value { get; }

    private FirstName(string value)
    {
        Value = value;
    }

    public FirstName()
    {
        Value = string.Empty;
    }

    public static FirstName From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > 50)
        {
            throw new ArgumentException("First name is too long", nameof(value));
        }

        if (value.Length < 2)
        {
            throw new ArgumentException("First name is too short", nameof(value));
        }

        if (!FirstNameRegex().IsMatch(value))
        {
            throw new ArgumentException("First name must contain only letters", nameof(value));
        }

        return new(value);
    }

    public readonly bool Equals(FirstName other)
    {
        return Value == other.Value;
    }

    public readonly bool Equals(string primitive) => Value == primitive;

    public override readonly bool Equals(object? obj) => obj is FirstName other && Equals(other);

    public static bool operator ==(FirstName left, FirstName right) => left.Equals(right);

    public static bool operator !=(FirstName left, FirstName right) => !left.Equals(right);

    public static bool operator ==(FirstName left, string right) => left.Equals(right);

    public static bool operator !=(FirstName left, string right) => !left.Equals(right);

    public static bool operator ==(string left, FirstName right) => right.Equals(left);

    public static bool operator !=(string left, FirstName right) => !right.Equals(left);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value;

    [GeneratedRegex("^[a-zA-Z]+$")]
    private static partial Regex FirstNameRegex();

    public static implicit operator string(FirstName firstName) => firstName.Value;
}
