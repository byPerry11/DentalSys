using ApplicationLogic.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTest.Services;

namespace UnitTest.Administrador
{
    [TestClass]
    public class AdministradorDeleteTest
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
        public void Delete_Success_ShouldRemoveRecord()
        {
            int id = _testService.CreateNewAdministrador();

            _service.DeleteAdministrador(id);

            var result = _service.GetAdministradorById(id);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void Delete_Fail_ShouldThrowException_WhenIdIsInvalid()
        {
            Assert.ThrowsException<Exception>(() =>
            {
                _service.DeleteAdministrador(-99999);
            });
        }
    }
}
