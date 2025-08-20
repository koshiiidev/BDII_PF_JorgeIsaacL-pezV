using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class Pago
    {
        public int Id_Pago { get; set; }
        public int Id_Paciente { get; set; }
        public int? Id_Tratamiento { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Monto { get; set; }
        public string Metodo_Pago { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; }

        
        public virtual Paciente Paciente { get; set; }
        public virtual Tratamiento Tratamiento { get; set; }
    }
}