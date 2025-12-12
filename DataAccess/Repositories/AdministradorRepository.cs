using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccess.Connections;

namespace DataAccess.Repositories
{
    public class AdministradorRepository
    {
        private readonly IConnectionProvider _provider;

        public AdministradorRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public List<AdministradorEntity> GetAllAdministradores()
        {
            var list = new List<AdministradorEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Administrador";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var a = new AdministradorEntity
                            {
                                Id_Administrador = reader.GetInt32(reader.GetOrdinal("id_administrador")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                                Fecha_Creacion = reader.IsDBNull(reader.GetOrdinal("fecha_creacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
                            };

                            list.Add(a);
                        }
                    }
                }
            }

            return list;
        }

        public AdministradorEntity? GetAdministradorById(int id)
        {
            AdministradorEntity? admin = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Administrador WHERE id_administrador = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            admin = new AdministradorEntity
                            {
                                Id_Administrador = reader.GetInt32(reader.GetOrdinal("id_administrador")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                                Fecha_Creacion = reader.IsDBNull(reader.GetOrdinal("fecha_creacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
                            };
                        }
                    }
                }
            }

            return admin;
        }

        public AdministradorEntity? GetAdministradorByUsuarioId(int userId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM Administrador WHERE id_usuario = @UserId";
                    var p1 = command.CreateParameter();
                    p1.ParameterName = "@UserId";
                    p1.Value = userId;
                    command.Parameters.Add(p1);

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        return new AdministradorEntity
                        {
                            Id_Administrador = reader.GetInt32(reader.GetOrdinal("id_administrador")),
                            Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                            Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                            Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                            Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                            Fecha_Creacion = reader.IsDBNull(reader.GetOrdinal("fecha_creacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_creacion")),
                            Id_Usuario = reader.IsDBNull(reader.GetOrdinal("id_usuario")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario"))
                        };
                    }
                }
            }
        }

        public void AddAdministrador(AdministradorEntity admin)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Administrador (nombre, telefono, email, estado)
                    VALUES (@Nombre, @Telefono, @Email, @Estado)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Nombre", admin.Nombre);
                    AddParameter(command, "@Telefono", admin.Telefono);
                    AddParameter(command, "@Email", admin.Email);
                    AddParameter(command, "@Estado", admin.Estado ?? "activo");

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAdministrador(AdministradorEntity admin)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Administrador
                    SET nombre = @Nombre,
                        telefono = @Telefono,
                        email = @Email,
                        estado = @Estado
                    WHERE id_administrador = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", admin.Id_Administrador);
                    AddParameter(command, "@Nombre", admin.Nombre);
                    AddParameter(command, "@Telefono", admin.Telefono);
                    AddParameter(command, "@Email", admin.Email);
                    AddParameter(command, "@Estado", admin.Estado ?? "activo");

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAdministrador(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM Administrador WHERE id_administrador = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        private void AddParameter(IDbCommand command, string name, object value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            command.Parameters.Add(parameter);
        }
    }
}