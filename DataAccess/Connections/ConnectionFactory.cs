using System.Configuration;

namespace DataAccess.Connections
{
    // Fabrica que elige el proveedor segun App.config
    public static class ConnectionFactory
    {
        public static IConnectionProvider Create()
        {
            var active = ConfigurationManager.AppSettings["ActiveDb"] ?? "MySql";
            return active switch
            {
                "SqlServer" => new SqlServerConnectionProvider(),
                _ => new MySqlConnectionProvider()
            };
        }
    }
}
