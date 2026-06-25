using System.Data;
using Microsoft.Data.SqlClient;

namespace PartnerManagement.Data;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly IConfiguration _configuration;

    public SqlConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateDefaultConnection()
    {
        return new SqlConnection(GetConnectionString("DefaultConnection"));
    }

    public IDbConnection CreateMasterConnection()
    {
        return new SqlConnection(GetConnectionString("MasterConnection"));
    }

    private string GetConnectionString(string name)
    {
        return _configuration.GetConnectionString(name)
            ?? throw new InvalidOperationException($"Connection string '{name}' is not configured.");
    }
}