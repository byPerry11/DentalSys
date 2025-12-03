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

        public void DeleteUser(int id)
        {
            _userRepository.DeleteUser(id);
        }

        public void CreateUser(UserDTO userDto, string password)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var entity = new DataAccess.Entities.UserEntity
            {
                Nombre_Usuario = userDto.Username,
                Pasword_Hash = passwordHash,
                Rol = userDto.Role,
                Fecha_Creacion = DateTime.Now
            };

            _userRepository.AddUser(entity);
        }

        public void UpdateUser(UserDTO userDto, string? newPassword)
        {
            var entity = new DataAccess.Entities.UserEntity
            {
                Id_Usuario = userDto.Id,
                Nombre_Usuario = userDto.Username,
                Rol = userDto.Role
            };

            if (!string.IsNullOrEmpty(newPassword))
            {
                entity.Pasword_Hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            }

            _userRepository.UpdateUser(entity);
        }
    }
}
