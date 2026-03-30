using CineLog.Domain.Exceptions;

namespace CineLog.Domain.ValueObjects;

public sealed class Rating : IEquatable<Rating>
{
    public decimal Value { get; }

    private Rating(decimal value) => Value = value;

    public static Rating Create(decimal value)
    {
        if (value < 0.5m || value > 5.0m)
            throw new ValidationDomainException("Rating must be between 0.5 and 5.0.");

        if (value % 0.5m != 0)
            throw new ValidationDomainException("Rating must be a multiple of 0.5.");

        return new Rating(value);
    }

    public static implicit operator Rating(decimal value) => Create(value);
    public static implicit operator decimal(Rating rating) => rating.Value;

    public bool Equals(Rating? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Rating r && Equals(r);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value.ToString("0.0");
}
