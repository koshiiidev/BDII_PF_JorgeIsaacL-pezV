using BDII_PF_JorgeIsaacLópezV.Models;
using BDII_PF_JorgeIsaacLópezV.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BDII_PF_JorgeIsaacLópezV.Controllers
{
    public class TratamientosController : Controller
    {
        private GestionHospitalesEntities db = new GestionHospitalesEntities();

        // GET: Tratamientos
        public ActionResult Crear(int idCita)
        {
            var cita = db.Citas.Find(idCita);
            if (cita == null)
            {
                TempData["Mensaje"] = "Cita no encontrada.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index", "Citas");
            }

            var modelo = new TratamientoCreate
            {
                IdCita = idCita,
                Medicamentos = db.Medicamentos
                    .Where(m => m.activo == true && m.id_hospital == cita.id_hospital)
                .Select(m => new MedicamentoSeleccion
                {
                    IdMedicamento = m.id_medicamento,
                    Nombre = m.nombre,
                    Stock = m.stock,
                    CostoUnidad = m.costo_unidad
                }).ToList()
            };

            return View(modelo);
        } 

        [HttpPost]
        public ActionResult Crear(TratamientoCreate modelo) 
        {
            var seleccionados = modelo.Medicamentos
            .Where(x => x.Seleccionado && x.Cantidad.HasValue && x.Cantidad.Value > 0)
            .ToList();

            if (!seleccionados.Any())
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un medicamento con cantidad.");
                return View(modelo);
            }

            var tratamiento = new Tratamientos
            {
                id_cita = modelo.IdCita,
                descripcion = modelo.Descripcion,
                fecha_tratamiento = DateTime.Now,
                costo = 0
            };
            db.Tratamientos.Add(tratamiento);
            db.SaveChanges();

            decimal total = 0;
            foreach (var med in seleccionados)
            {
                db.Tratamiento_Medicamento.Add(new Tratamiento_Medicamento
                {
                    id_tratamiento = tratamiento.id_tratamiento,
                    id_medicamento = med.IdMedicamento,
                    cantidad = med.Cantidad.Value
                });

                total += med.Cantidad.Value * med.CostoUnidad;
            }

            tratamiento.costo = total;
            db.SaveChanges();

            return RedirectToAction("Detalles", new { id = tratamiento.id_tratamiento });
        }

        public ActionResult Detalles(int id)
        {
            var t = db.Tratamientos
                .Include("Citas.Pacientes")
                .Include("Citas.Doctores")
                .Include("Tratamiento_Medicamento.Medicamentos")
                .FirstOrDefault(x => x.id_tratamiento == id);

            if (t == null) return HttpNotFound();

            var vm = new TratamientoDetalle
            {
                IdTratamiento = t.id_tratamiento,
                Paciente = t.Citas.Pacientes.nombre,
                Doctor = t.Citas.Doctores.nombre,
                Descripcion = t.descripcion,
                Fecha = t.fecha_tratamiento.Value.ToString("dd/MM/yyyy HH:mm"),
                Medicamentos = t.Tratamiento_Medicamento.Select(tm => new TratamientoMedicamentoItem
                {
                    Nombre = tm.Medicamentos.nombre,
                    Cantidad = tm.cantidad,
                    CostoUnidad = tm.Medicamentos.costo_unidad
                }).ToList()
            };
            vm.Total = vm.Medicamentos.Sum(x => x.Subtotal);

            return View(vm);
        }

        public ActionResult Eliminar(int id)
        {
            var tratamiento = db.Tratamientos
                .Include("Citas")
                .Include("Tratamiento_Medicamento")
                .FirstOrDefault(t => t.id_tratamiento == id);

            if (tratamiento == null)
            {
                TempData["Mensaje"] = "Tratamiento no encontrado.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index", "Citas");
            }

            var pagado = db.Pagos.Any(p => p.id_tratamiento == id && p.estado == "pagado");
            if (pagado)
            {
                TempData["Mensaje"] = "No se puede eliminar un tratamiento que ya fue pagado.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("DetallesCitas", "Citas", new { id = tratamiento.Citas.id_cita });
            }

            int idCita = tratamiento.Citas.id_cita;

            try
            {
                var medicamentosTratamiento = db.Tratamiento_Medicamento.Where(tm => tm.id_tratamiento == id).ToList();
                foreach (var tm in medicamentosTratamiento)
                {
                    db.Tratamiento_Medicamento.Remove(tm);
                }

                db.Tratamientos.Remove(tratamiento);

                db.SaveChanges();

                TempData["Mensaje"] = "Tratamiento eliminado exitosamente.";
                TempData["TipoMensaje"] = "success";
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al eliminar el tratamiento: " + ex.Message;
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction("DetallesCitas", "Citas", new { id = idCita });
        }

        [HttpPost]
        public ActionResult ConfirmarEliminacion(int id)
        {
            return Eliminar(id);
        }

    }
}