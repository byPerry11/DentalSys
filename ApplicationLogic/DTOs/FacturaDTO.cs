using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DTOs
{
    public class FacturaDTO
    {
        public string Folio { get; set; }
        public int ConsultaId { get; set; }
        public int PacienteId { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteTelefono { get; set; }
        public string PacienteEmail { get; set; }
        public string DentistaNombre { get; set; }
        public string TratamientoNombre { get; set; }
        public DateTime FechaConsulta { get; set; }
        public decimal Subtotal { get; set; }
        public decimal TasaIVA { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }

    }
}
