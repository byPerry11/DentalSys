using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicationLogic.DTOs;
using DataAccess.Connections;
using DataAccess.Repositories;

namespace ApplicationLogic.Services
{
    public class TratamientoService
    {
        private readonly TratamientoRepository _tratamientoRepository;

        public TratamientoService()
        {
            var provider = ConnectionFactory.Create();
            _tratamientoRepository = new TratamientoRepository(provider);
        }

        public List<TratamientoDTO> GetAllTratamientos()
        {
            var entities = _tratamientoRepository.GetAllTratamientos();
            return entities.Select(t => new TratamientoDTO
            {
                Id = t.Id_Tratamiento,
                Nombre = t.Nombre,
                Precio = t.Precio,
                Descripcion = t.Descripcion
            }).ToList();
        }

        public void AddTratamiento(TratamientoDTO dto)
        {
            var entity = new DataAccess.Entities.TratamientoEntity
            {
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Descripcion = dto.Descripcion
            };
            _tratamientoRepository.AddTratamiento(entity);
        }

        public void UpdateTratamiento(TratamientoDTO dto)
        {
            var entity = new DataAccess.Entities.TratamientoEntity
            {
                Id_Tratamiento = dto.Id,
                Nombre = dto.Nombre,
                Precio = dto.Precio,
                Descripcion = dto.Descripcion
            };
            _tratamientoRepository.UpdateTratamiento(entity);
        }

        public void DeleteTratamiento(int id)
        {
            _tratamientoRepository.DeleteTratamiento(id);
        }
    }
}
