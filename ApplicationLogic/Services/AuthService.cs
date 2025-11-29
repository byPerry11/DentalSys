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
    // Servicio de autenticación para validar credenciales de usuario
    public class AuthService
    {
        private readonly UserRepository _userRepository;

        
        public AuthService()
        {
            var provider = ConnectionFactory.Create();
            _userRepository = new UserRepository(provider);
        }

        public AuthService(IConnectionProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException(nameof(provider));

            _userRepository = new UserRepository(provider);
        }

        
        public UserDTO? Login(string username, string? password)
        {
            // Validar que los parámetros no sean nulos o vacíos
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var userEntity = _userRepository.GetUserByUsername(username);

            if (userEntity == null)
            {
                return null; // Usuario no encontrado
            }

            // Validamos la contraseña con BCrypt
            bool passwordValid = BCrypt.Net.BCrypt.Verify(password, userEntity.Pasword_Hash);

            if (passwordValid)
            {
                return new UserDTO
                {
                    Id = userEntity.Id_Usuario,
                    Username = userEntity.Nombre_Usuario,
                    Role = userEntity.Rol
                };
            }

            return null; // Contraseña incorrecta
        }
    }
}
