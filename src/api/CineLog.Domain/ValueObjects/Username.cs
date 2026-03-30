using System.Text.RegularExpressions;
using CineLog.Domain.Exceptions;

namespace CineLog.Domain.ValueObjects;

public sealed class Username : IEquatable<Username>
{
    private static readonly Regex ValidPattern = new(@"^[a-zA-Z0-9_]{3,30}$", RegexOptions.Compiled);

    public string Value { get; }

    private Username(string value) => Value = value;

    public static Username Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ValidationDomainException("Username cannot be empty.");

        if (!ValidPattern.IsMatch(value))
            throw new ValidationDomainException(
                "Username must be 3–30 characters and contain only letters, digits, or underscores.");

        return new Username(value);
    }

    public static implicit operator Username(string value) => Create(value);
    public static implicit operator string(Username username) => username.Value;

    public bool Equals(Username? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Username u && Equals(u);
    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);
    public override string ToString() => Value;
}
