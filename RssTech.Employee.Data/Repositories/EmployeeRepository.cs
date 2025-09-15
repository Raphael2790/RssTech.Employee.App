using RssTech.Employee.Domain.Interfaces.Repositories;
using RssTech.Employee.Infrastructure.Context;

namespace RssTech.Employee.Infrastructure.Repositories;

public sealed class EmployeeRepository(AppDbContext appDbContext)
    : IEmployeeRepository
{
    public Task<bool> ExistsByDocumentNumber(string documentNumber, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsByEmail(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsById(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Entities.Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<Domain.Entities.Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Update(Domain.Entities.Employee employee, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Create(Domain.Entities.Employee employee, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
