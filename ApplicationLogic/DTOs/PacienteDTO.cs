using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DTOs
{
    public class PacienteDTO
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? ContactoEmergencia { get; set; }
        public string? NumeroEmergencia { get; set; }
        public string? Email { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Facturada { get; set; }
        public decimal TasaIVA { get; set; }
    }
}
