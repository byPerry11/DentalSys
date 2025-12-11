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
            var administradores = new List<AdministradorEntity>();

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
                            var admin = new AdministradorEntity
                            {
                                Id_Administrador = reader.GetInt32(reader.GetOrdinal("Id_Administrador")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Telefono")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Email"))
                            };

                            administradores.Add(admin);
                        }
                    }
                }
            }

            return administradores;
        }

        public AdministradorEntity? GetAdministradorById(int id)
        {
            AdministradorEntity? admin = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Administrador WHERE Id_Administrador = @Id";

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
                                Id_Administrador = reader.GetInt32(reader.GetOrdinal("Id_Administrador")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Telefono")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email"))
                                    ? null
                                    : reader.GetString(reader.GetOrdinal("Email"))
                            };
                        }
                    }
                }
            }

            return admin;
        }

        public void AddAdministrador(AdministradorEntity admin)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Administrador (Nombre, Telefono, Email)
                    VALUES (@Nombre, @Telefono, @Email)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    AddParameter(command, "@Nombre", admin.Nombre);
                    AddParameter(command, "@Telefono", admin.Telefono);
                    AddParameter(command, "@Email", admin.Email);

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
                    SET Nombre = @Nombre,
                        Telefono = @Telefono,
                        Email = @Email
                    WHERE Id_Administrador = @Id_Administrador";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    AddParameter(command, "@Id_Administrador", admin.Id_Administrador);
                    AddParameter(command, "@Nombre", admin.Nombre);
                    AddParameter(command, "@Telefono", admin.Telefono);
                    AddParameter(command, "@Email", admin.Email);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteAdministrador(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM Administrador WHERE Id_Administrador = @Id";

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
