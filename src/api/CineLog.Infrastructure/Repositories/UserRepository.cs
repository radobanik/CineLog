using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IAppDbContext _context;

    public UserRepository(IAppDbContext context) => _context = context;

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
        await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default) =>
        await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserName == username, cancellationToken);
}
