using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class TratamientoMedicamento
    {
        public int Id_Tratamiento { get; set; }
        public int Id_Medicamento { get; set; }
        public int Cantidad { get; set; }

        
        public virtual Tratamiento Tratamiento { get; set; }
        public virtual Medicamento Medicamento { get; set; }
    }
}