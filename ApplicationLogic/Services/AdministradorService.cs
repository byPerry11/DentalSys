using ApplicationLogic.DTOs;
using DataAccess.Connections;
using DataAccess.Entities;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.Services
{
    public class AdministradorService
    {
        private readonly AdministradorRepository _administradorRepository;
        private readonly UserRepository _userRepository;

        public AdministradorService()
        {
            var provider = ConnectionFactory.Create();
            _administradorRepository = new AdministradorRepository(provider);
            _userRepository = new UserRepository(provider);
        }

        public List<AdministradorDTO> GetAllAdministradores()
        {
            var entities = _administradorRepository.GetAllAdministradores();

            return entities.Select(e => new AdministradorDTO
            {
                Id_Administrador = e.Id_Administrador,
                Nombre = e.Nombre,
                Telefono = e.Telefono,
                Email = e.Email,
                Estado = e.Estado,
                Fecha_Creacion = e.Fecha_Creacion,
                Id_Usuario = e.Id_Usuario
            }).ToList();
        }
        public List<AdministradorDTO> GetAdministradoresDisponibles()
        {
            var entities = _userRepository.GetAdministradoresDisponibles();
            return entities.Select(e => new AdministradorDTO
            {
                Id_Administrador = e.Id_Administrador,
                Nombre = e.Nombre,
                Telefono = e.Telefono,
                Email = e.Email,
                Estado = e.Estado,
                Fecha_Creacion = e.Fecha_Creacion,
                Id_Usuario = e.Id_Usuario
            }).ToList();
        }
        private void ValidateAdministrador(AdministradorDTO dto, bool isUpdate = false)
        {
            if (dto == null)
                throw new Exception("El objeto Administrador es nulo.");

            if (isUpdate && dto.Id_Administrador <= 0)
                throw new Exception("ID inválido para actualización.");

            if (string.IsNullOrWhiteSpace(dto.Nombre))
                throw new Exception("El nombre es obligatorio.");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new Exception("El email es obligatorio.");
        }

        public void AddAdministrador(AdministradorDTO dto)
        {
            ValidateAdministrador(dto);

            var entity = new AdministradorEntity
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Estado = dto.Estado ?? "activo"
            };

            _administradorRepository.AddAdministrador(entity);
        }

        public void UpdateAdministrador(AdministradorDTO dto)
        {
            ValidateAdministrador(dto, isUpdate: true);

            var entity = new AdministradorEntity
            {
                Id_Administrador = dto.Id_Administrador,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Estado = dto.Estado ?? "activo"
            };

            _administradorRepository.UpdateAdministrador(entity);
        }

        public void DeleteAdministrador(int id)
        {
            if (id <= 0)
                throw new Exception("ID inválido para eliminar.");

            _administradorRepository.DeleteAdministrador(id);
        }

        public AdministradorDTO? GetAdministradorById(int id)
        {
            var entity = _administradorRepository.GetAdministradorById(id);
            if (entity == null) return null;

            return new AdministradorDTO
            {
                Id_Administrador = entity.Id_Administrador,
                Nombre = entity.Nombre,
                Telefono = entity.Telefono,
                Email = entity.Email,
                Estado = entity.Estado,
                Fecha_Creacion = entity.Fecha_Creacion
            };
        }

        public AdministradorDTO? GetAdministradorByUsuarioId(int userId)
        {
            var entity = _administradorRepository.GetAdministradorByUsuarioId(userId);

            if (entity == null)
                return null;

            return new AdministradorDTO
            {
                Id_Administrador = entity.Id_Administrador,
                Nombre = entity.Nombre,
                Telefono = entity.Telefono,
                Email = entity.Email,
                Estado = entity.Estado,
                Fecha_Creacion = entity.Fecha_Creacion,
                Id_Usuario = entity.Id_Usuario
            };
        }
        public void AssignUserToAdministrador(int adminId, int usuarioId)
        {
            if (adminId <= 0 || usuarioId <= 0)
                throw new Exception("IDs inválidos.");

            _userRepository.AssignUserToAdministrador(adminId, usuarioId);
        }

        public void RemoveUserFromAdministrador(int adminId)
        {
            if (adminId <= 0)
                throw new Exception("ID inválido para remover usuario.");

            _userRepository.RemoveUserFromAdministrador(adminId);
        }

    }
}