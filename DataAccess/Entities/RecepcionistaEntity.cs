using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    // Clase que representa la tabla Recepcionista
    public class RecepcionistaEntity
    {
        public int Id_Recepcionista { get; set; }
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

    }
}
