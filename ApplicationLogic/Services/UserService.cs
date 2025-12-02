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
    public class UserService
    {
        private readonly UserRepository _userRepository;

        public UserService()
        {
            var provider = ConnectionFactory.Create();
            _userRepository = new UserRepository(provider);
        }

        public List<UserDTO> GetAllUsers()
        {
            var entities = _userRepository.GetAllUsers();
            return entities.Select(u => new UserDTO
            {
                Id = u.Id_Usuario,
                Username = u.Nombre_Usuario,
                Role = u.Rol,
                FechaCreacion = u.Fecha_Creacion
            }).ToList();
        }
    }
}
