using System.Configuration;

namespace DataAccess.Connections
{
    // Fábrica que crea el proveedor de conexión apropiado según la configuración en App.config
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
