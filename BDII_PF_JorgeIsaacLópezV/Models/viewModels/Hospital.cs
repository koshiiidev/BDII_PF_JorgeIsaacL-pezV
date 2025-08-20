using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class Hospital
    {
        public int Id_Hospital { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [Display(Name = "Nombre")]
        [RegularExpression(@"^[a-zA-ZáéíóúÁÉÍÓÚñÑ\s]+$", ErrorMessage = "El nombre no debe contener números")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La direccion es obligatoria")]
        [Display(Name = "Direccion")]
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public DateTime Fecha_Creacion { get; set; }
        public bool Activo { get; set; }

        
        

        //public virtual ICollection<Doctor> Doctores { get; set; }
        //public virtual ICollection<Paciente> Pacientes { get; set; }
        //public virtual ICollection<Medicamento> Medicamentos { get; set; }
        //public virtual ICollection<Cita> Citas { get; set; }
    }

    public class ListaHospitales
    {
        public List<Hospital> Hospitales { get; set; }

        public string Mensaje { get; set; }

        public string TipoMensaje { get; set; }

        public ListaHospitales()
        {
            Hospitales = new List<Hospital>();
        }
    }
}