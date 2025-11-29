using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace DataAccess.Connections
{
    // Proveedor de conexión para SQL Server
    public class SqlServerConnectionProvider : IConnectionProvider
    {
        private readonly string _connectionString;

      
        public string ProviderName => "Microsoft.Data.SqlClient";


        public SqlServerConnectionProvider()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DentalSysSqlServer"]?.ConnectionString
                ?? throw new ConfigurationErrorsException("Missing connection string: DentalSysSqlServer");
        }

        public IDbConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }
    }
}
