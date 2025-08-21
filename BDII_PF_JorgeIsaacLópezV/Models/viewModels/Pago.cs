using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [Required(ErrorMessage = "El monto es obligatorio")]
        [Display(Name = "Monto")]
        public decimal Monto { get; set; }

        [Required(ErrorMessage = "El metodo de pago es obligatorio")]
        [Display(Name = "Metodo Pago")]
        public string Metodo_Pago { get; set; }

        [Required(ErrorMessage = "La descripcion es obligatoria")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Descripción")]
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; }

        
        public virtual Paciente Paciente { get; set; }
        public virtual Tratamiento Tratamiento { get; set; }
    }
}