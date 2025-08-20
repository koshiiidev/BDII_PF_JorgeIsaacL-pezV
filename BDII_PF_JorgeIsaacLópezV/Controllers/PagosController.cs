using BDII_PF_JorgeIsaacLópezV.Models;
using BDII_PF_JorgeIsaacLópezV.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BDII_PF_JorgeIsaacLópezV.Controllers
{
    public class PagosController : Controller
    {
        private GestionHospitalesEntities db = new GestionHospitalesEntities();

        public ActionResult Crear(int idTratamiento)
        {
            var t = db.Tratamientos
                .Include("Cita.Paciente")
                .Include("TratamientoMedicamentos.Medicamento")
                .FirstOrDefault(x => x.id_tratamiento == idTratamiento);

            if (t == null) return HttpNotFound();

            var modelo = new PagoCreate
            {
                IdTratamiento = t.id_tratamiento,
                IdPaciente = t.Citas.id_paciente,
                Monto = t.costo
            };

            return View(modelo);
        }

        [HttpPost]
        public ActionResult Crear(PagoCreate modelo)
        {
            if (!ModelState.IsValid) return View(modelo);

            var pago = new Pagos
            {
                id_paciente = modelo.IdPaciente,
                id_tratamiento = modelo.IdTratamiento,
                fecha = DateTime.Now,
                monto = modelo.Monto,
                metodo_pago = modelo.MetodoPago,
                descripcion = modelo.Descripcion,
                estado = modelo.Estado,
                fecha_creacion = DateTime.Now
            };
            db.Pagos.Add(pago);

            if (pago.estado == "pagado")
            {
                var tratamiento = db.Tratamientos
                    .Include("TratamientoMedicamentos.Medicamento")
                    .FirstOrDefault(x => x.id_tratamiento == modelo.IdTratamiento);

                foreach (var tm in tratamiento.Tratamiento_Medicamento)
                {
                    if (tm.Medicamentos.stock < tm.cantidad)
                    {
                        ModelState.AddModelError("", $"No hay suficiente stock de {tm.Medicamentos.nombre}");
                        return View(modelo);
                    }
                    tm.Medicamentos.stock -= tm.cantidad;
                }
            }

            db.SaveChanges();
            return RedirectToAction("Detalles", new { id = pago.id_pago });
        }

        public ActionResult Detalles(int id)
        {
            var p = db.Pagos.Include("Paciente").FirstOrDefault(x => x.id_pago == id);
            if (p == null) return HttpNotFound();

            var vm = new PagoDetalle
            {
                IdPago = p.id_pago,
                IdTratamiento = p.id_tratamiento ?? 0,
                Paciente = p.Pacientes.nombre,
                Fecha = p.fecha.ToString("dd/MM/yyyy"),
                Monto = p.monto,
                MetodoPago = p.metodo_pago,
                Estado = p.estado,
                Descripcion = p.descripcion
            };

            return View(vm);
        }
    }
}