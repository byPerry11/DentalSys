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
    public class RecepcionistaService
    {
        private readonly RecepcionistaRepository _recepcionistaRepository;

        public RecepcionistaService(RecepcionistaRepository recepcionistaRepository)
        {
            _recepcionistaRepository = recepcionistaRepository;
        }

        public List<RecepcionistaDTO> GetAllRecepcionistas()
        {
            var entities = _recepcionistaRepository.GetAllRecepcionistas();

            return entities
                .Select(e => new RecepcionistaDTO
                {
                    IdRecepcionista = e.Id_Recepcionista,
                    Nombre = e.Nombre,
                    Telefono = e.Telefono,
                    Email = e.Email
                })
                .ToList();
        }

        public RecepcionistaDTO? GetRecepcionistaById(int id)
        {
            var entity = _recepcionistaRepository.GetRecepcionistaById(id);

            if (entity == null)
                return null;

            return new RecepcionistaDTO
            {
                IdRecepcionista = entity.Id_Recepcionista,
                Nombre = entity.Nombre,
                Telefono = entity.Telefono,
                Email = entity.Email
            };
        }

        public void AddRecepcionista(RecepcionistaDTO dto)
        {
            var entity = new RecepcionistaEntity
            {
                // Id lo genera la base de datos
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email
            };

            _recepcionistaRepository.AddRecepcionista(entity);
        }

        public void UpdateRecepcionista(RecepcionistaDTO dto)
        {
            var entity = new RecepcionistaEntity
            {
                Id_Recepcionista = dto.IdRecepcionista,
                Nombre = dto.Nombre,
                Telefono = dto.Telefono,
                Email = dto.Email
            };

            _recepcionistaRepository.UpdateRecepcionista(entity);
        }

        public void DeleteRecepcionista(int id)
        {
            _recepcionistaRepository.DeleteRecepcionista(id);
        }
    }
}
