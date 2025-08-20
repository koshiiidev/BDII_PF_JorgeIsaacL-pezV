using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class Medicamento
    {
        public int Id_Medicamento { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Stock { get; set; }
        public decimal Costo_Unidad { get; set; }
        public int Id_Hospital { get; set; }
        public DateTime Fecha_Registro { get; set; }

        [Required]
        public bool Activo { get; set; }

        
        public virtual Hospital Hospital { get; set; }
        public virtual ICollection<TratamientoMedicamento> TratamientoMedicamentos { get; set; }
    }

    
}