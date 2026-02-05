using backend.Models;
using Dapper;
using SqlKata;
using SqlKata.Compilers;

namespace backend.Data.Repositories;

public class RankingRepository : IRankingRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly PostgresCompiler _compiler = new();

    public RankingRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Ranking>> GetAllAsync()
    {
        var query = new Query("Rankings")
            .Select("Id", "Rank", "PlayerName", "Team", "Position", "Age", "ETA", "Score", "Volatility", "Consensus")
            .OrderBy("Rank");

        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Ranking>(compiled.Sql, compiled.NamedBindings);
    }

    public async Task<Ranking> CreateAsync(Ranking ranking)
    {
        ranking.Id = Guid.NewGuid();
        var query = new Query("Rankings").AsInsert(new
        {
            ranking.Id,
            ranking.Rank,
            ranking.PlayerName,
            ranking.Team,
            ranking.Position,
            ranking.Age,
            ranking.ETA,
            ranking.Score,
            ranking.Volatility,
            ranking.Consensus
        });
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return ranking;
    }
}
