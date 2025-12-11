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
        public string? Role { get; set; }//sera admin, dentista o recepcionista
        public int? Id_Dentista { get; set; } // For dentist users, stores their Id_Dentista
        public DateTime? FechaCreacion { get; set; }
    }
}
