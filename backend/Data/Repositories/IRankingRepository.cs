
using backend.Models;

namespace backend.Data.Repositories;

public interface IRankingRepository
{
    Task<IEnumerable<Ranking>> GetAllAsync();
    Task<Ranking> CreateAsync(Ranking ranking);
}