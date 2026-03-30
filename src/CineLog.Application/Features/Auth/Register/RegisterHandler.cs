using BCrypt.Net;
using CineLog.Domain.Entities;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Auth.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContext _context;
    private readonly IJwtService _jwtService;

    public RegisterHandler(
        IUserRepository userRepository,
        IAppDbContext context,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var existing = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
            throw new ConflictException($"Email '{request.Email}' is already registered.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = User.Create(request.Username, request.Email, passwordHash);

        await _context.Users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        var token = _jwtService.GenerateToken(user.Id, user.Username.Value, user.Email, user.Role.ToString());
        return new AuthResponse(token, user.Id, user.Username.Value);
    }
}
