using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class Paciente
    {
        public int Id_Paciente { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio")]
        [Display(Name = "Nombre")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no debe contener números")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [Display(Name = "Apellidos")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no debe contener números")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "La Identificacion es obligatoria")]
        [Display(Name = "Identificacion")]
        public string Identificacion { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime Fecha_Nacimiento { get; set; }

        [Required(ErrorMessage = "El genero es obligatorio")]
        [Display(Name = "Genero")]
        public char Genero { get; set; }

        [Required(ErrorMessage = "La direccion es obligatoria")]
        [Display(Name = "Direccion")]
        public string Direccion { get; set; }

        [Required(ErrorMessage = "El numero de telefono es obligatorio")]
        [Display(Name = "Telefono")]
        public string Telefono { get; set; }
        public int Id_Hospital { get; set; }
        public DateTime Fecha_Registro { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        [Display(Name = "Activo")]
        public bool Activo { get; set; }

        
        //public virtual Hospital Hospital { get; set; }
        //public virtual ICollection<Cita> Citas { get; set; }
        //public virtual ICollection<Pago> Pagos { get; set; }
    }

    public class ListaPacientes
    {
        public List<Paciente> Pacientes { get; set; }
        public string Mensaje { get; set; }
        public string TipoMensaje { get; set; }
        public ListaPacientes()
        {
            Pacientes = new List<Paciente>();
        }
    }
}