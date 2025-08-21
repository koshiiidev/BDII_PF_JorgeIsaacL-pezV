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
                .Include("Citas.Pacientes")
                .FirstOrDefault(x => x.id_tratamiento == idTratamiento);

            if (t == null) return HttpNotFound();


            var pagoExistente = db.Pagos.FirstOrDefault(p => p.id_tratamiento == idTratamiento);
            if (pagoExistente != null)
            {
                TempData["Mensaje"] = "Este tratamiento ya tiene un pago procesado.";
                TempData["TipoMensaje"] = "warning";
                return RedirectToAction("DetallesCitas", "Citas", new { id = t.Citas.id_cita });
            }


            decimal montoTotal = t.costo;


            if (montoTotal <= 0)
            {
                TempData["Mensaje"] = "No se puede procesar el pago. El tratamiento no tiene un costo válido.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("DetallesCitas", "Citas", new { id = t.Citas.id_cita });
            }

            var modelo = new PagoCreate
            {
                IdTratamiento = t.id_tratamiento,
                IdPaciente = t.Citas.id_paciente,
                IdCita = t.Citas.id_cita,
                Monto = montoTotal,
                Estado = "pagado", 
                MetodoPago = "efectivo" 
            };
            
            return View(modelo);
        }

        [HttpPost]
        public ActionResult Crear(PagoCreate modelo)
        {
            if (Request.Form["Monto"] != null)
            {
                if (decimal.TryParse(Request.Form["Monto"].Replace(".", ","), out decimal montoConvertido))
                {
                    modelo.Monto = montoConvertido;
                }
            }

            if (!ModelState.IsValid) return View(modelo);

            var pagoExistente = db.Pagos.FirstOrDefault(p => p.id_tratamiento == modelo.IdTratamiento);
            if (pagoExistente != null)
            {
                TempData["Mensaje"] = "Este tratamiento ya tiene un pago procesado.";
                TempData["TipoMensaje"] = "warning";
                return RedirectToAction("DetallesCitas", "Citas", new { id = modelo.IdCita });
            }

            var pago = new Pagos
            {
                id_paciente = modelo.IdPaciente,
                id_tratamiento = modelo.IdTratamiento,
                fecha = DateTime.Now,
                monto = modelo.Monto,
                metodo_pago = modelo.MetodoPago,
                descripcion = modelo.Descripcion ?? "Pago procesado exitosamente",
                estado = "pagado",
                fecha_creacion = DateTime.Now
            };

            db.Pagos.Add(pago);

            var tratamiento = db.Tratamientos
                .Include("Tratamiento_Medicamento.Medicamentos")
                .FirstOrDefault(x => x.id_tratamiento == modelo.IdTratamiento);

            if (tratamiento != null && tratamiento.Tratamiento_Medicamento != null)
            {
                foreach (var tm in tratamiento.Tratamiento_Medicamento)
                {
                    if (tm.Medicamentos.stock < tm.cantidad)
                    {
                        ModelState.AddModelError("", $"No hay suficiente stock de {tm.Medicamentos.nombre}. Stock disponible: {tm.Medicamentos.stock}");
                        return View(modelo);
                    }
                    tm.Medicamentos.stock -= tm.cantidad;
                }
            }

            try
            {
                db.SaveChanges();
                TempData["Mensaje"] = "Pago procesado exitosamente.";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Detalles", new { id = pago.id_pago });
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al procesar el pago: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return View(modelo);
            }
        }

        public ActionResult Detalles(int id)
        {
            var p = db.Pagos.Include("Pacientes").FirstOrDefault(x => x.id_pago == id);
            if (p == null) return HttpNotFound();

            var vm = new PagoDetalle
            {
                IdPago = p.id_pago,
                IdTratamiento = p.id_tratamiento ?? 0,
                IdPaciente = p.id_paciente,
                Paciente = p.Pacientes.nombre,
                Fecha = p.fecha.ToString("dd/MM/yyyy"),
                Monto = p.monto,
                MetodoPago = p.metodo_pago,
                Estado = p.estado,
                Descripcion = p.descripcion
            };

            return View(vm);
        }


        public bool TratamientoFuePagado(int idTratamiento)
        {
            return db.Pagos.Any(p => p.id_tratamiento == idTratamiento && p.estado == "pagado");
        }
    }
}