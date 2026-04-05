using CineLog.Domain.Entities;
using CineLog.Domain.Interfaces;
using CineLog.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CineLog.Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly IAppDbContext _context;

    public PersonRepository(IAppDbContext context) => _context = context;

    public async Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _context.Persons
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task AddAsync(Person person, CancellationToken cancellationToken = default)
    {
        await _context.Persons.AddAsync(person, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Person person, CancellationToken cancellationToken = default)
    {
        _context.Persons.Update(person);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Person person, CancellationToken cancellationToken = default)
    {
        _context.Persons.Remove(person);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
