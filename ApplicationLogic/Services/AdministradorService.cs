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

        public AdministradorService()
        {
            var provider = ConnectionFactory.Create();
            _administradorRepository = new AdministradorRepository(provider);
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
                Fecha_Creacion = e.Fecha_Creacion
            }).ToList();
        }

        public void AddAdministrador(AdministradorDTO dto)
        {
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
    }
}