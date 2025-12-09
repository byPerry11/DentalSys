using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLogic.DTOs
{
    public class ConsultaDTO
    {
        public int Id { get; set; }
        public int TratamientoId { get; set; }
        public string TratamientoNombre { get; set; }
        public int CitaId { get; set; }
        public string PacienteNombre { get; set; }
        public string DentistaNombre { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Precio { get; set; }
        public string Notas { get; set; }
    }
}
