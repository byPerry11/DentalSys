using System;
using System.Collections.Generic;
using System.Linq;
using ApplicationLogic.DTOs;
using DataAccess.Connections;
using DataAccess.Repositories;
using DataAccess.Entities;

namespace ApplicationLogic.Services
{
    public class DentistaService
    {
        private readonly DentistaRepository _dentistaRepository;
        private readonly UserRepository _userRepository;

        public DentistaService()
        {
            var provider = ConnectionFactory.Create();
            _dentistaRepository = new DentistaRepository(provider);
            _userRepository = new UserRepository(provider);
        }

        public List<DentistaDTO> GetAllDentistas()
        {
            var entities = _dentistaRepository.GetAllDentistas();

            return entities.Select(d => new DentistaDTO
            {
                Id_Dentista = d.Id_Dentista,
                Id_Usuario = d.Id_Usuario,
                Nombre = d.Nombre,
                Telefono = d.Telefono,
                Especialidad = d.Especialidad,
                Email = d.Email
            }).ToList();
        }
        public List<DentistaDTO> GetDentistasDisponibles()
        {
            var entities = _userRepository.GetDentistasDisponibles();
            return entities.Select(d => new DentistaDTO
            {
                Id_Dentista = d.Id_Dentista,
                Id_Usuario = d.Id_Usuario,
                Nombre = d.Nombre,
                Telefono = d.Telefono,
                Especialidad = d.Especialidad,
                Email = d.Email
            }).ToList();
        }
        public void AddDentista(DentistaDTO dto)
        {
            var entity = new DentistaEntity
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Especialidad = dto.Especialidad,
                Email = dto.Email
            };

            _dentistaRepository.AddDentista(entity);
        }

        public void UpdateDentista(DentistaDTO dto)
        {
            var entity = new DentistaEntity
            {
                Id_Dentista = dto.Id_Dentista,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Especialidad = dto.Especialidad,
                Email = dto.Email
            };

            _dentistaRepository.UpdateDentista(entity);
        }


        public DentistaDTO? GetDentistaByUsuarioId(int usuarioId)
        {
            var entity = _dentistaRepository.GetDentistaByUsuarioId(usuarioId);

            if (entity == null)
                return null;

            return new DentistaDTO
            {
                Id_Dentista = entity.Id_Dentista,
                Id_Usuario = entity.Id_Usuario,
                Nombre = entity.Nombre,
                Telefono = entity.Telefono,
                Especialidad = entity.Especialidad,
                Email = entity.Email
            };
        }

        public void DeleteDentista(int id)
        {
            _dentistaRepository.DeleteDentista(id);
        }

        public void AssignUserToDentista(int dentistaId, int usuarioId)
        {
            _userRepository.AssignUserToDentista(dentistaId, usuarioId);
        }

        public void RemoveUserFromDentista(int dentistaId)
        {
            _userRepository.RemoveUserFromDentista(dentistaId);
        }
    }
}