using Dapper;
using Microsoft.Data.SqlClient;
using PartnerManagement.Models;

namespace PartnerManagement.Data;

public sealed class PartnerRepository : IPartnerRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public PartnerRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IReadOnlyList<PartnerListItem>> GetAllAsync()
    {
        const string sql = """
            SELECT
                p.PartnerId,
                p.FirstName,
                p.LastName,
                p.Address,
                p.PartnerNumber,
                p.CroatianPIN,
                p.PartnerTypeId,
                p.CreatedAtUtc,
                p.CreatedByUser,
                p.IsForeign,
                p.ExternalCode,
                p.Gender,
                COUNT(po.PolicyId) AS PolicyCount,
                COALESCE(SUM(po.PolicyAmount), 0) AS TotalPolicyAmount
            FROM dbo.Partners p
            LEFT JOIN dbo.Policies po ON po.PartnerId = p.PartnerId
            GROUP BY
                p.PartnerId,
                p.FirstName,
                p.LastName,
                p.Address,
                p.PartnerNumber,
                p.CroatianPIN,
                p.PartnerTypeId,
                p.CreatedAtUtc,
                p.CreatedByUser,
                p.IsForeign,
                p.ExternalCode,
                p.Gender
            ORDER BY p.CreatedAtUtc DESC;
            """;

        using var connection = _connectionFactory.CreateDefaultConnection();
        var partners = await connection.QueryAsync<PartnerListItem>(sql);
        return partners.AsList();
    }

    public async Task<IReadOnlyList<PartnerOption>> GetPartnerOptionsAsync()
    {
        const string sql = """
            SELECT PartnerId, CONCAT(FirstName, ' ', LastName) AS FullName
            FROM dbo.Partners
            ORDER BY CreatedAtUtc DESC;
            """;

        using var connection = _connectionFactory.CreateDefaultConnection();
        var partners = await connection.QueryAsync<PartnerOption>(sql);
        return partners.AsList();
    }
    public async Task<int> CreateAsync(PartnerCreateViewModel partner)
    {
        const string sql = """
            INSERT INTO dbo.Partners
            (
                FirstName,
                LastName,
                Address,
                PartnerNumber,
                CroatianPIN,
                PartnerTypeId,
                CreatedByUser,
                IsForeign,
                ExternalCode,
                Gender
            )
            OUTPUT INSERTED.PartnerId
            VALUES
            (
                @FirstName,
                @LastName,
                @Address,
                @PartnerNumber,
                NULLIF(@CroatianPIN, ''),
                @PartnerTypeId,
                @CreatedByUser,
                @IsForeign,
                @ExternalCode,
                @Gender
            );
            """;

        using var connection = _connectionFactory.CreateDefaultConnection();
        try
        {
            return await connection.ExecuteScalarAsync<int>(sql, partner);
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            throw new InvalidOperationException("External code must be unique.", ex);
        }
    }

    public async Task<bool> ExternalCodeExistsAsync(string externalCode)
    {
        const string sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.Partners WHERE ExternalCode = @ExternalCode) THEN 1 ELSE 0 END AS bit);";

        using var connection = _connectionFactory.CreateDefaultConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { ExternalCode = externalCode });
    }

    public async Task<bool> PartnerExistsAsync(int partnerId)
    {
        const string sql = "SELECT CAST(CASE WHEN EXISTS (SELECT 1 FROM dbo.Partners WHERE PartnerId = @PartnerId) THEN 1 ELSE 0 END AS bit);";

        using var connection = _connectionFactory.CreateDefaultConnection();
        return await connection.ExecuteScalarAsync<bool>(sql, new { PartnerId = partnerId });
    }

    public async Task<(int PolicyCount, decimal TotalPolicyAmount, string FullName)> AddPolicyAsync(PolicyCreateViewModel policy)
    {
        const string sql = """
            INSERT INTO dbo.Policies (PartnerId, PolicyNumber, PolicyAmount)
            VALUES (@PartnerId, @PolicyNumber, @PolicyAmount);

            SELECT
                COUNT(po.PolicyId) AS PolicyCount,
                COALESCE(SUM(po.PolicyAmount), 0) AS TotalPolicyAmount,
                CONCAT(p.FirstName, ' ', p.LastName) AS FullName
            FROM dbo.Partners p
            LEFT JOIN dbo.Policies po ON po.PartnerId = p.PartnerId
            WHERE p.PartnerId = @PartnerId
            GROUP BY p.FirstName, p.LastName;
            """;

        using var connection = _connectionFactory.CreateDefaultConnection();
        var result = await connection.QuerySingleAsync<PolicySummary>(sql, policy);
        return (result.PolicyCount, result.TotalPolicyAmount, result.FullName);
    }

    private sealed class PolicySummary
    {
        public int PolicyCount { get; set; }
        public decimal TotalPolicyAmount { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
}