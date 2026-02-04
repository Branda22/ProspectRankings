using backend.Models;

namespace backend.Data.Repositories;

public interface IProspectRepository
{
    Task<IEnumerable<Prospect>> GetAllAsync(int? sourceId);
    Task<Prospect?> GetByIdAsync(int id);
    Task<Prospect> CreateAsync(Prospect prospect);
    Task<bool> UpdateAsync(Prospect prospect);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
