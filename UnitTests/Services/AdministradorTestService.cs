using ApplicationLogic.DTOs;
using ApplicationLogic.Services;
using System;
using System.Linq;

namespace UnitTest.Services
{
    public class AdministradorTestService
    {
        private readonly AdministradorService _service;

        public AdministradorTestService(AdministradorService service)
        {
            _service = service;
        }

        public int GetExistingId()
        {
            var list = _service.GetAllAdministradores();

            if (list.Count == 0)
            {
                var dto = new AdministradorDTO
                {
                    Nombre = "TestAdmin",
                    Telefono = "5512345678",
                    Email = "testadmin@test.com",
                    Estado = "activo"
                };

                _service.AddAdministrador(dto);
                list = _service.GetAllAdministradores();
            }

            return list.OrderBy(a => a.Id_Administrador).Last().Id_Administrador;
        }

        public int CreateNewAdministrador()
        {
            var email = UtilsTestService.RandomEmail(); // lo guardamos para buscar el registro exacto

            var dto = new AdministradorDTO
            {
                Nombre = "TestAdministrador_" + Guid.NewGuid().ToString("N").Substring(5),
                Telefono = UtilsTestService.RandomPhone(),
                Email = email,
                Estado = "activo"
            };

            _service.AddAdministrador(dto);

            // Recuperar EXACTAMENTE el registro recién insertado
            var inserted = _service.GetAllAdministradores()
                                   .First(a => a.Email == email);

            return inserted.Id_Administrador;
        }

    }
}