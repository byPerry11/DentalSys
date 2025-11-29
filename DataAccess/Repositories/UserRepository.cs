using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Connections;
using DataAccess.Entities; // para el user entities

namespace DataAccess.Repositories
{
    // Repositorio para operaciones de usuarios en la base de datos
   
    public class UserRepository
    {
        private readonly IConnectionProvider _provider;

       
        public UserRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        
        public UserEntity? GetUserByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            UserEntity? user = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT id_usuario, nombre_usuario, password_hash, rol FROM usuarios WHERE nombre_usuario = @username";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    // Agregar parámetro de forma agnóstica al proveedor
                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@username";
                    parameter.Value = username;
                    command.Parameters.Add(parameter);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            user = new UserEntity
                            {
                                Id_Usuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Nombre_Usuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                                Pasword_Hash = reader.GetString(reader.GetOrdinal("password_hash")),
                                Rol = reader.GetString(reader.GetOrdinal("rol"))

                            };
                        }
                    }
                }
            }

            return user;
        }
    }
}
