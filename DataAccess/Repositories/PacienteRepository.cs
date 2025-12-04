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
    public class PacienteRepository
    {
        private readonly IConnectionProvider _provider;

        public PacienteRepository(IConnectionProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public List<PacienteEntity> GetAllPacientes()
        {
            var pacientes = new List<PacienteEntity>();

            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "SELECT * FROM paciente";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var paciente = new PacienteEntity
                            {
                                Id_Paciente = reader.GetInt32(reader.GetOrdinal("Id_Paciente")),
                                Nombre = reader.IsDBNull(reader.GetOrdinal("Nombre")) ? null : reader.GetString(reader.GetOrdinal("Nombre")),
                                Telefono = reader.IsDBNull(reader.GetOrdinal("Telefono")) ? null : reader.GetString(reader.GetOrdinal("Telefono")),
                                Contacto_Emergencia = reader.IsDBNull(reader.GetOrdinal("Contacto_Emergencia")) ? null : reader.GetString(reader.GetOrdinal("Contacto_Emergencia")),
                                Numero_Emergencia = reader.IsDBNull(reader.GetOrdinal("Numero_Emergencia")) ? null : reader.GetString(reader.GetOrdinal("Numero_Emergencia")),
                                Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                                Fecha_Nacimiento = reader.GetDateTime(reader.GetOrdinal("Fecha_Nacimiento")),
                                Fecha_Registro = reader.GetDateTime(reader.GetOrdinal("Fecha_Registro")),
                                Facturada = reader.IsDBNull(reader.GetOrdinal("Facturada")) ? null : reader.GetString(reader.GetOrdinal("Facturada")),
                                Tasa_IVA = reader.GetDecimal(reader.GetOrdinal("Tasa_IVA"))
                            };
                            pacientes.Add(paciente);
                        }
                    }
                }
            }
            return pacientes;
        }

        public void AddPaciente(PacienteEntity paciente)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    INSERT INTO paciente (Nombre, Telefono, Contacto_Emergencia, Numero_Emergencia, Email, Fecha_Nacimiento, Fecha_Registro, Facturada, Tasa_IVA)
                    VALUES (@Nombre, @Telefono, @Contacto_Emergencia, @Numero_Emergencia, @Email, @Fecha_Nacimiento, @Fecha_Registro, @Facturada, @Tasa_IVA)";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Nombre", paciente.Nombre);
                    AddParameter(command, "@Telefono", paciente.Telefono);
                    AddParameter(command, "@Contacto_Emergencia", paciente.Contacto_Emergencia);
                    AddParameter(command, "@Numero_Emergencia", paciente.Numero_Emergencia);
                    AddParameter(command, "@Email", paciente.Email);
                    AddParameter(command, "@Fecha_Nacimiento", paciente.Fecha_Nacimiento);
                    AddParameter(command, "@Fecha_Registro", paciente.Fecha_Registro);
                    AddParameter(command, "@Facturada", paciente.Facturada);
                    AddParameter(command, "@Tasa_IVA", paciente.Tasa_IVA);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePaciente(PacienteEntity paciente)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = @"
                    UPDATE paciente
                    SET Nombre = @Nombre,
                        Telefono = @Telefono,
                        Contacto_Emergencia = @Contacto_Emergencia,
                        Numero_Emergencia = @Numero_Emergencia,
                        Email = @Email,
                        Fecha_Nacimiento = @Fecha_Nacimiento,
                        Facturada = @Facturada,
                        Tasa_IVA = @Tasa_IVA
                    WHERE Id_Paciente = @Id_Paciente";

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = query;
                    AddParameter(command, "@Id_Paciente", paciente.Id_Paciente);
                    AddParameter(command, "@Nombre", paciente.Nombre);
                    AddParameter(command, "@Telefono", paciente.Telefono);
                    AddParameter(command, "@Contacto_Emergencia", paciente.Contacto_Emergencia);
                    AddParameter(command, "@Numero_Emergencia", paciente.Numero_Emergencia);
                    AddParameter(command, "@Email", paciente.Email);
                    AddParameter(command, "@Fecha_Nacimiento", paciente.Fecha_Nacimiento);
                    AddParameter(command, "@Facturada", paciente.Facturada);
                    AddParameter(command, "@Tasa_IVA", paciente.Tasa_IVA);

                    command.ExecuteNonQuery();
                }
            }
        }

        public void DeletePaciente(int id)
        {
            using (var connection = _provider.CreateConnection())
            {
                connection.Open();
                string query = "DELETE FROM paciente WHERE Id_Paciente = @Id";

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
