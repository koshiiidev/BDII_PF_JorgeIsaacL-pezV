using BDII_PF_JorgeIsaacLópezV.Models;
using BDII_PF_JorgeIsaacLópezV.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace BDII_PF_JorgeIsaacLópezV.Controllers
{
    public class MedicamentosController : Controller
    {
        private GestionHospitalesEntities db = new GestionHospitalesEntities();

        // GET: Medicamentos
        public ActionResult Index(int? hospitalId)
        {
            try
            {
                var medicamentos = db.Medicamentos.Include(m => m.Hospitales).Where(m => m.activo == true);

                if (hospitalId.HasValue && hospitalId.Value > 0)
                {
                    medicamentos = medicamentos.Where(m => m.id_hospital == hospitalId.Value);
                }

                ViewBag.Hospitales = new SelectList(db.Hospitales.Where(h => h.activo == true), "Id_Hospital", "Nombre");
                ViewBag.HospitalSeleccionado = hospitalId;

                return View(medicamentos.OrderBy(m => m.nombre).ToList());
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los medicamentos: " + ex.Message;
                return View(new List<Medicamento>());
            }
        }

        public ActionResult Detalles(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Medicamentos medicamento = db.Medicamentos.Include(m => m.Hospitales)
                    .FirstOrDefault(m => m.id_medicamento == id && m.activo == true);

                if (medicamento == null)
                {
                    return HttpNotFound();
                }

                return View(medicamento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el medicamento: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult Crear()
        {
            try
            {
                ViewBag.Id_Hospital = new SelectList(db.Hospitales.Where(h => h.activo == true), "id_hospital", "nombre");
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la página de creación: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Crear([Bind(Include = "Nombre,Descripcion,Stock,Costo_Unidad,Id_Hospital")] Medicamentos medicamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (medicamento.stock < 0)
                    {
                        ModelState.AddModelError("Stock", "El stock no puede ser negativo.");
                    }

                    if (medicamento.costo_unidad <= 0)
                    {
                        ModelState.AddModelError("Costo_Unidad", "El costo por unidad debe ser mayor a cero.");
                    }

                    var hospital = db.Hospitales.Find(medicamento.id_hospital);
                    if (hospital == null || !hospital.activo == true)
                    {
                        ModelState.AddModelError("Id_Hospital", "Debe seleccionar un hospital válido.");
                    }

                    if (string.IsNullOrWhiteSpace(medicamento.nombre))
                    {
                        ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                    }

                    if (!string.IsNullOrEmpty(medicamento.nombre) && medicamento.nombre.Length > 100)
                    {
                        ModelState.AddModelError("Nombre", "El nombre no puede exceder 100 caracteres.");
                    }

                    if (!string.IsNullOrEmpty(medicamento.descripcion) && medicamento.descripcion.Length > 500)
                    {
                        ModelState.AddModelError("Descripcion", "La descripción no puede exceder 500 caracteres.");
                    }

                    if (ModelState.IsValid)
                    {
                        medicamento.fecha_registro = DateTime.Now;
                        medicamento.activo = true;

                        db.Medicamentos.Add(medicamento);
                        db.SaveChanges();

                        TempData["Mensaje"] = "Medicamento creado exitosamente.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el medicamento: " + ex.Message;
            }

            ViewBag.Id_Hospital = new SelectList(db.Hospitales.Where(h => h.activo == true), "Id_Hospital", "Nombre", medicamento.id_hospital);
            return View(medicamento);
        }

        public ActionResult Editar(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Medicamentos medicamento = db.Medicamentos.Find(id);

                if (medicamento == null || !medicamento.activo == true)
                {
                    return HttpNotFound();
                }

                ViewBag.Id_Hospital = new SelectList(db.Hospitales.Where(h => h.activo == true), "Id_Hospital", "Nombre", medicamento.id_hospital);
                return View(medicamento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el medicamento para editar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar([Bind(Include = "Id_Medicamento,Nombre,Descripcion,Costo_Unidad,Id_Hospital,Stock,Activo,Fecha_Registro")] Medicamento medicamento)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (medicamento.Costo_Unidad <= 0)
                    {
                        ModelState.AddModelError("Costo_Unidad", "El costo por unidad debe ser mayor a cero.");
                    }

                    var hospital = db.Hospitales.Find(medicamento.Id_Hospital);
                    if (hospital == null || !hospital.activo == true)
                    {
                        ModelState.AddModelError("Id_Hospital", "Debe seleccionar un hospital válido.");
                    }

                    if (string.IsNullOrWhiteSpace(medicamento.Nombre))
                    {
                        ModelState.AddModelError("Nombre", "El nombre es obligatorio.");
                    }

                    if (!string.IsNullOrEmpty(medicamento.Nombre) && medicamento.Nombre.Length > 100)
                    {
                        ModelState.AddModelError("Nombre", "El nombre no puede exceder 100 caracteres.");
                    }

                    if (!string.IsNullOrEmpty(medicamento.Descripcion) && medicamento.Descripcion.Length > 500)
                    {
                        ModelState.AddModelError("Descripcion", "La descripción no puede exceder 500 caracteres.");
                    }

                    if (medicamento.Stock < 0)
                    {
                        ModelState.AddModelError("Stock", "El stock no puede ser negativo.");
                    }

                    if (ModelState.IsValid)
                    {
                        var medicamentoOriginal = db.Medicamentos.AsNoTracking()
                            .FirstOrDefault(m => m.id_medicamento == medicamento.Id_Medicamento);

                        if (medicamentoOriginal == null)
                        {
                            return HttpNotFound();
                        }

                        medicamento.Fecha_Registro = medicamentoOriginal.fecha_registro ?? DateTime.Now;

                        db.Entry(medicamento).State = EntityState.Modified;
                        db.SaveChanges();

                        TempData["Mensaje"] = "Medicamento actualizado exitosamente.";
                        return RedirectToAction("Index");
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al actualizar el medicamento: " + ex.Message;
            }

            ViewBag.Id_Hospital = new SelectList(db.Hospitales.Where(h => h.activo == true), "Id_Hospital", "Nombre", medicamento.Id_Hospital);
            return View(medicamento);
        }

        public ActionResult GestionarStock(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Medicamentos medicamento = db.Medicamentos.Include(m => m.Hospitales)
                    .FirstOrDefault(m => m.id_medicamento == id && m.activo == true);

                if (medicamento == null)
                {
                    return HttpNotFound();
                }

                ViewBag.StockActual = medicamento.stock;
                ViewBag.NombreMedicamento = medicamento.nombre;
                ViewBag.NombreHospital = medicamento.Hospitales?.nombre;

                return View(medicamento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la gestión de stock: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarStock(int id, int cantidadAgregar)
        {
            try
            {
                if (cantidadAgregar <= 0)
                {
                    TempData["Error"] = "La cantidad a agregar debe ser mayor a cero.";
                    return RedirectToAction("GestionarStock", new { id = id });
                }

                var medicamento = db.Medicamentos.Find(id);

                if (medicamento == null || !medicamento.activo == true)
                {
                    return HttpNotFound();
                }

                if (medicamento.stock > int.MaxValue - cantidadAgregar)
                {
                    TempData["Error"] = "La cantidad resultante excede el límite permitido.";
                    return RedirectToAction("GestionarStock", new { id = id });
                }

                medicamento.stock += cantidadAgregar;
                db.SaveChanges();

                TempData["Mensaje"] = $"Se agregaron {cantidadAgregar} unidades al stock. Stock actual: {medicamento.stock}";
                return RedirectToAction("Detalles", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al agregar stock: " + ex.Message;
                return RedirectToAction("GestionarStock", new { id = id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReducirStock(int id, int cantidadReducir)
        {
            try
            {
                if (cantidadReducir <= 0)
                {
                    TempData["Error"] = "La cantidad a reducir debe ser mayor a cero.";
                    return RedirectToAction("GestionarStock", new { id = id });
                }

                var medicamento = db.Medicamentos.Find(id);

                if (medicamento == null || !medicamento.activo == true)
                {
                    return HttpNotFound();
                }

                if (medicamento.stock < cantidadReducir)
                {
                    TempData["Error"] = $"No hay suficiente stock. Stock actual: {medicamento.stock}, solicitado: {cantidadReducir}";
                    return RedirectToAction("GestionarStock", new { id = id });
                }

                medicamento.stock -= cantidadReducir;
                db.SaveChanges();

                TempData["Mensaje"] = $"Se redujeron {cantidadReducir} unidades del stock. Stock actual: {medicamento.stock}";
                return RedirectToAction("Detalles", new { id = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al reducir stock: " + ex.Message;
                return RedirectToAction("GestionarStock", new { id = id });
            }
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                Medicamentos medicamento = db.Medicamentos.Include(m => m.Hospitales)
                    .FirstOrDefault(m => m.id_medicamento == id && m.activo == true);

                if (medicamento == null)
                {
                    return HttpNotFound();
                }

                return View(medicamento);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el medicamento para eliminar: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Medicamentos medicamento = db.Medicamentos.Find(id);

                if (medicamento == null)
                {
                    return HttpNotFound();
                }

                var tratamientosActivos = db.Tratamiento_Medicamento
                    .Any(tm => tm.id_medicamento == id);

                if (tratamientosActivos)
                {
                    TempData["Error"] = "No se puede desactivar el medicamento porque está siendo usado en tratamientos activos.";
                    return RedirectToAction("Detalles", new { id = id });
                }

                medicamento.activo = false;
                db.SaveChanges();

                TempData["Mensaje"] = "Medicamento desactivado exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al desactivar el medicamento: " + ex.Message;
                return RedirectToAction("Detalles", new { id = id });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}