using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class MedicamentoSeleccion
    {
        public int IdMedicamento { get; set; }
        public string Nombre { get; set; }
        public int Stock { get; set; }
        public decimal CostoUnidad { get; set; }
        public bool Seleccionado { get; set; }
        public int? Cantidad { get; set; }
    }

    public class TratamientoCreate
    {
        public int IdCita { get; set; }
        public string Descripcion { get; set; }
        public List<MedicamentoSeleccion> Medicamentos { get; set; } = new List<MedicamentoSeleccion>();
    }
}