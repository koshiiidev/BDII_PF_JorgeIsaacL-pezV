using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class Cita
    {
        public int Id_Cita { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
        public string Diagnostico { get; set; }
        public int Id_Doctor { get; set; }
        public int Id_Paciente { get; set; }
        public int Id_Hospital { get; set; }
        public string Estado { get; set; }
        public DateTime Fecha_Creacion { get; set; }

        
        public virtual Doctor Doctor { get; set; }
        public virtual Paciente Paciente { get; set; }
        public virtual Hospital Hospital { get; set; }
        public virtual ICollection<Tratamiento> Tratamientos { get; set; }
    }

    public class ListaCitas
    {
        public List<Cita> Citas { get; set; }
        public string Mensaje { get; set; }
        public string TipoMensaje { get; set; }
        public ListaCitas()
        {
            Citas = new List<Cita>();
        }
    }
}