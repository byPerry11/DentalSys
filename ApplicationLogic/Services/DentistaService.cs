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

        public DentistaService()
        {
            var provider = ConnectionFactory.Create();
            _dentistaRepository = new DentistaRepository(provider);
        }

        public List<DentistaDTO> GetAllDentistas()
        {
            var entities = _dentistaRepository.GetAllDentistas();

            return entities.Select(d => new DentistaDTO
            {
                Id_Dentista = d.Id_Dentista,
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


        public void DeleteDentista(int id)
        {
            _dentistaRepository.DeleteDentista(id);
        }
    }
}