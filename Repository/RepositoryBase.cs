using System.Data;
using Contracts;
using Npgsql;
using Microsoft.Extensions.Configuration;

namespace Repository;

public class RepositoryBase
{
    private string _connSting;

    public RepositoryBase(IConfiguration configuration)
    {
        _connSting = configuration.GetConnectionString("PostgreSQL");
    }
    public IDbConnection Connection
    {
        get => new NpgsqlConnection(_connSting);
    }
}