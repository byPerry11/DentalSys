using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; } // administrador, dentista, recepcionista
        public int? Id_Dentista { get; set; }
        public int? Id_Recepcionista { get; set; }
        public int? Id_Administrador { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
