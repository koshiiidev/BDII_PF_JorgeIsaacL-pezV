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

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no debe contener números")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [Display(Name = "Apellidos")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El apellido no debe contener números")]
        [StringLength(200, ErrorMessage = "El apellido no puede exceder 200 caracteres")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La identificacion es obligatoria")]
        [Display(Name = "Identificacion")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "La especialidad es obligatoria")]
        [Display(Name = "Especialidad")]
        public string Especialidad { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [Display(Name = "Correo Electrónico")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El número de telefono es obligatorio")]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }
        public int Id_Hospital { get; set; }

        public DateTime Fecha_Registro { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Activo")]
        public bool Activo { get; set; }


        public virtual Hospital Hospital { get; set; }
        public virtual ICollection<Cita> Citas { get; set; }
    }
}