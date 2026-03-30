using CineLog.Application.Behaviors;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;

namespace CineLog.UnitTests.Application;

public record ValidationTestRequest(string Value) : IRequest<string>;

public class ValidationBehaviorTests
{

    [Fact]
    public async Task Handle_WithNoValidators_CallsNext()
    {
        var behavior = new ValidationBehavior<ValidationTestRequest, string>(
            Enumerable.Empty<IValidator<ValidationTestRequest>>());

        var nextCalled = false;
        Task<string> Next(CancellationToken ct = default)
        {
            nextCalled = true;
            return Task.FromResult("ok");
        }

        var result = await behavior.Handle(
            new ValidationTestRequest("hello"), Next, CancellationToken.None);

        nextCalled.Should().BeTrue();
        result.Should().Be("ok");
    }

    [Fact]
    public async Task Handle_WithValidRequest_CallsNext()
    {
        var validator = Substitute.For<IValidator<ValidationTestRequest>>();
        validator
            .ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var behavior = new ValidationBehavior<ValidationTestRequest, string>(
            new[] { validator });

        var nextCalled = false;
        Task<string> Next(CancellationToken ct = default)
        {
            nextCalled = true;
            return Task.FromResult("ok");
        }

        await behavior.Handle(new ValidationTestRequest("valid"), Next, CancellationToken.None);

        nextCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_WithInvalidRequest_ThrowsValidationException()
    {
        var validator = Substitute.For<IValidator<ValidationTestRequest>>();
        var failure = new ValidationFailure("Value", "Value is required.");
        validator
            .ValidateAsync(Arg.Any<ValidationContext<ValidationTestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult(new[] { failure }));

        var behavior = new ValidationBehavior<ValidationTestRequest, string>(
            new[] { validator });

        var act = async () => await behavior.Handle(
            new ValidationTestRequest(""), ct => Task.FromResult("ok"), CancellationToken.None);

        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("*Value is required*");
    }
}
