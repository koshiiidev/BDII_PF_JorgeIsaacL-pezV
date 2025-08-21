using BDII_PF_JorgeIsaacLópezV.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BDII_PF_JorgeIsaacLópezV.Models.viewModels;

namespace BDII_PF_JorgeIsaacLópezV.Controllers
{
    public class HospitalesController : Controller
    {
       private GestionHospitalesEntities db = new GestionHospitalesEntities();

        // GET: Hospitales
        public ActionResult Index()
        {
            var hospitales = ObtenerListaHospitales();
            var modelo = new ListaHospitales
            {
                Hospitales = hospitales
            };

            if (TempData["Mensaje"] != null)
            {
                modelo.Mensaje = TempData["Mensaje"].ToString();
                modelo.TipoMensaje = TempData["TipoMensaje"]?.ToString() ?? "info";
            }

            return View(modelo);
        }

        public ActionResult CrearHospital()
        {
            var modelo = new Hospital();
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearHospital(Hospital modelo)
        {
            if (ModelState.IsValid)
            {
                try 
                {
                    if ( db.Hospitales.Any(h => h.nombre == modelo.Nombre)) 
                    {
                        ModelState.AddModelError("Nombre", "Ya existe un hospital con ese nombre.");
                        return View(modelo);
                    }

                    var hospital = new Hospitales
                    {
                        nombre = modelo.Nombre,
                        direccion = modelo.Direccion,
                        telefono = modelo.Telefono,
                        fecha_creacion = DateTime.Now,
                        activo = true
                    };

                    db.Hospitales.Add(hospital);
                    db.SaveChanges();

                    TempData["Mensaje"] = "Hospital creado exitosamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el hospital: " + ex.Message);
                    return View(modelo);
                }
            }
            return View(modelo);
        }

        public ActionResult EditarHospital(int id) 
        {
            try 
            {
                var hospital = db.Hospitales
                    .Where(h => h.id_hospital == id)
                    .FirstOrDefault();

                if (hospital == null) 
                {
                    TempData["Mensaje"] = "Hospital no encontrado.";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }
                var modelo = new Hospital
                {
                    Id_Hospital = hospital.id_hospital,
                    Nombre = hospital.nombre,
                    Direccion = hospital.direccion,
                    Telefono = hospital.telefono,
                    Fecha_Creacion = hospital.fecha_creacion ?? DateTime.Now,
                    Activo = hospital.activo ?? true
                };
                return View(modelo);

            } 
            catch (Exception ex) 
            {
                TempData["Mensaje"] = "Error al cargar el cliente: " + ex.Message;
                TempData["Mensaje"] = "Error al cargar el hospital.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarHospital (Hospital modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var hospital = db.Hospitales.Find(modelo.Id_Hospital);
                    if (hospital != null)
                    {
                        hospital.nombre = modelo.Nombre;
                        hospital.direccion = modelo.Direccion;
                        hospital.telefono = modelo.Telefono;
                        hospital.activo = modelo.Activo;

                        db.Entry(hospital).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();

                        TempData["Mensaje"] = "Hospital actualizado exitosamente.";
                        TempData["TipoMensaje"] = "success";
                        return RedirectToAction("Index");

                    }
                    else 
                    {
                        TempData["Mensaje"] = "Hospital no encontrado";
                        TempData["TipoMensaje"] = "error";
                        return RedirectToAction("Index");
                    } 
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el hospital: " + ex.Message);
                }
            }
            return View(modelo);
        }

        public ActionResult DetallesHospital(int id) 
        {
            try 
            {
                var hospital = db.Hospitales
                    .Where(h => h.id_hospital == id)
                    .FirstOrDefault();

                if (hospital == null) 
                {
                    TempData["Mensaje"] = "Hospital no encontrado.";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                var estadisticas = ObtenerEstadisticasHospital(id);

                var modelo = new Hospital
                {
                    Id_Hospital = hospital.id_hospital,
                    Nombre = hospital.nombre,
                    Direccion = hospital.direccion,
                    Telefono = hospital.telefono,
                    Fecha_Creacion = hospital.fecha_creacion ?? DateTime.Now,
                    Activo = hospital.activo ?? true
                };

                ViewBag.TotalDoctores = estadisticas.TotalDoctores;
                ViewBag.TotalPacientes = estadisticas.TotalPacientes;
                ViewBag.TotalMedicamentos = estadisticas.TotalMedicamentos;
                ViewBag.TotalCitas = estadisticas.TotalCitas;

                return View(modelo);
            }
            catch (Exception ex) 
            {
                TempData["Mensaje"] = "Error al cargar el hospital: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }

        #region Metodos Privados

        private List<Hospital> ObtenerListaHospitales()
        {
            return db.Hospitales
                .Where(h => h.activo == true)
                .Select(h => new Hospital
                {
                    Id_Hospital = h.id_hospital,
                    Nombre = h.nombre,
                    Direccion = h.direccion,
                    Telefono = h.telefono,
                    Fecha_Creacion = h.fecha_creacion ?? DateTime.Now,
                    Activo = h.activo ?? true
                })
                .OrderBy(h => h.Nombre)
                .ToList();
        }

        private EstadisticasHospital ObtenerEstadisticasHospital(int idHospital)
        {
            return new EstadisticasHospital
            {
                TotalDoctores = db.Doctores.Count(d => d.id_hospital == idHospital && (d.activo == true || d.activo == null)),
                TotalPacientes = db.Pacientes.Count(p => p.id_hospital == idHospital && (p.activo == true || p.activo == null)),
                TotalMedicamentos = db.Medicamentos.Count(m => m.id_hospital == idHospital && (m.activo == true || m.activo == null)),
                TotalCitas = db.Citas.Count(c => c.id_hospital == idHospital && (c.estado == "Programada"|| c.estado == null))
            };
        }



        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }


    public class EstadisticasHospital
    {
        public int TotalDoctores { get; set; }
        public int TotalPacientes { get; set; }
        public int TotalMedicamentos { get; set; }
        public int TotalCitas { get; set; }
    }
}
    



