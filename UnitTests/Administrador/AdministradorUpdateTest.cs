using ApplicationLogic.DTOs;
using ApplicationLogic.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Services;

namespace UnitTest.Administrador
{
    [TestClass]
    public class AdministradorUpdateTest
    {
        private AdministradorService _service;
        private AdministradorTestService _testService;

        [TestInitialize]
        public void Setup()
        {
            _service = new AdministradorService();
            _testService = new AdministradorTestService(_service);
        }

        [TestMethod]
        public void Update_Success_ShouldModifyRecord()
        {
            int id = _testService.CreateNewAdministrador();

            var dto = _service.GetAdministradorById(id);
            dto.Nombre = "NombreModificado_UnitTest";

            _service.UpdateAdministrador(dto);

            var updated = _service.GetAdministradorById(id);

            Assert.AreEqual("NombreModificado_UnitTest", updated.Nombre);
        }

        [TestMethod]
        public void Update_Fail_ShouldThrowException_WhenIdDoesNotExist()
        {
            var dto = new AdministradorDTO
            {
                Id_Administrador = -99999,
                Nombre = "X_Prueba",
                Telefono = "1234567890",
                Email = "noexiste@test.com",
                Estado = "activo"
            };

            Assert.ThrowsException<Exception>(() =>
            {
                _service.UpdateAdministrador(dto);
            });
        }
    }
}