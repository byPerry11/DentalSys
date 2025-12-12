using System;

namespace DataAccess.Entities
{
    // Clase que representa la tabla Administrador
    public class AdministradorEntity
    {
        public int Id_Administrador { get; set; }
        public int? Id_Usuario { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public string Estado { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
    }
}

