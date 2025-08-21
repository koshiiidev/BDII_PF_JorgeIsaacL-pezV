using BDII_PF_JorgeIsaacLópezV.Models;
using BDII_PF_JorgeIsaacLópezV.Models.viewModels;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace BDII_PF_JorgeIsaacLópezV.Controllers
{
    public class CitasController : Controller
    {
        private GestionHospitalesEntities db = new GestionHospitalesEntities();

        public ActionResult Index()
        {
            var listaCitas = new ListaCitas();
            try
            {
                var citasEntity = db.Citas
                        .Include("Doctores")
                        .Include("Pacientes")
                        .Include("Hospitales")
                        .Where(c => c.Hospitales.activo == true)
                        .OrderByDescending(c => c.fecha)
                        .ThenByDescending(c => c.hora)
                        .ToList();

                listaCitas.Citas = citasEntity.Select(c => new Cita
                {
                    Id_Cita = c.id_cita,
                    Fecha = c.fecha,
                    Hora = c.hora,
                    Diagnostico = c.diagnostico,
                    Id_Doctor = c.id_doctor,
                    Id_Paciente = c.id_paciente,
                    Id_Hospital = c.id_hospital,
                    Estado = c.estado,
                    Fecha_Creacion = c.fecha_creacion ?? DateTime.Now,
                    Paciente = null,
                    Hospital = null
                }).ToList();

                if (listaCitas.Citas.Count == 0)
                {
                    listaCitas.Mensaje = "No hay citas registradas";
                    listaCitas.TipoMensaje = "info";
                }
            }
            catch (Exception ex)
            {
                listaCitas.Mensaje = "Error al cargar las citas: " + ex.Message;
                listaCitas.TipoMensaje = "error";
            }

            return View(listaCitas);
        }


        public ActionResult DetallesCitas(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var cita = db.Citas
                    .Include("Doctores")
                    .Include("Pacientes")
                    .Include("Hospitales")
                    .Include("Tratamientos")
                    .FirstOrDefault(c => c.id_cita == id);

                if (cita == null)
                {
                    TempData["Mensaje"] = "La cita no fue encontrada";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                ViewBag.TratamientosDetalle = ObtenerTratamientosPorCita(id.Value);

                return View(cita);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar los detalles de la cita: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        public ActionResult CrearCita()
        {
            try
            {
                CargarListasDropDown();
                var cita = new Citas  
                {
                    fecha = DateTime.Today,
                    estado = "programada",
                    fecha_creacion = DateTime.Now
                };
                return View(cita);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar el formulario de cita: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCita(Citas cita)  
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (cita.fecha < DateTime.Today)
                    {
                        ModelState.AddModelError("fecha", "La fecha de la cita no puede ser anterior a hoy");
                        CargarListasDropDown(cita.id_hospital);
                        return View(cita);
                    }

                    var citaExistente = db.Citas.FirstOrDefault(c =>
                        c.id_doctor == cita.id_doctor &&
                        c.fecha == cita.fecha &&
                        c.hora == cita.hora &&
                        c.estado != "cancelada");

                    if (citaExistente != null)
                    {
                        ModelState.AddModelError("", "El doctor ya tiene una cita programada en esa fecha y hora");
                        CargarListasDropDown(cita.id_hospital);
                        return View(cita);
                    }

                    cita.estado = "programada";
                    cita.fecha_creacion = DateTime.Now;

                    db.Citas.Add(cita);
                    db.SaveChanges();

                    TempData["Mensaje"] = "Cita creada exitosamente";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                else
                {
                    CargarListasDropDown(cita.id_hospital);
                    return View(cita);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al crear la cita: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                CargarListasDropDown(cita.id_hospital);
                return View(cita);
            }
        }

        public ActionResult EditarCita(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var cita = db.Citas.Find(id);
                if (cita == null)
                {
                    TempData["Mensaje"] = "La cita no fue encontrada";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                CargarListasDropDown(cita.id_hospital);
                return View(cita);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar la cita: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarCita(Citas cita)  
        {
            try
            {
                if (ModelState.IsValid)
                {
                    
                    if (cita.estado == "programada" && cita.fecha < DateTime.Today)
                    {
                        ModelState.AddModelError("fecha", "La fecha de la cita no puede ser anterior a hoy");
                        CargarListasDropDown(cita.id_hospital);
                        return View(cita);
                    }

                    
                    var citaExistente = db.Citas.FirstOrDefault(c =>
                        c.id_doctor == cita.id_doctor &&
                        c.fecha == cita.fecha &&
                        c.hora == cita.hora &&
                        c.estado != "cancelada" &&
                        c.id_cita != cita.id_cita);

                    if (citaExistente != null)
                    {
                        ModelState.AddModelError("", "El doctor ya tiene una cita programada en esa fecha y hora");
                        CargarListasDropDown(cita.id_hospital);
                        return View(cita);
                    }

                    db.Entry(cita).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["Mensaje"] = "Cita actualizada exitosamente";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                else
                {
                    CargarListasDropDown(cita.id_hospital);
                    return View(cita);
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al actualizar la cita: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                CargarListasDropDown(cita.id_hospital);
                return View(cita);
            }
        }



        public ActionResult Cancelar(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var cita = db.Citas.Find(id);
                if (cita == null)
                {
                    TempData["Mensaje"] = "La cita no fue encontrada";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                cita.estado = "cancelada";
                db.Entry(cita).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = "Cita cancelada exitosamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cancelar la cita: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }


        public ActionResult Completar(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            try
            {
                var cita = db.Citas.Find(id);
                if (cita == null)
                {
                    TempData["Mensaje"] = "La cita no fue encontrada";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                cita.estado = "completada";
                db.Entry(cita).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                TempData["Mensaje"] = "Cita completada exitosamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al completar la cita: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        public ActionResult GetDoctoresPorHospital(int hospitalId)
        {
            try
            {
                var doctores = db.Doctores
                    .Where(d => d.id_hospital == hospitalId && d.activo == true)
                    .Select(d => new {
                        id_doctor = d.id_doctor,
                        nombre_completo = d.nombre + " " + d.apellidos + " - " + d.especialidad
                    })
                    .ToList();

                return Json(doctores, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetPacientesPorHospital(int hospitalId)
        {
            try
            {
                var pacientes = db.Pacientes
                    .Where(p => p.id_hospital == hospitalId && p.activo == true)
                    .Select(p => new {
                        id_paciente = p.id_paciente,
                        nombre_completo = p.nombre + " " + p.apellidos
                    })
                    .ToList();

                return Json(pacientes, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);
            }
        }

        private void CargarListasDropDown(int? hospitalSeleccionado = null)
        {
            
            ViewBag.Hospitales = new SelectList(
                db.Hospitales.Where(h => h.activo == true).ToList(),
                "id_hospital",
                "nombre",
                hospitalSeleccionado
            );


            if (hospitalSeleccionado.HasValue)
            {
                ViewBag.Doctores = new SelectList(
                    db.Doctores
                        .Where(d => d.id_hospital == hospitalSeleccionado && d.activo == true)
                        .Select(d => new {
                            id_doctor = d.id_doctor,
                            nombre_completo = d.nombre + " " + d.apellidos + " - " + d.especialidad
                        }).ToList(),
                    "id_doctor",
                    "nombre_completo"
                );

                ViewBag.Pacientes = new SelectList(
                    db.Pacientes
                        .Where(p => p.id_hospital == hospitalSeleccionado && p.activo == true)
                        .Select(p => new {
                            id_paciente = p.id_paciente,
                            nombre_completo = p.nombre + " " + p.apellidos
                        }).ToList(),
                    "id_paciente",
                    "nombre_completo"
                );
            }
            else
            {
                ViewBag.Doctores = new SelectList(new List<object>(), "id_doctor", "nombre_completo");
                ViewBag.Pacientes = new SelectList(new List<object>(), "id_paciente", "nombre_completo");
            }


            ViewBag.Estados = new SelectList(new[] {
                new { value = "programada", text = "Programada" },
                new { value = "completada", text = "Completada" },
                new { value = "cancelada", text = "Cancelada" }
            }, "value", "text");
        }

        private List<TratamientoDetalle> ObtenerTratamientosPorCita(int idCita)
        {

            try
            {
                var tratamientos = db.Tratamientos
                    .Include("Citas")
                    .Include("Citas.Pacientes")
                    .Include("Citas.Doctores")
                    .Include("Tratamiento_Medicamento")
                    .Include("Tratamiento_Medicamento.Medicamentos")
                    .Where(t => t.id_cita == idCita)
                    .ToList();

                var resultado = new List<TratamientoDetalle>();

                foreach (var tratamiento in tratamientos)
                {
                    var detalle = new TratamientoDetalle
                    {
                        IdTratamiento = tratamiento.id_tratamiento,
                        Paciente = tratamiento.Citas?.Pacientes != null
                    ? $"{tratamiento.Citas.Pacientes.nombre} {tratamiento.Citas.Pacientes.apellidos}"
                    : "Paciente no disponible",
                        Doctor = tratamiento.Citas?.Doctores != null
                    ? $"{tratamiento.Citas.Doctores.nombre} {tratamiento.Citas.Doctores.apellidos}"
                    : "Doctor no disponible",
                        Descripcion = tratamiento.descripcion ?? "Sin descripción",
                        Fecha = tratamiento.fecha_tratamiento?.ToString("dd/MM/yyyy") ?? "No especificada",
                        Medicamentos = new List<TratamientoMedicamentoItem>()
                    };

                    if (tratamiento.Tratamiento_Medicamento != null && tratamiento.Tratamiento_Medicamento.Any())
                    {
                        detalle.Medicamentos = tratamiento.Tratamiento_Medicamento.Select(tm => new TratamientoMedicamentoItem
                        {
                            Nombre = tm.Medicamentos?.nombre ?? "Medicamento no disponible",
                            Cantidad = tm.cantidad,
                            CostoUnidad = tm.Medicamentos?.costo_unidad ?? 0
                        }).ToList();
                    }

                    detalle.Total = detalle.Medicamentos.Sum(m => m.Subtotal);
                    resultado.Add(detalle);
                }

                return resultado;
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al cargar los tratamientos de la cita." + ex.Message;
                TempData["TipoMensaje"] = "error";
                return new List<TratamientoDetalle>();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}    
