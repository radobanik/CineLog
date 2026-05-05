using FluentValidation;

namespace CineLog.Application.Features.Users.UpdateProfile;

public class UpdateProfileValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(100)
            .When(x => x.Bio is not null);
    }
}
