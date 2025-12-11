using System;
using System.Collections.Generic;
using System.Data;
using DataAccess.Connections;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    public class DentistaRepository
    {
        private readonly IConnectionProvider _provider;

        public DentistaRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
        public List<DentistaEntity> GetAllDentistas()
        {
            var dentistas = new List<DentistaEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Dentista";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var dentista = new DentistaEntity
                            {
                                Id_Dentista = reader.GetInt32(reader.GetOrdinal("Id_Dentista")),
                                Id_Usuario = reader.IsDBNull(reader.GetOrdinal("id_usuario")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? null : reader.GetString(reader.GetOrdinal("Especialidad")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"))
                            };
                            dentistas.Add(dentista);
                        }
                    }
                }
            }

            return dentistas;
        }


        public DentistaEntity? GetDentistaById(int id)
        {
            DentistaEntity? dentista = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Dentista WHERE Id_Dentista = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dentista = new DentistaEntity
                            {
                                Id_Dentista = reader.GetInt32(reader.GetOrdinal("Id_Dentista")),
                                Id_Usuario = reader.IsDBNull(reader.GetOrdinal("id_usuario")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? null : reader.GetString(reader.GetOrdinal("Especialidad")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"))
                            };
                        }
                    }
                }
            }

            return dentista;
        }

        public void AddDentista(DentistaEntity dentista)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Dentista (Nombre, Telefono, Especialidad, Email)
                    VALUES (@Nombre, @Telefono, @Especialidad, @Email)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Nombre", dentista.Nombre);
                    AddParameter(command, "@Telefono", dentista.Telefono);
                    AddParameter(command, "@Especialidad", dentista.Especialidad);
                    AddParameter(command, "@Email", dentista.Email);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateDentista(DentistaEntity dentista)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Dentista
                    SET Nombre = @Nombre,
                        Telefono = @Telefono,
                        Especialidad = @Especialidad,
                        Email = @Email
                    WHERE Id_Dentista = @Id_Dentista";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id_Dentista", dentista.Id_Dentista);
                    AddParameter(command, "@Nombre", dentista.Nombre);
                    AddParameter(command, "@Telefono", dentista.Telefono);
                    AddParameter(command, "@Especialidad", dentista.Especialidad);
                    AddParameter(command, "@Email", dentista.Email);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteDentista(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM Dentista WHERE Id_Dentista = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public DentistaEntity? GetDentistaByUsuarioId(int usuarioId)
        {
            DentistaEntity? dentista = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Dentista WHERE id_usuario = @UsuarioId";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@UsuarioId", usuarioId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dentista = new DentistaEntity
                            {
                                Id_Dentista = reader.GetInt32(reader.GetOrdinal("Id_Dentista")),
                                Id_Usuario = reader.IsDBNull(reader.GetOrdinal("id_usuario")) ? null : reader.GetInt32(reader.GetOrdinal("id_usuario")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Especialidad = reader.IsDBNull(reader.GetOrdinal("Especialidad")) ? null : reader.GetString(reader.GetOrdinal("Especialidad")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email"))
                            };
                        }
                    }
                }
            }

            return dentista;
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
