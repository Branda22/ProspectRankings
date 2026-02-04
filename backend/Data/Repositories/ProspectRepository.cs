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
<<<<<<< HEAD
            .Select("Id", "PlayerName", "Team", "Position", "Age", "ETA", "Rank", "Source")
            .OrderBy("Rank");
=======
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
>>>>>>> 610df4e (fixes deployment script)

        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
<<<<<<< HEAD
        return await connection.QueryAsync<Prospect>(compiled.Sql, compiled.NamedBindings);
    }

    public async Task<Prospect?> GetByIdAsync(Guid id)
    {
        var query = new Query("Prospects")
            .Select("Id", "PlayerName", "Team", "Position", "Age", "ETA", "Rank", "Source")
            .Where("Id", id);
=======
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
>>>>>>> 610df4e (fixes deployment script)

        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
<<<<<<< HEAD
        return await connection.QueryFirstOrDefaultAsync<Prospect>(compiled.Sql, compiled.NamedBindings);
=======
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
>>>>>>> 610df4e (fixes deployment script)
    }

    public async Task<Prospect> CreateAsync(Prospect prospect)
    {
<<<<<<< HEAD
        prospect.Id = Guid.NewGuid();
        var query = new Query("Prospects").AsInsert(new
        {
            prospect.Id,
=======
        var query = new Query("Prospects").AsInsert(new
        {
>>>>>>> 610df4e (fixes deployment script)
            prospect.PlayerName,
            prospect.Team,
            prospect.Position,
            prospect.Age,
            prospect.ETA,
            prospect.Rank,
<<<<<<< HEAD
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

=======
            prospect.SourceId
        });
        var compiled = _compiler.Compile(query);
        var sql = compiled.Sql + " RETURNING \"Id\"";

        using var connection = _connectionFactory.CreateConnection();
        prospect.Id = await connection.ExecuteScalarAsync<int>(sql, compiled.NamedBindings);
        return prospect;
    }

>>>>>>> 610df4e (fixes deployment script)
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
<<<<<<< HEAD
            prospect.Source
=======
            prospect.SourceId
>>>>>>> 610df4e (fixes deployment script)
        });
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return affected > 0;
    }

<<<<<<< HEAD
    public async Task<bool> DeleteAsync(Guid id)
=======
    public async Task<bool> DeleteAsync(int id)
>>>>>>> 610df4e (fixes deployment script)
    {
        var query = new Query("Prospects").Where("Id", id).AsDelete();
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var affected = await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return affected > 0;
    }

<<<<<<< HEAD
    public async Task<bool> ExistsAsync(Guid id)
=======
    public async Task<bool> ExistsAsync(int id)
>>>>>>> 610df4e (fixes deployment script)
    {
        var query = new Query("Prospects").Where("Id", id).AsCount();
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(compiled.Sql, compiled.NamedBindings);
        return count > 0;
    }
}
