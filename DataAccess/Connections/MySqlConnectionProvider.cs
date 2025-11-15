using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace DataAccess.Connections
{
    // Proveedor de conexion para MySQL
    public class MySqlConnectionProvider : IConnectionProvider
    {
        private readonly string _connectionString;
        public string ProviderName => "MySql.Data.MySqlClient";

        public MySqlConnectionProvider()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["DentalSysMySql"]?.ConnectionString
                ?? throw new ConfigurationErrorsException("Missing connection string: DentalSysMySql");
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
