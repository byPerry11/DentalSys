using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DataAccess.Connections;
using DataAccess.Entities;

namespace DataAccess.Repositories
{
    public class RecepcionistaRepository
    {
        private readonly IConnectionProvider _provider;

        public RecepcionistaRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public List<RecepcionistaEntity> GetAllRecepcionistas()
        {
            var recepcionistas = new List<RecepcionistaEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Recepcionista";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var recepcionista = new RecepcionistaEntity
                            {
                                Id_Recepcionista = reader.GetInt32(reader.GetOrdinal("Id_Recepcionista")),
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

                            recepcionistas.Add(recepcionista);
                        }
                    }
                }
            }

            return recepcionistas;
        }

        public RecepcionistaEntity? GetRecepcionistaById(int id)
        {
            RecepcionistaEntity? recepcionista = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Recepcionista WHERE Id_Recepcionista = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            recepcionista = new RecepcionistaEntity
                            {
                                Id_Recepcionista = reader.GetInt32(reader.GetOrdinal("Id_Recepcionista")),
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

            return recepcionista;
        }

        public void AddRecepcionista(RecepcionistaEntity recepcionista)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Recepcionista (Nombre, Telefono, Email)
                    VALUES (@Nombre, @Telefono, @Email)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    AddParameter(command, "@Nombre", recepcionista.Nombre);
                    AddParameter(command, "@Telefono", recepcionista.Telefono);
                    AddParameter(command, "@Email", recepcionista.Email);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRecepcionista(RecepcionistaEntity recepcionista)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Recepcionista
                    SET Nombre = @Nombre,
                        Telefono = @Telefono,
                        Email = @Email
                    WHERE Id_Recepcionista = @Id_Recepcionista";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    AddParameter(command, "@Id_Recepcionista", recepcionista.Id_Recepcionista);
                    AddParameter(command, "@Nombre", recepcionista.Nombre);
                    AddParameter(command, "@Telefono", recepcionista.Telefono);
                    AddParameter(command, "@Email", recepcionista.Email);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteRecepcionista(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM Recepcionista WHERE Id_Recepcionista = @Id";

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
