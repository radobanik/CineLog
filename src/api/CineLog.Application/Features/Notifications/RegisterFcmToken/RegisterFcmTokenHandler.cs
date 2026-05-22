using CineLog.Application.Common;
using CineLog.Domain.Exceptions;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using MediatR;

namespace CineLog.Application.Features.Notifications.RegisterFcmToken;

public class RegisterFcmTokenHandler : IRequestHandler<RegisterFcmTokenCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IAppDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RegisterFcmTokenHandler(
        IUserRepository userRepository,
        IAppDbContext context,
        ICurrentUserService currentUser)
    {
        _userRepository = userRepository;
        _context = context;
        _currentUser = currentUser;
    }

    public async Task Handle(RegisterFcmTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(_currentUser.UserId, cancellationToken)
            ?? throw new NotFoundException($"User {_currentUser.UserId} not found.");

        user.FcmToken = request.Token;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
