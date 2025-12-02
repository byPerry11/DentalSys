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
        public DateTime? FechaCreacion { get; set; }
    }
}
