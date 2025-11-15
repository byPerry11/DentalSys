using DataAccess.Check.Connection;
using DataAccess.Connections;

namespace ApplicationLogic.Services
{
    // prueba de conexion a la UI
    public class DatabaseService
    {
        public string ProbarConexion()
        {
            var checker = new ConnectionChecker(ConnectionFactory.Create());
            var (ok, msg) = checker.TestConnection();
            return msg;
        }
    }
}
