using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    
    // Entidad que representa un usuario en la base de datos
    public class UserEntity
    {
        public int Id_Usuario { get; set; }
        public string? Nombre_Usuario { get; set; }
        public string? Pasword_Hash { get; set; }
        public string? Rol { get; set; }
        public DateTime? Fecha_Creacion { get; set; }
    }
}
