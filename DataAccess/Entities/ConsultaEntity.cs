using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class ConsultaEntity
    {
        public int Id_Consulta { get; set; }
        public int Id_Tratamiento { get; set; }
        public int Id_Cita { get; set; }
        public decimal Precio_Consulta { get; set; }
        public string Notas { get; set; }
    }
}
