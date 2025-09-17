using Microsoft.EntityFrameworkCore;
using RssTech.Employee.Domain.Interfaces.Repositories;
using RssTech.Employee.Infrastructure.Context;

namespace RssTech.Employee.Infrastructure.Repositories;

public sealed class EmployeeRepository(AppDbContext appDbContext)
    : IEmployeeRepository
{
    public async Task<bool> ExistsByDocumentNumber(string documentNumber, CancellationToken cancellationToken)
    => await appDbContext.Employees
            .AnyAsync(e => e.Document.DocumentNumber == documentNumber, cancellationToken);

    public async Task<bool> ExistsByEmail(string email, CancellationToken cancellationToken)
        => await appDbContext.Employees
            .AnyAsync(e => e.Email.Address == email, cancellationToken);

    public async Task<bool> ExistsById(Guid id, CancellationToken cancellationToken)
    => await appDbContext.Employees
            .AnyAsync(e => e.Id == id, cancellationToken);

    public async Task<Domain.Entities.Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    => await appDbContext.Employees
        .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<Domain.Entities.Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    => await appDbContext.Employees
        .FirstOrDefaultAsync(e => e.Email.Address == email, cancellationToken);

    public async Task Update(Domain.Entities.Employee employee, CancellationToken cancellationToken)
    {
        appDbContext.Employees.Update(employee);
        await appDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task Create(Domain.Entities.Employee employee, CancellationToken cancellationToken)
    {
        await appDbContext.Employees.AddAsync(employee, cancellationToken);
        await appDbContext.SaveChangesAsync(cancellationToken);
    }
}
