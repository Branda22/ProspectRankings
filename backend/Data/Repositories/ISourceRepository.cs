using backend.Models;

namespace backend.Data.Repositories;

public interface ISourceRepository
{
    Task<IEnumerable<Source>> GetAllAsync();
    Task<Source> CreateAsync(Source source);
}
