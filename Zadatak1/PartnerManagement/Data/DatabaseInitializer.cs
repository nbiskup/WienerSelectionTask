using Dapper;
using Microsoft.Data.SqlClient;

namespace PartnerManagement.Data;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    public DatabaseInitializer(
        ISqlConnectionFactory connectionFactory,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _connectionFactory = connectionFactory;
        _configuration = configuration;
        _environment = environment;
    }

    public async Task EnsureCreatedAsync()
    {
        await EnsureDatabaseExistsAsync();
        await EnsureSchemaExistsAsync();
    }

    private async Task EnsureDatabaseExistsAsync()
    {
        var defaultConnectionString = _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured.");
        var builder = new SqlConnectionStringBuilder(defaultConnectionString);
        var databaseName = builder.InitialCatalog;

        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new InvalidOperationException("DefaultConnection must define a database name.");
        }

        var escapedDatabaseName = databaseName.Replace("]", "]]", StringComparison.Ordinal);
        var sql = $"IF DB_ID(N'{databaseName.Replace("'", "''", StringComparison.Ordinal)}') IS NULL CREATE DATABASE [{escapedDatabaseName}];";

        using var connection = _connectionFactory.CreateMasterConnection();
        await connection.ExecuteAsync(sql);
    }

    private async Task EnsureSchemaExistsAsync()
    {
        var scriptPath = Path.Combine(_environment.ContentRootPath, "Data", "Scripts", "001_create_schema.sql");
        var schemaSql = await File.ReadAllTextAsync(scriptPath);

        using var connection = _connectionFactory.CreateDefaultConnection();
        await connection.ExecuteAsync(schemaSql);
    }
}