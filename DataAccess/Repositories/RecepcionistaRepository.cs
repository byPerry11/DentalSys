using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            var list = new List<RecepcionistaEntity>();

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
                            var r = new RecepcionistaEntity
                            {
                                Id_Recepcionista = reader.GetInt32(reader.GetOrdinal("id_recepcionista")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("nombre")) ? null : reader.GetString(reader.GetOrdinal("nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("telefono")) ? null : reader.GetString(reader.GetOrdinal("telefono")),
                                Email = reader.IsDBNull(reader.GetOrdinal("email")) ? null : reader.GetString(reader.GetOrdinal("email")),
                                Estado = reader.IsDBNull(reader.GetOrdinal("estado")) ? null : reader.GetString(reader.GetOrdinal("estado")),
                                Fecha_Creacion = reader.IsDBNull(reader.GetOrdinal("fecha_creacion")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("fecha_creacion"))
                            };

                            list.Add(r);
                        }
                    }
                }
            }

            return list;
        }

        public RecepcionistaEntity? GetRecepcionistaById(int id)
        {
            RecepcionistaEntity? resp = null;

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM Recepcionista WHERE id_recepcionista = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", id);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            resp = new RecepcionistaEntity
                            {
                                Id_Recepcionista = reader.GetInt32(reader.GetOrdinal("id_recepcionista")),
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

            return resp;
        }

        public void AddRecepcionista(RecepcionistaEntity r)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO Recepcionista (nombre, telefono, email, estado)
                    VALUES (@Nombre, @Telefono, @Email, @Estado)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Nombre", r.Nombre);
                    AddParameter(command, "@Telefono", r.Telefono);
                    AddParameter(command, "@Email", r.Email);
                    AddParameter(command, "@Estado", r.Estado ?? "activo");

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateRecepcionista(RecepcionistaEntity r)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE Recepcionista
                    SET nombre = @Nombre,
                        telefono = @Telefono,
                        email = @Email,
                        estado = @Estado
                    WHERE id_recepcionista = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id", r.Id_Recepcionista);
                    AddParameter(command, "@Nombre", r.Nombre);
                    AddParameter(command, "@Telefono", r.Telefono);
                    AddParameter(command, "@Email", r.Email);
                    AddParameter(command, "@Estado", r.Estado ?? "activo");

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteRecepcionista(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM Recepcionista WHERE id_recepcionista = @Id";

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