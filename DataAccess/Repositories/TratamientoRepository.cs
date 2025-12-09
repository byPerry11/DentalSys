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
    public class TratamientoRepository
    {
        private readonly IConnectionProvider _provider;

        public TratamientoRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public List<TratamientoEntity> GetAllTratamientos()
        {
            var tratamientos = new List<TratamientoEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT Id_Tratamiento, Nombre, Precio, Descripcion FROM tratamiento";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var tratamiento = new TratamientoEntity
                            {
                                Id_Tratamiento = reader.GetInt32(reader.GetOrdinal("Id_Tratamiento")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion"))
                            };
                            tratamientos.Add(tratamiento);
                        }
                    }
                }
            }

            return tratamientos;
        }

        public void AddTratamiento(TratamientoEntity tratamiento)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "INSERT INTO tratamiento (Nombre, Precio, Descripcion) VALUES (@Nombre, @Precio, @Descripcion)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var paramNombre = command.CreateParameter();
                    paramNombre.ParameterName = "@Nombre";
                    paramNombre.Value = tratamiento.Nombre;
                    command.Parameters.Add(paramNombre);

                    var paramPrecio = command.CreateParameter();
                    paramPrecio.ParameterName = "@Precio";
                    paramPrecio.Value = tratamiento.Precio;
                    command.Parameters.Add(paramPrecio);

                    var paramDesc = command.CreateParameter();
                    paramDesc.ParameterName = "@Descripcion";
                    paramDesc.Value = (object)tratamiento.Descripcion ?? DBNull.Value;
                    command.Parameters.Add(paramDesc);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateTratamiento(TratamientoEntity tratamiento)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "UPDATE tratamiento SET Nombre = @Nombre, Precio = @Precio, Descripcion = @Descripcion WHERE Id_Tratamiento = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@Id";
                    paramId.Value = tratamiento.Id_Tratamiento;
                    command.Parameters.Add(paramId);

                    var paramNombre = command.CreateParameter();
                    paramNombre.ParameterName = "@Nombre";
                    paramNombre.Value = tratamiento.Nombre;
                    command.Parameters.Add(paramNombre);

                    var paramPrecio = command.CreateParameter();
                    paramPrecio.ParameterName = "@Precio";
                    paramPrecio.Value = tratamiento.Precio;
                    command.Parameters.Add(paramPrecio);

                    var paramDesc = command.CreateParameter();
                    paramDesc.ParameterName = "@Descripcion";
                    paramDesc.Value = (object)tratamiento.Descripcion ?? DBNull.Value;
                    command.Parameters.Add(paramDesc);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTratamiento(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM tratamiento WHERE Id_Tratamiento = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@Id";
                    paramId.Value = id;
                    command.Parameters.Add(paramId);

                    command.ExecuteNonQuery();
                }
            }
        }

        public TratamientoEntity GetTratamientoById(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT Id_Tratamiento, Nombre, Precio, Descripcion FROM tratamiento WHERE Id_Tratamiento = @Id";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    var paramId = command.CreateParameter();
                    paramId.ParameterName = "@Id";
                    paramId.Value = id;
                    command.Parameters.Add(paramId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new TratamientoEntity
                            {
                                Id_Tratamiento = reader.GetInt32(reader.GetOrdinal("Id_Tratamiento")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                                Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString(reader.GetOrdinal("Descripcion"))
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
