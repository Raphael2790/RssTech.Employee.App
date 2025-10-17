namespace RssTech.Employee.Domain.Interfaces.Repositories;

public interface IEmployeeRepository
{
    Task<Entities.Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Entities.Employee?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByEmail(string email, CancellationToken cancellationToken);
    Task<bool> ExistsByDocumentNumber(string documentNumber, CancellationToken cancellationToken);
    Task<bool> ExistsById(Guid id, CancellationToken cancellationToken);
    Task Create(Entities.Employee employee, CancellationToken cancellationToken);
    Task Update(Entities.Employee employee, CancellationToken cancellationToken);
    Task<IEnumerable<Entities.Employee>> GetAllAsync(CancellationToken cancellationToken);
}
