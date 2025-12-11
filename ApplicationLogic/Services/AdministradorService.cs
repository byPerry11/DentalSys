using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLogic.DTOs;
using DataAccess.Entities;
using DataAccess.Repositories;

namespace ApplicationLogic.Services
{
    public class AdministradorService
    {
        private readonly AdministradorRepository _adminRepository;

        public AdministradorService(AdministradorRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public List<AdministradorDTO> GetAllAdministradores()
        {
            var entities = _adminRepository.GetAllAdministradores();

            return entities
                .Select(e => new AdministradorDTO
                {
                    IdAdministrador = e.Id_Administrador,
                    Nombre = e.Nombre,
                    Telefono = e.Telefono,
                    Email = e.Email
                })
                .ToList();
        }

        public AdministradorDTO? GetAdministradorById(int id)
        {
            var entity = _adminRepository.GetAdministradorById(id);

            if (entity == null)
                return null;

            return new AdministradorDTO
            {
                IdAdministrador = entity.Id_Administrador,
                Nombre = entity.Nombre,
                Telefono = entity.Telefono,
                Email = entity.Email
            };
        }

        public void AddAdministrador(AdministradorDTO dto)
        {
            var entity = new AdministradorEntity
            {
                // Id lo genera la base
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email
            };

            _adminRepository.AddAdministrador(entity);
        }

        public void UpdateAdministrador(AdministradorDTO dto)
        {
            var entity = new AdministradorEntity
            {
                Id_Administrador = dto.IdAdministrador,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email
            };

            _adminRepository.UpdateAdministrador(entity);
        }

        public void DeleteAdministrador(int id)
        {
            _adminRepository.DeleteAdministrador(id);
        }
    }
}
