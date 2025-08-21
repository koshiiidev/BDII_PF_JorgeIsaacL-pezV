using BDII_PF_JorgeIsaacLópezV.Models;
using BDII_PF_JorgeIsaacLópezV.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace BDII_PF_JorgeIsaacLópezV.Controllers
{
    public class PacientesController : Controller
    {
        private GestionHospitalesEntities db = new GestionHospitalesEntities();

        // GET: Paciente
        public ActionResult Index()
        {
            var pacientes = ObtenerListaPacientes();
            var modelo = new ListaPacientes
            {
                Pacientes = pacientes
            };

            if (TempData["Mensaje"] != null) 
            {
                modelo.Mensaje = TempData["Mensaje"].ToString();
                modelo.TipoMensaje = TempData["TipoMensaje"]?.ToString() ?? "info";
            }

            return View(modelo);

        }

        public ActionResult CrearPaciente()
        {
            var modelo = new Paciente();

            CargarListaHospitales();
            return View(modelo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearPaciente(Paciente modelo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (db.Pacientes.Any(p => p.identificacion == modelo.Identificacion))
                    {
                        ModelState.AddModelError("Nombre", "Ya existe un paciente con esta identificacion");
                        
                        CargarListaHospitales();
                        return View(modelo);
                    }

                    var paciente = new Pacientes
                    {
                        nombre = modelo.Nombre,
                        apellidos = modelo.Apellidos,
                        identificacion = modelo.Identificacion,
                        fecha_nacimiento = modelo.Fecha_Nacimiento,
                        genero = modelo.Genero.ToString(),
                        direccion = modelo.Direccion,
                        telefono = modelo.Telefono,
                        id_hospital = modelo.Id_Hospital,
                        fecha_registro = DateTime.Now,
                        activo = true
                    };

                    db.Pacientes.Add(paciente);
                    db.SaveChanges();

                    TempData["Mensaje"] = "Paciente creado correctamente.";
                    TempData["TipoMensaje"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al crear el paciente: " + ex.Message);
                }
            }
            CargarListaHospitales();
            return View(modelo);
        }

        public ActionResult EditarPaciente(int id)
        {
            try 
            {
                var paciente = db.Pacientes
                    .Where(p => p.id_paciente == id)
                    .FirstOrDefault();

                if (paciente == null) 
                {
                    TempData["Mensaje"] = "Paciente no encontrado.";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }
                var modelo = new Paciente
                {
                    Id_Paciente = paciente.id_paciente,
                    Nombre = paciente.nombre,
                    Apellidos = paciente.apellidos,
                    Identificacion = paciente.identificacion,
                    Fecha_Nacimiento = paciente.fecha_nacimiento,
                    Genero = !string.IsNullOrEmpty(paciente.genero) ? paciente.genero[0] : 'M',
                    Direccion = paciente.direccion,
                    Telefono = paciente.telefono,
                    Id_Hospital = paciente.id_hospital,
                    Activo = paciente.activo ?? true
                };

                CargarListaHospitales();
                return View(modelo);

            } 
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al cargar el paciente: " + ex.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPaciente(Paciente modelo) 
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var paciente = db.Pacientes.Find(modelo.Id_Paciente);
                    if (paciente != null)
                    {
                        paciente.nombre = modelo.Nombre;
                        paciente.apellidos = modelo.Apellidos;
                        paciente.identificacion = modelo.Identificacion;
                        paciente.fecha_nacimiento = modelo.Fecha_Nacimiento;
                        paciente.genero = modelo.Genero.ToString();
                        paciente.direccion = modelo.Direccion;
                        paciente.telefono = modelo.Telefono;
                        paciente.id_hospital = modelo.Id_Hospital;
                        paciente.activo = modelo.Activo;


                        db.Entry(paciente).State = EntityState.Modified;
                        db.SaveChanges();


                        TempData["Mensaje"] = "Paciente actualizado correctamente.";
                        TempData["TipoMensaje"] = "success";
                        return RedirectToAction("Index");
                    }
                    else 
                    {
                        TempData["Mensaje"] = "Paciente no encontrado";
                        TempData["TipoMensaje"] = "error";
                        return RedirectToAction("Index");
                    }
                    
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error al actualizar el paciente: " + ex.Message);
                }
            }
            CargarListaHospitales();
            return View(modelo);
        }

        public ActionResult DetallesPacientes(int id) 
        {
            try 
            {
                var paciente = db.Pacientes
                    .Where(p => p.id_paciente == id)
                    .FirstOrDefault();

                if (paciente == null)
                {
                    TempData["Mensaje"] = "Paciente no encontrado.";
                    TempData["TipoMensaje"] = "error";
                    return RedirectToAction("Index");
                }

                var estadisticas = ObtenerEstadisticasPaciente(id);

                var modelo = new Paciente
                {
                    Id_Paciente = paciente.id_paciente,
                    Nombre = paciente.nombre,
                    Apellidos = paciente.apellidos,
                    Identificacion = paciente.identificacion,
                    Fecha_Nacimiento = paciente.fecha_nacimiento,
                    Genero = !string.IsNullOrEmpty(paciente.genero) ? paciente.genero[0] : 'M',
                    Direccion = paciente.direccion,
                    Telefono = paciente.telefono,
                    Id_Hospital = paciente.id_hospital,
                    Fecha_Registro = paciente.fecha_registro ?? DateTime.Now,
                    Activo = paciente.activo ?? true
                };

                ViewBag.TotalCitas = estadisticas.TotalCitas;
                ViewBag.TotalPagos = estadisticas.TotalPagos;
                ViewBag.TotalTratamientos = estadisticas.TotalTratamientos;
                ViewBag.TotalDoctores = estadisticas.TotalDoctores;

                return View(modelo);
            }
            catch (Exception ex) 
            {
                TempData["Mensaje"] = "Error al cargar el paciente: " + ex.Message;
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
        }


        public ActionResult Citas(int id)
        {
            var paciente = db.Pacientes.Find(id);
            if (paciente == null)
            {
                TempData["Mensaje"] = "Paciente no encontrado.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }

            var citas = db.Citas
                .Include("Doctores")
                .Include("Hospitales")
                .Where(c => c.id_paciente == id)
                .OrderByDescending(c => c.fecha)
                .ToList();

            ViewBag.Paciente = paciente;
            return View(citas);
        }
        public ActionResult Pagos(int id)
        {
            var paciente = db.Pacientes.Find(id);
            if (paciente == null)
            {
                TempData["Mensaje"] = "Paciente no encontrado.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
            var pagos = db.Pagos
                .Include("Tratamientos")
                .Where(p => p.id_paciente == id)
                .OrderByDescending(p => p.fecha)
                .ToList();

            ViewBag.Paciente = paciente;
            return View(pagos);
        }


        

        #region Metodos Privados

        private List<Paciente> ObtenerListaPacientes()
        {
            var pacientes = db.Pacientes
                .Include("Hospitales")
                .Where(p => p.activo == true || p.activo == null)
                .OrderBy(p => p.nombre)
                .ToList();

            return pacientes.Select(p => new Paciente
                {
                    Id_Paciente = p.id_paciente,
                    Nombre = p.nombre,
                    Apellidos = p.apellidos,
                    Identificacion = p.identificacion,
                    Fecha_Nacimiento = p.fecha_nacimiento,
                    Genero = !string.IsNullOrEmpty(p.genero) ? p.genero[0] : 'M',
                    Direccion = p.direccion,
                    Telefono = p.telefono,
                    Id_Hospital = p.id_hospital,
                    Fecha_Registro = p.fecha_registro ?? DateTime.Now,
                    Activo = p.activo ?? true    
                })
                .ToList();
        }

        private void CargarListaHospitales()
        {
            ViewBag.Hospitales = db.Hospitales
                .Where(h => h.activo == true || h.activo == null)
                .OrderBy(h => h.nombre)
                .Select(h => new SelectListItem
                {
                    Value = h.id_hospital.ToString(),
                    Text = h.nombre
                })
                .ToList();
        }


        private EstadisticasPaciente ObtenerEstadisticasPaciente(int idPaciente)
        {
            return new EstadisticasPaciente
            {
                TotalCitas = db.Citas.Count(c => c.id_paciente == idPaciente),
                TotalPagos = db.Pagos.Count(p => p.id_paciente == idPaciente),
                TotalTratamientos = db.Tratamientos.Count(t => t.id_tratamiento == idPaciente),
                TotalDoctores = db.Citas.Where(c => c.id_paciente == idPaciente).Select(c => c.id_doctor).Distinct().Count()
            };
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }

        public class EstadisticasPaciente
        {
            public int TotalCitas { get; set; }
            public int TotalPagos { get; set; }
            public int TotalTratamientos { get; set; }
            public int TotalDoctores { get; set; }
        }
    }
}