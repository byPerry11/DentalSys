using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLogic.DTOs;
using DataAccess.Connections;
using DataAccess.Repositories;

namespace ApplicationLogic.Services
{
    public class PacienteService
    {
        private readonly PacienteRepository _pacienteRepository;

        public PacienteService()
        {
            var provider = ConnectionFactory.Create();
            _pacienteRepository = new PacienteRepository(provider);
        }

        public List<PacienteDTO> GetAllPacientes()
        {
            var entities = _pacienteRepository.GetAllPacientes();
            return entities.Select(p => new PacienteDTO
            {
                Id = p.Id_Paciente,
                Nombre = p.Nombre,
                Telefono = p.Telefono,
                ContactoEmergencia = p.Contacto_Emergencia,
                NumeroEmergencia = p.Numero_Emergencia,
                Email = p.Email,
                FechaNacimiento = p.Fecha_Nacimiento,
                FechaRegistro = p.Fecha_Registro,
                Facturada = p.Facturada,
                TasaIVA = p.Tasa_IVA
            }).ToList();
        }

        public void AddPaciente(PacienteDTO dto)
        {
            var entity = new DataAccess.Entities.PacienteEntity
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Contacto_Emergencia = dto.ContactoEmergencia,
                Numero_Emergencia = dto.NumeroEmergencia,
                Email = dto.Email,
                Fecha_Nacimiento = dto.FechaNacimiento,
                Fecha_Registro = DateTime.Now, // Set registration date to now
                Facturada = dto.Facturada,
                Tasa_IVA = dto.TasaIVA
            };
            _pacienteRepository.AddPaciente(entity);
        }

        public void UpdatePaciente(PacienteDTO dto)
        {
            var entity = new DataAccess.Entities.PacienteEntity
            {
                Id_Paciente = dto.Id,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Contacto_Emergencia = dto.ContactoEmergencia,
                Numero_Emergencia = dto.NumeroEmergencia,
                Email = dto.Email,
                Fecha_Nacimiento = dto.FechaNacimiento,
                Facturada = dto.Facturada,
                Tasa_IVA = dto.TasaIVA
            };
            _pacienteRepository.UpdatePaciente(entity);
        }

        public void DeletePaciente(int id)
        {
            _pacienteRepository.DeletePaciente(id);
        }
    }
}
