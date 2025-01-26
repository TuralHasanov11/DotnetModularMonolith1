using System.Text.RegularExpressions;

namespace ModularMonolith.Users.Core.UserAggregate;

public readonly partial struct UserName : IEquatable<UserName>
{
    public readonly string Value { get; }

    private UserName(string value)
    {
        Value = value;
    }
    public UserName()
    {
        Value = string.Empty;
    }

    public static UserName From(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (value.Length < 3)
        {
            throw new ArgumentException("UserName must be at least 3 characters long", nameof(value));
        }

        if (value.Length > 20)
        {
            throw new ArgumentException("UserName must be at most 20 characters long", nameof(value));
        }

        if (!UserNameRegex().IsMatch(value))
        {
            throw new ArgumentException("UserName must contain only letters and numbers", nameof(value));
        }

        return new(value);
    }

    public readonly bool Equals(UserName other)
    {
        return Value == other.Value;
    }

    public readonly bool Equals(string primitive) => Value == primitive;

    public override readonly bool Equals(object? obj) => obj is UserName other && Equals(other);

    public static bool operator ==(UserName left, UserName right) => left.Equals(right);

    public static bool operator !=(UserName left, UserName right) => !left.Equals(right);

    public static bool operator ==(UserName left, string right) => left.Equals(right);

    public static bool operator !=(UserName left, string right) => !left.Equals(right);

    public static bool operator ==(string left, UserName right) => right.Equals(left);

    public static bool operator !=(string left, UserName right) => !right.Equals(left);

    public override readonly int GetHashCode() => Value.GetHashCode();

    public override readonly string ToString() => Value;

    public static implicit operator string(UserName userName) => userName.Value;

    [GeneratedRegex(@"^[a-zA-Z0-9]+$")]
    private static partial Regex UserNameRegex();
}
