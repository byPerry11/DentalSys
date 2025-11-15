using System;
using System.Data;
using DataAccess.Connections;

namespace DataAccess.Check.Connection
{
    // verificacion de conexion
    public class ConnectionChecker
    {
        private readonly IConnectionProvider _provider;

        public ConnectionChecker(IConnectionProvider provider)
        {
            _provider = provider;
        }

        public (bool ok, string message) TestConnection()
        {
            try
            {
                using (IDbConnection conn = _provider.CreateConnection())
                {
                    conn.Open();

                    using (var cmd = conn.CreateCommand())
                    {
                        // Prueba de conexion
                        cmd.CommandText = "SELECT 1";
                        cmd.ExecuteScalar();

                        return (true, $"Conexion de prueba exitosa ({_provider.ProviderName})");
                    }
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error en la conexion ({_provider.ProviderName}) - {ex.Message}");
            }
        }
    }
}
