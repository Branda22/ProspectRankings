using backend.Models;
using Dapper;
using SqlKata;
using SqlKata.Compilers;

namespace backend.Data.Repositories;

public class ProspectRepository : IProspectRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly PostgresCompiler _compiler = new();

    public ProspectRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Prospect>> GetAllAsync(int? sourceId)
    {
        var query = new Query("Prospects")
            .Select("Id", "PlayerName", "Team", "Position", "Age", "ETA", "Rank", "Source")
            .OrderBy("Rank");

        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Prospect>(compiled.Sql, compiled.NamedBindings);
    }

    public async Task<Prospect?> GetByIdAsync(Guid id)
    {
        var query = new Query("Prospects")
            .Select("Id", "PlayerName", "Team", "Position", "Age", "ETA", "Rank", "Source")
            .Where("Id", id);

        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<Prospect>(compiled.Sql, compiled.NamedBindings);
    }

    public async Task<Prospect> CreateAsync(Prospect prospect)
    {
        prospect.Id = Guid.NewGuid();
        var query = new Query("Prospects").AsInsert(new
        {
            prospect.Id,
            prospect.PlayerName,
            prospect.Team,
            prospect.Position,
            prospect.Age,
            prospect.ETA,
            prospect.Rank,
            prospect.Source
        });
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return prospect;
    }

    public async Task<List<Guid>> BulkCreateAsync(IEnumerable<Prospect> prospects)
    {
        var prospectList = prospects.ToList();
        var ids = new List<Guid>();

        using var connection = _connectionFactory.CreateConnection();
        connection.Open();

        foreach (var prospect in prospectList)
        {
            prospect.Id = Guid.NewGuid();
            ids.Add(prospect.Id);

            var query = new Query("Prospects").AsInsert(new
            {
                prospect.Id,
                prospect.PlayerName,
                prospect.Team,
                prospect.Position,
                prospect.Age,
                prospect.ETA,
                prospect.Rank,
                prospect.Source
            });
            var compiled = _compiler.Compile(query);
            await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        }

        return ids;
    }

    public async Task<bool> UpdateAsync(Prospect prospect)
    {
        var query = new Query("Prospects").Where("Id", prospect.Id).AsUpdate(new
        {
            prospect.PlayerName,
            prospect.Team,
            prospect.Position,
            prospect.Age,
            prospect.ETA,
            prospect.Rank,
            prospect.Source
        });
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var query = new Query("Prospects").Where("Id", id).AsDelete();
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return affected > 0;
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        var query = new Query("Prospects").Where("Id", id).AsCount();
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(compiled.Sql, compiled.NamedBindings);
        return count > 0;
    }
}
