using backend.Models;
using Dapper;
using SqlKata;
using SqlKata.Compilers;

namespace backend.Data.Repositories;

public class SourceRepository : ISourceRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly PostgresCompiler _compiler = new();

    public SourceRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<Source>> GetAllAsync()
    {
        var query = new Query("Sources").OrderBy("Name");
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Source>(compiled.Sql, compiled.NamedBindings);
    }

    public async Task<Source> CreateAsync(Source source)
    {
        var query = new Query("Sources").AsInsert(new
        {
            source.Name
        });
        var compiled = _compiler.Compile(query);
        var sql = compiled.Sql + " RETURNING \"Id\"";

        using var connection = _connectionFactory.CreateConnection();
        source.Id = await connection.ExecuteScalarAsync<int>(sql, compiled.NamedBindings);
        return source;
    }
}
