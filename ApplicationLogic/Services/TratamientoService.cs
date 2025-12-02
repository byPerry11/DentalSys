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
    }
}
