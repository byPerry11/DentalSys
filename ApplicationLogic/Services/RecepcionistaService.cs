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
    public class RecepcionistaService
    {
        private readonly RecepcionistaRepository _recepcionistaRepository;

        public RecepcionistaService()
        {
            var provider = ConnectionFactory.Create();
            _recepcionistaRepository = new RecepcionistaRepository(provider);
        }

        public List<RecepcionistaDTO> GetAllRecepcionistas()
        {
            var entities = _recepcionistaRepository.GetAllRecepcionistas();
            return entities.Select(e => new RecepcionistaDTO
            {
                Id_Recepcionista = e.Id_Recepcionista,
                Nombre = e.Nombre,
                Telefono = e.Telefono,
                Email = e.Email,
                Estado = e.Estado,
                Fecha_Creacion = e.Fecha_Creacion
            }).ToList();
        }

        public void AddRecepcionista(RecepcionistaDTO dto)
        {
            var entity = new RecepcionistaEntity
            {
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Estado = dto.Estado ?? "activo"
            };

            _recepcionistaRepository.AddRecepcionista(entity);
        }

        public void UpdateRecepcionista(RecepcionistaDTO dto)
        {
            var entity = new RecepcionistaEntity
            {
                Id_Recepcionista = dto.Id_Recepcionista,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Estado = dto.Estado ?? "activo"
            };

            _recepcionistaRepository.UpdateRecepcionista(entity);
        }

        public void DeleteRecepcionista(int id)
        {
            _recepcionistaRepository.DeleteRecepcionista(id);
        }

        public RecepcionistaDTO? GetRecepcionistaById(int id)
        {
            var e = _recepcionistaRepository.GetRecepcionistaById(id);
            if (e == null) return null;
            return new RecepcionistaDTO
            {
                Id_Recepcionista = e.Id_Recepcionista,
                Nombre = e.Nombre,
                Telefono = e.Telefono,
                Email = e.Email,
                Estado = e.Estado,
                Fecha_Creacion = e.Fecha_Creacion
            };
        }
    }
}
