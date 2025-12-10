using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationLogic.DTOs;
using DataAccess.Connections;
using DataAccess.Repositories;

namespace ApplicationLogic.Services
{
    public class CitaService
    {
        private readonly CitaRepository _citaRepository;
        private readonly PacienteRepository _pacienteRepository;
        private readonly DentistaRepository _dentistaRepository;

        public CitaService()
        {
            var provider = ConnectionFactory.Create();
            _citaRepository = new CitaRepository(provider);
            _pacienteRepository = new PacienteRepository(provider);
            _dentistaRepository = new DentistaRepository(provider);
        }

        public List<CitaDTO> GetUpcomingCitas()
        {
            var entities = _citaRepository.GetUpcomingCitas();
            return MapToDTOs(entities);
        }

        public List<CitaDTO> GetAllCitas()
        {
            var entities = _citaRepository.GetAllCitas();
            return MapToDTOs(entities);
        }

        private List<CitaDTO> MapToDTOs(List<DataAccess.Entities.CitaEntity> entities)
        {
            var dtos = new List<CitaDTO>();

            foreach (var entity in entities)
            {
                var paciente = _pacienteRepository.GetPacienteById(entity.Id_Paciente);
                var dentista = _dentistaRepository.GetDentistaById(entity.Id_Dentista);

                dtos.Add(new CitaDTO
                {
                    Id = entity.Id_cita,
                    PacienteId = entity.Id_Paciente,
                    PacienteNombre = paciente.Nombre ?? string.Empty,
                    DentistaId = entity.Id_Dentista,
                    DentistaNombre = dentista.Nombre ?? string.Empty,
                    FechaHora = entity.Fecha_hora,
                    Estatus = entity.Estatus_Cita
                });
            }
            return dtos;
        }

        public void AddCita(CitaDTO dto)
        {
            var entity = new DataAccess.Entities.CitaEntity
            {
                Id_Paciente = dto.PacienteId,
                Id_Dentista = dto.DentistaId,
                Fecha_hora = dto.FechaHora,
                Estatus_Cita = dto.Estatus
            };
            _citaRepository.AddCita(entity);
        }

        public void UpdateCita(CitaDTO dto)
        {
            var entity = new DataAccess.Entities.CitaEntity
            {
                Id_cita = dto.Id,
                Id_Paciente = dto.PacienteId,
                Id_Dentista = dto.DentistaId,
                Fecha_hora = dto.FechaHora,
                Estatus_Cita = dto.Estatus
            };
            _citaRepository.UpdateCita(entity);
        }

        public void DeleteCita(int id)
        {
            _citaRepository.DeleteCita(id);
        }
    }
}
