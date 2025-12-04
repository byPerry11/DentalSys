using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class PacienteEntity
    {
        public int Id_Paciente { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Contacto_Emergencia { get; set; }
        public string? Numero_Emergencia { get; set; }
        public string? Email { get; set; }
        public DateTime Fecha_Nacimiento { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public string? Facturada { get; set; } // ENUM('Si', 'No')
        public decimal Tasa_IVA { get; set; }
    }
}
