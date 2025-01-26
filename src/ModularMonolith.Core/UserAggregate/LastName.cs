using System.Text.RegularExpressions;

namespace ModularMonolith.Users.Core.UserAggregate;

public readonly partial struct LastName : IEquatable<LastName>
{
    public readonly string Value { get; }

    private LastName(string value)
    {
        Value = value;
    }

    public LastName()
    {
        Value = string.Empty;
    }

    public static LastName From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length > 50)
        {
            throw new ArgumentException("Last name is too long", nameof(value));
        }

        if (value.Length < 2)
        {
            throw new ArgumentException("Last name is too short", nameof(value));
        }

        if (!LastNameRegex().IsMatch(value))
        {
            throw new ArgumentException("Last name must contain only letters", nameof(value));
        }

        return new(value);
    }

    public readonly bool Equals(LastName other)
    {
        return Value == other.Value;
    }

    public readonly bool Equals(string primitive) => Value == primitive;

    public override readonly bool Equals(object? obj) => obj is LastName other && Equals(other);

    public static bool operator ==(LastName left, LastName right) => left.Equals(right);

    public static bool operator !=(LastName left, LastName right) => !left.Equals(right);

    public static bool operator ==(LastName left, string right) => left.Equals(right);

    public static bool operator !=(LastName left, string right) => !left.Equals(right);

    public static bool operator ==(string left, LastName right) => right.Equals(left);

    public static bool operator !=(string left, LastName right) => !right.Equals(left);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value;

    [GeneratedRegex("^[a-zA-Z]+$")]
    private static partial Regex LastNameRegex();
}
