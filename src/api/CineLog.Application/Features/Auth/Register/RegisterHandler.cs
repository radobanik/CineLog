using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CineLog.Application.Features.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public RegisterHandler(UserManager<User> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new ConflictException(errors);
        }

        await _userManager.AddToRoleAsync(user, UserRoles.User);

        var token = _jwtService.GenerateToken(user.Id, user.UserName!, user.Email!, UserRoles.User);
        return new AuthResponse(token, user.Id, user.UserName!);
    }
}
