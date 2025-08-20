using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class TratamientoDetalle
    {
        public int IdTratamiento { get; set; }
        public string Paciente { get; set; }
        public string Doctor { get; set; }
        public string Descripcion { get; set; }
        public string Fecha { get; set; }
        public List<TratamientoMedicamentoItem> Medicamentos { get; set; } = new List<TratamientoMedicamentoItem>();
        public decimal Total { get; set; }
    }

    public class TratamientoMedicamentoItem
    {
        public string Nombre { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnidad { get; set; }
        public decimal Subtotal => Cantidad * CostoUnidad;
    }

}