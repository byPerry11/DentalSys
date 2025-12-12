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
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly DentistaRepository _dentistaRepository;
        private readonly RecepcionistaRepository _recepcionistaRepository;
        private readonly AdministradorRepository _administradorRepository;

        public UserService()
        {
            var provider = ConnectionFactory.Create();
            _userRepository = new UserRepository(provider);
            _dentistaRepository = new DentistaRepository(provider);
            _recepcionistaRepository = new RecepcionistaRepository(provider);
            _administradorRepository = new AdministradorRepository(provider);
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
            var user = _userRepository.GetUserById(id);
            if (user != null)
            {
                var role = user.Rol?.ToLower().Trim();
                switch (role)
                {
                    case "dentista":
                        var dent = _dentistaRepository.GetDentistaByUsuarioId(id);
                        if (dent != null)
                            _userRepository.RemoveUserFromDentista(dent.Id_Dentista);
                        break;
                    case "recepcionista":
                        var resp = _recepcionistaRepository.GetRecepcionistaByUsuarioId(id);
                        if (resp != null)
                            _userRepository.RemoveUserFromRecepcionista(resp.Id_Recepcionista);
                        break;
                    case "administrador":
                        var adm = _administradorRepository.GetAdministradorByUsuarioId(id);
                        if (adm != null)
                            _userRepository.RemoveUserFromAdministrador(adm.Id_Administrador);
                        break;
                }
            }
            _userRepository.DeleteUser(id);
        }


        public int CreateUser(UserDTO userDto, string password)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password required", nameof(password));
            if (string.IsNullOrWhiteSpace(userDto.Role)) throw new ArgumentException("Role required", nameof(userDto.Role));

            var existing = _userRepository.GetUserByUsername(userDto.Username ?? string.Empty);
            if (existing != null)
                throw new InvalidOperationException("El nombre de usuario ya existe.");

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

            var entity = new UserEntity
            {
                Nombre_Usuario = userDto.Username,
                Pasword_Hash = passwordHash,
                Rol = userDto.Role,
                Fecha_Creacion = DateTime.Now
            };

            int newUserId = _userRepository.AddUserReturningId(entity);
            if (newUserId <= 0)
                throw new InvalidOperationException("No se pudo obtener el id del usuario creado.");

            // Asignación según rol
            var normalizedRole = userDto.Role.ToLower().Trim();
            switch (normalizedRole)
            {
                case "dentista":
                    if (!userDto.Id_Dentista.HasValue)
                        throw new InvalidOperationException("Debe seleccionar un dentista disponible.");

                    // Valida que ese dentista esté disponible
                    var disponiblesD = _userRepository.GetDentistasDisponibles();
                    if (!disponiblesD.Any(d => d.Id_Dentista == userDto.Id_Dentista.Value))
                        throw new InvalidOperationException("El dentista seleccionado ya no está disponible.");

                    _userRepository.AssignUserToDentista(userDto.Id_Dentista.Value, newUserId);
                    break;

                case "recepcionista":
                    if (!userDto.Id_Recepcionista.HasValue)
                        throw new InvalidOperationException("Debe seleccionar una recepcionista disponible.");

                    var disponiblesR = _userRepository.GetRecepcionistasDisponibles();
                    if (!disponiblesR.Any(r => r.Id_Recepcionista == userDto.Id_Recepcionista.Value))
                        throw new InvalidOperationException("La recepcionista seleccionada ya no está disponible.");

                    _userRepository.AssignUserToRecepcionista(userDto.Id_Recepcionista.Value, newUserId);
                    break;

                case "administrador":
                    if (!userDto.Id_Administrador.HasValue)
                        throw new InvalidOperationException("Debe seleccionar un administrador disponible.");

                    var disponiblesA = _userRepository.GetAdministradoresDisponibles();
                    if (!disponiblesA.Any(a => a.Id_Administrador == userDto.Id_Administrador.Value))
                        throw new InvalidOperationException("El administrador seleccionado ya no está disponible.");

                    _userRepository.AssignUserToAdministrador(userDto.Id_Administrador.Value, newUserId);
                    break;

                default:
                    // Si es otro rol (no se asocia a perfil), nada más se creó el usuario.
                    break;
            }

            return newUserId;
        }

        public void UpdateUser(UserDTO userDto, string? newPassword)
        {
            var entity = new UserEntity
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

        // Métodos para exponer perfiles disponibles a la UI
        public List<DentistaDTO> GetDentistasDisponibles()
        {
            var entities = _userRepository.GetDentistasDisponibles();
            return entities.Select(e => new DentistaDTO
            {
                Id_Dentista = e.Id_Dentista,
                Nombre = e.Nombre,
                Telefono = e.Telefono,
                Especialidad = e.Especialidad,
                Email = e.Email
            }).ToList();
        }

        public List<AdministradorDTO> GetAdministradoresDisponibles()
        {
            var entities = _userRepository.GetAdministradoresDisponibles();
            return entities.Select(e => new AdministradorDTO
            {
                Id_Administrador = e.Id_Administrador,
                Nombre = e.Nombre,
                Telefono = e.Telefono,
                Email = e.Email,
                Estado = e.Estado,
                Fecha_Creacion = e.Fecha_Creacion
            }).ToList();
        }

        public List<RecepcionistaDTO> GetRecepcionistasDisponibles()
        {
            var entities = _userRepository.GetRecepcionistasDisponibles();
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
    }
}
