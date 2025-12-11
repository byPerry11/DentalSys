using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationLogic.DTOs;
using DataAccess.Connections;
using DataAccess.Repositories;

namespace ApplicationLogic.Services
{
    public class ConsultaService
    {
        private readonly ConsultaRepository _consultaRepository;
        private readonly TratamientoRepository _tratamientoRepository;
        private readonly CitaRepository _citaRepository;
        private readonly PacienteRepository _pacienteRepository;
        private readonly DentistaRepository _dentistaRepository;

        public ConsultaService()
        {
            var provider = ConnectionFactory.Create();
            _consultaRepository = new ConsultaRepository(provider);
            _tratamientoRepository = new TratamientoRepository(provider);
            _citaRepository = new CitaRepository(provider);
            _pacienteRepository = new PacienteRepository(provider);
            _dentistaRepository = new DentistaRepository(provider);
        }

        public List<ConsultaDTO> GetAllConsultas()
        {
            var entities = _consultaRepository.GetAllConsultas();
            var dtos = new List<ConsultaDTO>();

            foreach (var entity in entities)
            {
                var dto = new ConsultaDTO
                {
                    Id = entity.Id_Consulta,
                    TratamientoId = entity.Id_Tratamiento,
                    CitaId = entity.Id_Cita,
                    Precio = entity.Precio_Consulta,
                    Notas = entity.Notas
                };

                // Get Tratamiento info
                var tratamiento = _tratamientoRepository.GetTratamientoById(entity.Id_Tratamiento);
                dto.TratamientoNombre = tratamiento?.Nombre ?? "Desconocido";

                // Get Cita info to get Patient and Dentist
                var cita = _citaRepository.GetCitaById(entity.Id_Cita);
                if (cita != null)
                {
                    dto.Fecha = cita.Fecha_hora;

                    var paciente = _pacienteRepository.GetPacienteById(cita.Id_Paciente);
                    dto.PacienteNombre = paciente?.Nombre ?? "Desconocido";

                    var dentista = _dentistaRepository.GetDentistaById(cita.Id_Dentista);
                    dto.DentistaNombre = dentista?.Nombre ?? "Desconocido";
                }
                else
                {
                    dto.PacienteNombre = "Cita no encontrada";
                    dto.DentistaNombre = "Desconocido";
                }

                dtos.Add(dto);
            }

            return dtos;
        }

        public void AddConsulta(ConsultaDTO dto)
        {
            var entity = new DataAccess.Entities.ConsultaEntity
            {
                Id_Tratamiento = dto.TratamientoId,
                Id_Cita = dto.CitaId,
                Precio_Consulta = dto.Precio,
                Notas = dto.Notas
            };
            _consultaRepository.AddConsulta(entity);
        }

        public bool ExistsConsultaByCitaId(int citaId)
        {
            return _consultaRepository.ExistsConsultaByCitaId(citaId);
        }
    }
}
