using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class CitaEntity
    {
        public int Id_cita { get; set; }
        public int Id_Paciente { get; set; }
        public int Id_Dentista { get; set; }
        public DateTime Fecha_hora { get; set; }
        public string Estatus_Cita { get; set; }
    }
}
