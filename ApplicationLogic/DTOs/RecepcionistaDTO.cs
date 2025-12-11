using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DTOs
{
    // Objeto de transferencia para Recepcionista
    public class RecepcionistaDTO
    {
        public int IdRecepcionista { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

    }
}
