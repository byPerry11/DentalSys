using ApplicationLogic.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTest.Administrador
{
    [TestClass]
    public class GetAllAdministradoresTests
    {
        private AdministradorService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new AdministradorService();
        }

        [TestMethod]
        public void GetAll_ShouldReturnList()
        {
            var result = _service.GetAllAdministradores();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count >= 0); // Siempre debe devolver lista válida
        }
    }
}
