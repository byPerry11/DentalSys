using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DTOs
{
    public class CitaDTO
    {
        public int Id { get; set; }
        public int PacienteId { get; set; }
        public string PacienteNombre { get; set; } // For display purposes
        public int DentistaId { get; set; }
        public string DentistaNombre { get; set; } // For display purposes
        public DateTime FechaHora { get; set; }
        public string Estatus { get; set; }
    }
}
