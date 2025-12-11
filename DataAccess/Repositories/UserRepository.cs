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

        public UserEntity? GetUserById(int id)
        {
            UserEntity? user = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT id_usuario, nombre_usuario, password_hash, rol FROM usuarios WHERE id_usuario = @id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@id";
                    parameter.Value = id;
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

        public List<UserEntity> GetAllUsers()
        {
            var users = new List<UserEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT id_usuario, nombre_usuario, password_hash, rol, fecha_creacion FROM usuarios";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new UserEntity
                            {
                                Id_Usuario = reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Nombre_Usuario = reader.GetString(reader.GetOrdinal("nombre_usuario")),
                                Pasword_Hash = reader.GetString(reader.GetOrdinal("password_hash")),
                                Rol = reader.GetString(reader.GetOrdinal("rol")),
                                Fecha_Creacion = reader.IsDBNull(reader.GetOrdinal("fecha_creacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
                            };
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public void DeleteUser(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM usuarios WHERE id_usuario = @id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = "@id";
                    parameter.Value = id;
                    command.Parameters.Add(parameter);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void AddUser(UserEntity user)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "INSERT INTO usuarios (nombre_usuario, password_hash, rol, fecha_creacion) VALUES (@username, @password, @role, @date)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var p1 = command.CreateParameter(); p1.ParameterName = "@username"; p1.Value = user.Nombre_Usuario; command.Parameters.Add(p1);
                    var p2 = command.CreateParameter(); p2.ParameterName = "@password"; p2.Value = user.Pasword_Hash; command.Parameters.Add(p2);
                    var p3 = command.CreateParameter(); p3.ParameterName = "@role"; p3.Value = user.Rol; command.Parameters.Add(p3);
                    var p4 = command.CreateParameter(); p4.ParameterName = "@date"; p4.Value = user.Fecha_Creacion; command.Parameters.Add(p4);

                    command.ExecuteNonQuery();
                }
            }
        }


        public void UpdateUser(UserEntity user)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "UPDATE usuarios SET nombre_usuario = @username, rol = @role WHERE id_usuario = @id";
                if (!string.IsNullOrEmpty(user.Pasword_Hash))
                {
                    query = "UPDATE usuarios SET nombre_usuario = @username, rol = @role, password_hash = @password WHERE id_usuario = @id";
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var p1 = command.CreateParameter(); p1.ParameterName = "@username"; p1.Value = user.Nombre_Usuario; command.Parameters.Add(p1);
                    var p2 = command.CreateParameter(); p2.ParameterName = "@role"; p2.Value = user.Rol; command.Parameters.Add(p2);
                    var p3 = command.CreateParameter(); p3.ParameterName = "@id"; p3.Value = user.Id_Usuario; command.Parameters.Add(p3);

                    if (!string.IsNullOrEmpty(user.Pasword_Hash))
                    {
                        var p4 = command.CreateParameter(); p4.ParameterName = "@password"; p4.Value = user.Pasword_Hash; command.Parameters.Add(p4);
                    }

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
