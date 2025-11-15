using System.Data;

namespace DataAccess.Connections
{
    
    public interface IConnectionProvider
    {
        IDbConnection CreateConnection();
        string ProviderName { get; }
    }
}
