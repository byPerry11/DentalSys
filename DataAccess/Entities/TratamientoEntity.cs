using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class TratamientoEntity
    {
        public int Id_Tratamiento { get; set; }
        public string? Nombre { get; set; }
        public decimal Precio { get; set; }
        public string? Descripcion { get; set; }
    }
}
