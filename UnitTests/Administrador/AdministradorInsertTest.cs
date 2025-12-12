using ApplicationLogic.DTOs;
using ApplicationLogic.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Services;

namespace UnitTest.Administrador
{
    [TestClass]
    public class AdministradorInsertTest
    {
        private AdministradorService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new AdministradorService();
        }

        [TestMethod]
        public void Insert_Success_ShouldCreateRecord()
        {
            var dto = new AdministradorDTO
            {
                Nombre = UtilsTestService.RandomString(),
                Telefono = UtilsTestService.RandomPhone(),
                Email = UtilsTestService.RandomEmail(),
                Estado = "activo"
            };

            _service.AddAdministrador(dto);

            var list = _service.GetAllAdministradores();

            Assert.IsTrue(list.Exists(a => a.Email == dto.Email));
        }

        [TestMethod]
        public void Insert_Fail_ShouldThrowException_WhenNameIsNull()
        {
            var dto = new AdministradorDTO
            {
                Nombre = null,
                Telefono = UtilsTestService.RandomPhone(),
                Email = UtilsTestService.RandomEmail(),
                Estado = "activo"
            };

            Assert.ThrowsException<Exception>(() =>
            {
                _service.AddAdministrador(dto);
            });
        }
    }
}