using Microsoft.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;

namespace DataAccess.Connections
{
    // Proveedor de conexion para SQL Server
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
