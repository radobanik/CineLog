using CineLog.Domain.Exceptions;
using CineLog.Domain.ValueObjects;
using FluentAssertions;

namespace CineLog.UnitTests.Domain;

public class UsernameTests
{
    [Theory]
    [InlineData("abc")]
    [InlineData("a_b_c")]
    [InlineData("abcdefghij")]
    [InlineData("User123")]
    public void Create_ValidValue_Succeeds(string value)
    {
        var username = Username.Create(value);
        username.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("ab")]            // too short
    [InlineData("")]              // empty
    [InlineData("hello world")]   // space
    [InlineData("user@name")]     // special char
    [InlineData("user-name")]     // dash not allowed
    public void Create_InvalidValue_ThrowsDomainException(string value)
    {
        var act = () => Username.Create(value);
        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void ImplicitConversion_FromString_Works()
    {
        Username username = "valid_user";
        username.Value.Should().Be("valid_user");
    }

    [Fact]
    public void Equality_SameValue_IsEqual()
    {
        var a = Username.Create("alice");
        var b = Username.Create("alice");
        a.Should().Be(b);
    }
}
