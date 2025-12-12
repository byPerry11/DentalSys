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

        public int AddUserReturningId(UserEntity user)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO usuarios (nombre_usuario, password_hash, rol, fecha_creacion) VALUES (@username, @password, @role, @date)";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@username"; p1.Value = user.Nombre_Usuario; command.Parameters.Add(p1);
                    var p2 = command.CreateParameter(); p2.ParameterName = "@password"; p2.Value = user.Pasword_Hash; command.Parameters.Add(p2);
                    var p3 = command.CreateParameter(); p3.ParameterName = "@role"; p3.Value = user.Rol; command.Parameters.Add(p3);
                    var p4 = command.CreateParameter(); p4.ParameterName = "@date"; p4.Value = user.Fecha_Creacion; command.Parameters.Add(p4);

                    command.ExecuteNonQuery();

                    // Intentar obtener LAST_INSERT_ID (MySQL), si falla probar SCOPE_IDENTITY (SQL Server)
                    try
                    {
                        using (var idCommand = connection.CreateCommand())
                        {
                            idCommand.CommandText = "SELECT LAST_INSERT_ID()";
                            var result = idCommand.ExecuteScalar();
                            if (result != null && int.TryParse(result.ToString(), out var idRes))
                                return idRes;
                        }
                    }
                    catch { /* ignora y prueba siguiente */ }

                    try
                    {
                        using (var idCommand = connection.CreateCommand())
                        {
                            idCommand.CommandText = "SELECT CAST(SCOPE_IDENTITY() AS int)";
                            var result = idCommand.ExecuteScalar();
                            if (result != null && int.TryParse(result.ToString(), out var idRes))
                                return idRes;
                        }
                    }
                    catch { /* ignora */ }


                    return 0;
                }
            }
        }

        public List<DentistaEntity> GetDentistasDisponibles()
        {
            var list = new List<DentistaEntity>();
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Dentista WHERE id_usuario IS NULL";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new DentistaEntity
                            {
                                Id_Dentista = reader.GetInt32(reader.GetOrdinal("Id_Dentista")),
                                Id_Usuario = reader.IsDBNull(reader.GetOrdinal("id_usuario")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? null : reader.GetString(reader.GetOrdinal("Especialidad")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"))
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<AdministradorEntity> GetAdministradoresDisponibles()
        {
            var list = new List<AdministradorEntity>();
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Administrador WHERE id_usuario IS NULL";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new AdministradorEntity
                            {
                                Id_Administrador = reader.GetInt32(reader.GetOrdinal("id_administrador")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                                Fecha_Creacion = reader.IsDBNull(reader.GetOrdinal("fecha_creacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
                            });
                        }
                    }
                }
            }
            return list;
        }

        public List<RecepcionistaEntity> GetRecepcionistasDisponibles()
        {
            var list = new List<RecepcionistaEntity>();
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Recepcionista WHERE id_usuario IS NULL";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new RecepcionistaEntity
                            {
                                Id_Recepcionista = reader.GetInt32(reader.GetOrdinal("id_recepcionista")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                                Fecha_Creacion = reader.IsDBNull(reader.GetOrdinal("fecha_creacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
                            });
                        }
                    }
                }
            }
            return list;
        }

        // --- Asignar usuario a perfil ---
        public void AssignUserToDentista(int dentistaId, int usuarioId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Dentista SET id_usuario = @UserId WHERE Id_Dentista = @DentistaId";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@UserId"; p1.Value = usuarioId; command.Parameters.Add(p1);
                    var p2 = command.CreateParameter(); p2.ParameterName = "@DentistaId"; p2.Value = dentistaId; command.Parameters.Add(p2);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AssignUserToRecepcionista(int recepId, int usuarioId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Recepcionista SET id_usuario = @UserId WHERE id_recepcionista = @RecepId";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@UserId"; p1.Value = usuarioId; command.Parameters.Add(p1);
                    var p2 = command.CreateParameter(); p2.ParameterName = "@RecepId"; p2.Value = recepId; command.Parameters.Add(p2);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AssignUserToAdministrador(int adminId, int usuarioId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Administrador SET id_usuario = @UserId WHERE id_administrador = @AdminId";
                    var p1 = command.CreateParameter(); p1.ParameterName = "@UserId"; p1.Value = usuarioId; command.Parameters.Add(p1);
                    var p2 = command.CreateParameter(); p2.ParameterName = "@AdminId"; p2.Value = adminId; command.Parameters.Add(p2);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RemoveUserFromDentista(int dentistaId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Dentista SET id_usuario = NULL WHERE Id_Dentista = @DentistaId";
                    var p = command.CreateParameter(); p.ParameterName = "@DentistaId"; p.Value = dentistaId; command.Parameters.Add(p);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RemoveUserFromRecepcionista(int recepId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Recepcionista SET id_usuario = NULL WHERE id_recepcionista = @RecepId";
                    var p = command.CreateParameter(); p.ParameterName = "@RecepId"; p.Value = recepId; command.Parameters.Add(p);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RemoveUserFromAdministrador(int adminId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "UPDATE Administrador SET id_usuario = NULL WHERE id_administrador = @AdminId";
                    var p = command.CreateParameter(); p.ParameterName = "@AdminId"; p.Value = adminId; command.Parameters.Add(p);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
