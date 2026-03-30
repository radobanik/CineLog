using CineLog.Domain.Entities;
using CineLog.Domain.Enums;
using CineLog.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CineLog.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;

    public LoginHandler(UserManager<User> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(request.Email)
            ?? throw new NotFoundException($"User with email '{request.Email}' not found.");

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? UserRoles.User;

        var token = _jwtService.GenerateToken(user.Id, user.UserName!, user.Email!, role);
        return new AuthResponse(token, user.Id, user.UserName!);
    }
}
