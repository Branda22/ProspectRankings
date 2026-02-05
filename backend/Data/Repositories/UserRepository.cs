using backend.Models;
using Dapper;
using SqlKata;
using SqlKata.Compilers;

namespace backend.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly PostgresCompiler _compiler = new();

    public UserRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var query = new Query("Users").Where("Email", email).AsCount();
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        var count = await connection.ExecuteScalarAsync<int>(compiled.Sql, compiled.NamedBindings);
        return count > 0;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var query = new Query("Users").Where("Email", email);
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<User>(compiled.Sql, compiled.NamedBindings);
    }

    public async Task<User> CreateAsync(User user)
    {
        user.Id = Guid.NewGuid();
        var query = new Query("Users").AsInsert(new
        {
            user.Id,
            user.Email,
            user.PasswordHash,
            user.FirstName,
            user.LastName,
            user.CreatedAt,
            user.UpdatedAt
        });
        var compiled = _compiler.Compile(query);

        using var connection = _connectionFactory.CreateConnection();
        await connection.ExecuteAsync(compiled.Sql, compiled.NamedBindings);
        return user;
    }
}
