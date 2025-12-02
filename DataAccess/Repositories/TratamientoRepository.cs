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
    }
}
