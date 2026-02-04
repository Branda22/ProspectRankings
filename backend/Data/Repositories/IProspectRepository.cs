using backend.Models;

namespace backend.Data.Repositories;

public interface IProspectRepository
{
    Task<IEnumerable<Prospect>> GetAllAsync(int? sourceId);
<<<<<<< HEAD
    Task<Prospect?> GetByIdAsync(Guid id);
    Task<Prospect> CreateAsync(Prospect prospect);
    Task<List<Guid>> BulkCreateAsync(IEnumerable<Prospect> prospects);
    Task<bool> UpdateAsync(Prospect prospect);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
=======
    Task<Prospect?> GetByIdAsync(int id);
    Task<Prospect> CreateAsync(Prospect prospect);
    Task<bool> UpdateAsync(Prospect prospect);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
>>>>>>> 610df4e (fixes deployment script)
}
