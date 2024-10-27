using System.Data;
using Microsoft.Data.SqlClient;

namespace Metrics.ConnectionFactory;

public interface IConnectionFactory
{
    public Task<IDbConnection> GetConnection(CancellationToken cancellationToken = default);
}

public class ConnectionFactory(string connectionString) : IConnectionFactory
{
    public async Task<IDbConnection> GetConnection(CancellationToken cancellationToken = default)
    {
        var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}