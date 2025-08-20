using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class Doctor
    {
        public int Id_Doctor { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        public string Identificacion { get; set; }
        public string Especialidad { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public int Id_Hospital { get; set; }
        public DateTime Fecha_Registro { get; set; }
        public bool Activo { get; set; }


        public virtual Hospital Hospital { get; set; }
        public virtual ICollection<Cita> Citas { get; set; }
    }
}