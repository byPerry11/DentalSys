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
                var userDto = new UserDTO
                {
                    Id = userEntity.Id_Usuario,
                    Username = userEntity.Nombre_Usuario,
                    Role = userEntity.Rol,
                    FechaCreacion = userEntity.Fecha_Creacion
                };

                switch (userEntity.Rol?.ToLower())
                {
                    case "administrador":
                        {
                            var adminService = new AdministradorService();
                            var admin = adminService.GetAdministradorByUsuarioId(userEntity.Id_Usuario);

                            if (admin == null)
                                return null; // usuario sin perfil asignado → no debe entrar

                            userDto.Id_Administrador = admin.Id_Administrador;
                            break;
                        }

                    case "recepcionista":
                        {
                            var recepService = new RecepcionistaService();
                            var recepcionista = recepService.GetRecepcionistaByUsuarioId(userEntity.Id_Usuario);

                            if (recepcionista == null)
                                return null; // usuario sin perfil asignado

                            userDto.Id_Recepcionista = recepcionista.Id_Recepcionista;
                            break;
                        }

                    case "dentista":
                        {
                            var dentistaService = new DentistaService();
                            var dentista = dentistaService.GetDentistaByUsuarioId(userEntity.Id_Usuario);

                            if (dentista == null)
                                return null; // usuario sin perfil asignado

                            userDto.Id_Dentista = dentista.Id_Dentista;
                            break;
                        }

                    default:
                        return null;
                }

                return userDto;
            }

            return null; // Contraseña incorrecta
        }
    }
}
