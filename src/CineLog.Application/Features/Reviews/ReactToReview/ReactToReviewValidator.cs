using FluentValidation;

namespace CineLog.Application.Features.Reviews.ReactToReview;

public class ReactToReviewValidator : AbstractValidator<ReactToReviewCommand>
{
    public ReactToReviewValidator()
    {
        RuleFor(x => x.ReviewId).NotEmpty();
        RuleFor(x => x.ReactionType).IsInEnum();
    }
}
