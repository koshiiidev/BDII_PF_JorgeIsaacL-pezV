using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDII_PF_JorgeIsaacLópezV.Models.viewModels
{
    public class PagoCreate
    {
        public int IdTratamiento { get; set; }
        public int IdPaciente { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; } = "efectivo";
        public string Descripcion { get; set; }
        public string Estado { get; set; } = "pagado"; // pagado / pendiente / cancelado
    }

    public class PagoDetalle
    {
        public int IdPago { get; set; }
        public int IdTratamiento { get; set; }
        public string Paciente { get; set; }
        public string Fecha { get; set; }
        public decimal Monto { get; set; }
        public string MetodoPago { get; set; }
        public string Estado { get; set; }
        public string Descripcion { get; set; }
    }
}