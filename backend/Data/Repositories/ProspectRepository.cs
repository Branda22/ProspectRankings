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
            .Join("Sources", "Sources.Id", "Prospects.SourceId")
            .Select(
                "Prospects.Id", "Prospects.PlayerName", "Prospects.Team",
                "Prospects.Position", "Prospects.Age", "Prospects.ETA",
                "Prospects.Rank", "Prospects.SourceId",
                "Sources.Id", "Sources.Name")
            .OrderBy("Prospects.Rank");

        if (sourceId.HasValue)
        {
            query = query.Where("Prospects.SourceId", sourceId.Value);
        }

        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Prospect, Source, Prospect>(
            compiled.Sql,
            (prospect, source) =>
            {
                prospect.Source = source;
                return prospect;
            },
            compiled.NamedBindings,
            splitOn: "Id");
    }

    public async Task<Prospect?> GetByIdAsync(int id)
    {
        var query = new Query("Prospects")
            .Join("Sources", "Sources.Id", "Prospects.SourceId")
            .Select(
                "Prospects.Id", "Prospects.PlayerName", "Prospects.Team",
                "Prospects.Position", "Prospects.Age", "Prospects.ETA",
                "Prospects.Rank", "Prospects.SourceId",
                "Sources.Id", "Sources.Name")
            .Where("Prospects.Id", id);

        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var results = await connection.QueryAsync<Prospect, Source, Prospect>(
            compiled.Sql,
            (prospect, source) =>
            {
                prospect.Source = source;
                return prospect;
            },
            compiled.NamedBindings,
            splitOn: "Id");

        return results.FirstOrDefault();
    }

    public async Task<Prospect> CreateAsync(Prospect prospect)
    {
        var query = new Query("Prospects").AsInsert(new
        {
            prospect.PlayerName,
            prospect.Team,
            prospect.Position,
            prospect.Age,
            prospect.ETA,
            prospect.Rank,
            prospect.SourceId
        });
        var compiled = _compiler.Compile(query);
        var sql = compiled.Sql + " RETURNING \"Id\"";

        using var connection = _connectionFactory.CreateConnection();
        prospect.Id = await connection.ExecuteScalarAsync<int>(sql, compiled.NamedBindings);
        return prospect;
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
            prospect.SourceId
        });
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return affected > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var query = new Query("Prospects").Where("Id", id).AsDelete();
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return affected > 0;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        var query = new Query("Prospects").Where("Id", id).AsCount();
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(compiled.Sql, compiled.NamedBindings);
        return count > 0;
    }
}
