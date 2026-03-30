using BCrypt.Net;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;

    public LoginHandler(IUserRepository userRepository, IJwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken)
            ?? throw new NotFoundException($"User with email '{request.Email}' not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var token = _jwtService.GenerateToken(user.Id, user.Username.Value, user.Email, user.Role.ToString());
        return new AuthResponse(token, user.Id, user.Username.Value);
    }
}
