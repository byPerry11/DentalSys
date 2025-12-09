using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationLogic.DTOs;
using DataAccess.Connections;
using DataAccess.Repositories;

namespace ApplicationLogic.Services
{
    public class FacturaService
    {
        private readonly ConsultaRepository _consultaRepository;
        private readonly CitaRepository _citaRepository;
        private readonly PacienteRepository _pacienteRepository;
        private readonly TratamientoRepository _tratamientoRepository;
        private readonly UserRepository _userRepository;

        public FacturaService()
        {
            var provider = ConnectionFactory.Create();

            _consultaRepository = new ConsultaRepository(provider);
            _citaRepository = new CitaRepository(provider);
            _pacienteRepository = new PacienteRepository(provider);
            _tratamientoRepository = new TratamientoRepository(provider);
            _userRepository = new UserRepository(provider);
        }

        public FacturaDTO BuildFacturaFromConsulta(ApplicationLogic.DTOs.ConsultaDTO consultaDto)
        {
            if (consultaDto == null)
            {
                throw new ArgumentNullException(nameof(consultaDto));
            }

            var cita = _citaRepository.GetCitaById(consultaDto.CitaId);
            if (cita == null)
                throw new InvalidOperationException("No se encontro la cita asociada a la consulta.");

            var paciente = _pacienteRepository.GetPacienteById(cita.Id_Paciente);
            if (paciente == null)
                throw new InvalidOperationException("No se encontro el paciente asociado.");

            var dentista = _userRepository.GetUserById(cita.Id_Dentista);
            var tratamiento = _tratamientoRepository.GetTratamientoById(consultaDto.TratamientoId);

            var subtotal = consultaDto.Precio;
            var tasaIVA = paciente.Tasa_IVA;
            var iva = Math.Round(subtotal * tasaIVA, 2);
            var total = subtotal + iva;

            var folio = $"F-{consultaDto.Id:000000}";

            var factura = new FacturaDTO
            {
                Folio = folio,
                ConsultaId = consultaDto.Id,
                PacienteId = paciente.Id_Paciente,
                PacienteNombre = paciente.Nombre ?? string.Empty,
                PacienteTelefono = paciente.Telefono ?? string.Empty,
                PacienteEmail = paciente.Email ?? string.Empty,
                DentistaNombre = dentista?.Nombre_Usuario ?? "Desconocido",
                TratamientoNombre = tratamiento?.Nombre ?? consultaDto.TratamientoNombre,
                FechaConsulta = consultaDto.Fecha,
                Subtotal = subtotal,
                TasaIVA = tasaIVA,
                IVA = iva,
                Total = total
            };

            return factura;
        }

    }
}
