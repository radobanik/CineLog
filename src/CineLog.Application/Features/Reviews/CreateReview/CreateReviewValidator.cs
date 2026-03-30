using FluentValidation;

namespace CineLog.Application.Features.Reviews.CreateReview;

public class CreateReviewValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewValidator()
    {
        RuleFor(x => x.MovieId).NotEmpty();
        RuleFor(x => x.Rating)
            .InclusiveBetween(0.5m, 5.0m)
            .Must(r => r % 0.5m == 0)
            .WithMessage("Rating must be a multiple of 0.5 between 0.5 and 5.0.");
        RuleFor(x => x.ReviewText)
            .MaximumLength(5000)
            .When(x => x.ReviewText is not null);
    }
}
