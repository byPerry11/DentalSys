using System;
using System.Collections.Generic;
using System.Data;
using DataAccess.Connections;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    public class ConsultaRepository
    {
        private readonly IConnectionProvider _provider;

        public ConsultaRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public List<ConsultaEntity> GetAllConsultas()
        {
            var consultas = new List<ConsultaEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM consulta ORDER BY Id_Consulta DESC";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var consulta = new ConsultaEntity
                            {
                                Id_Consulta = reader.GetInt32(reader.GetOrdinal("Id_Consulta")),
                                Id_Tratamiento = reader.GetInt32(reader.GetOrdinal("Id_Tratamiento")),
                                Id_Cita = reader.GetInt32(reader.GetOrdinal("Id_Cita")),
                                Precio_Consulta = reader.GetDecimal(reader.GetOrdinal("Precio_Consulta")),
                                Notas = reader.IsDBNull(reader.GetOrdinal("Notas")) ? null : reader.GetString(reader.GetOrdinal("Notas"))
                            };
                            consultas.Add(consulta);
                        }
                    }
                }
            }
            return consultas;
        }

        public void AddConsulta(ConsultaEntity consulta)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"INSERT INTO consulta (Id_Tratamiento, Id_Cita, Precio_Consulta, Notas)
                                 VALUES (@IdTratamiento, @IdCita, @Precio, @Notas)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@IdTratamiento", consulta.Id_Tratamiento);
                    AddParameter(command, "@IdCita", consulta.Id_Cita);
                    AddParameter(command, "@Precio", consulta.Precio_Consulta);
                    AddParameter(command, "@Notas", consulta.Notas);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool ExistsConsultaByCitaId(int citaId)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM consulta WHERE Id_Cita = @CitaId";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@CitaId", citaId);

                    var result = command.ExecuteScalar();
                    return Convert.ToInt32(result) > 0;
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
