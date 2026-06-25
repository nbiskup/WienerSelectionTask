using System.Data;

namespace PartnerManagement.Data;

public interface ISqlConnectionFactory
{
    IDbConnection CreateDefaultConnection();
    IDbConnection CreateMasterConnection();
}