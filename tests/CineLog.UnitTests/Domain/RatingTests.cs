using CineLog.Domain.Exceptions;
using CineLog.Domain.ValueObjects;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class RatingTests
{
    [Theory]
    [InlineData(0.5)]
    [InlineData(1.0)]
    [InlineData(2.5)]
    [InlineData(5.0)]
    public void Create_ValidValue_Succeeds(decimal value)
    {
        var rating = Rating.Create(value);
        rating.Value.Should().Be(value);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(0.3)]
    [InlineData(5.5)]
    [InlineData(-1)]
    public void Create_InvalidValue_ThrowsDomainException(decimal value)
    {
        var act = () => Rating.Create(value);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ImplicitConversion_FromDecimal_Works()
    {
        Rating rating = 3.5m;
        rating.Value.Should().Be(3.5m);
    }

    [Fact]
    public void ImplicitConversion_ToDecimal_Works()
    {
        var rating = Rating.Create(4.0m);
        decimal value = rating;
        value.Should().Be(4.0m);
    }
}
