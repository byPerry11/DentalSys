using ApplicationLogic.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Services;

namespace UnitTest.Administrador
{
    [TestClass]
    public class AdministradorFindTest
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
        public void Find_ExistingId_ShouldReturnAdministrador()
        {
            int id = _testService.GetExistingId();

            var admin = _service.GetAdministradorById(id);

            Assert.IsNotNull(admin);
            Assert.AreEqual(id, admin.Id_Administrador);
        }

        [TestMethod]
        public void Find_InvalidId_ShouldReturnNull()
        {
            var result = _service.GetAdministradorById(-99999);

            Assert.IsNull(result);
        }
    }
}
