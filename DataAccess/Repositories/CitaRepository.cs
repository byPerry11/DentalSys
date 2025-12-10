using System;
using System.Collections.Generic;
using System.Data;
using DataAccess.Connections;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    public class CitaRepository
    {
        private readonly IConnectionProvider _provider;

        public CitaRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public List<CitaEntity> GetUpcomingCitas()
        {
            var citas = new List<CitaEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                // Get appointments from today onwards
                string query = "SELECT * FROM cita WHERE Fecha_hora >= @FechaActual ORDER BY Fecha_hora ASC";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@FechaActual", DateTime.Today);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cita = new CitaEntity
                            {
                                Id_cita = reader.GetInt32(reader.GetOrdinal("Id_cita")),
                                Id_Paciente = reader.GetInt32(reader.GetOrdinal("Id_Paciente")),
                                Id_Dentista = reader.GetInt32(reader.GetOrdinal("Id_Dentista")),
                                Fecha_hora = reader.GetDateTime(reader.GetOrdinal("Fecha_hora")),
                                Estatus_Cita = reader.IsDBNull(reader.GetOrdinal("Estatus_Cita")) ? null : reader.GetString(reader.GetOrdinal("Estatus_Cita"))
                            };
                            citas.Add(cita);
                        }
                    }
                }
            }
            return citas;
        }

        public void AddCita(CitaEntity cita)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO cita (Id_Paciente, Id_Dentista, Fecha_hora, Estatus_Cita)
                    VALUES (@Id_Paciente, @Id_Dentista, @Fecha_hora, @Estatus_Cita)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id_Paciente", cita.Id_Paciente);
                    AddParameter(command, "@Id_Dentista", cita.Id_Dentista);
                    AddParameter(command, "@Fecha_hora", cita.Fecha_hora);
                    AddParameter(command, "@Estatus_Cita", cita.Estatus_Cita);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateCita(CitaEntity cita)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE cita
                    SET Id_Paciente = @Id_Paciente,
                        Id_Dentista = @Id_Dentista,
                        Fecha_hora = @Fecha_hora,
                        Estatus_Cita = @Estatus_Cita
                    WHERE Id_cita = @Id_cita";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id_cita", cita.Id_cita);
                    AddParameter(command, "@Id_Paciente", cita.Id_Paciente);
                    AddParameter(command, "@Id_Dentista", cita.Id_Dentista);
                    AddParameter(command, "@Fecha_hora", cita.Fecha_hora);
                    AddParameter(command, "@Estatus_Cita", cita.Estatus_Cita);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteCita(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM cita WHERE Id_cita = @Id";

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

        public CitaEntity GetCitaById(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM cita WHERE Id_cita = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CitaEntity
                            {
                                Id_cita = reader.GetInt32(reader.GetOrdinal("Id_cita")),
                                Id_Paciente = reader.GetInt32(reader.GetOrdinal("Id_Paciente")),
                                Id_Dentista = reader.GetInt32(reader.GetOrdinal("Id_Dentista")),
                                Fecha_hora = reader.GetDateTime(reader.GetOrdinal("Fecha_hora")),
                                Estatus_Cita = reader.IsDBNull(reader.GetOrdinal("Estatus_Cita")) ? null : reader.GetString(reader.GetOrdinal("Estatus_Cita"))
                            };
                        }
                    }
                }
            }
            return null;
        }

        public List<CitaEntity> GetAllCitas()
        {
            var citas = new List<CitaEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM cita ORDER BY Fecha_hora DESC";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cita = new CitaEntity
                            {
                                Id_cita = reader.GetInt32(reader.GetOrdinal("Id_cita")),
                                Id_Paciente = reader.GetInt32(reader.GetOrdinal("Id_Paciente")),
                                Id_Dentista = reader.GetInt32(reader.GetOrdinal("Id_Dentista")),
                                Fecha_hora = reader.GetDateTime(reader.GetOrdinal("Fecha_hora")),
                                Estatus_Cita = reader.IsDBNull(reader.GetOrdinal("Estatus_Cita")) ? null : reader.GetString(reader.GetOrdinal("Estatus_Cita"))
                            };
                            citas.Add(cita);
                        }
                    }
                }
            }
            return citas;
        }
    }
}
