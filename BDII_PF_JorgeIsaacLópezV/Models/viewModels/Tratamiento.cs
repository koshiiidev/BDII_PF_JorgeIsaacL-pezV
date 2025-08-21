using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class Tratamiento
    {
        public int Id_Tratamiento { get; set; }
        public int Id_Cita { get; set; }

        [Required(ErrorMessage = "El Costo es obligatorio")]
        [Display(Name = "Costo")]
        public decimal Costo { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        public DateTime Fecha_Tratamiento { get; set; }

        
        public virtual Cita Cita { get; set; }
        public virtual ICollection<TratamientoMedicamento> TratamientoMedicamentos { get; set; }
        public virtual ICollection<Pago> Pagos { get; set; }
    }
}