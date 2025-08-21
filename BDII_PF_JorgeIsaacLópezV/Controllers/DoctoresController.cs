using BDII_PF_JorgeIsaacLópezV.Models;
using BDII_PF_JorgeIsaacLópezV.Models.viewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BDII_PF_JorgeIsaacLópezV.Controllers
{
    public class DoctoresController : Controller
    {
        private GestionHospitalesEntities db = new GestionHospitalesEntities();

        // GET: Doctores
        public ActionResult Index()
        {
            var doctores = db.Doctores
                .Include("Hospitales")
                .OrderBy(d => d.nombre)
                .ToList();
            return View(doctores);
        }

        public ActionResult DetallesDoctores(int id)
        {
            var doctor = db.Doctores
                .Include("Hospitales")
                .FirstOrDefault(d => d.id_doctor == id);
            if (doctor == null) return HttpNotFound();

            return View(doctor);
        }


        public ActionResult CrearDoctor()
        {
            CargarListaHospitales();
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearDoctor(Doctor model)
        {
            if (ModelState.IsValid)
            {
                var doctor = new Doctores
                {
                    identificacion = model.Identificacion,
                    nombre = model.Nombre,
                    apellidos = model.Apellidos,
                    especialidad = model.Especialidad,
                    email = model.Email,
                    telefono = model.Telefono,
                    id_hospital = model.Id_Hospital,
                    fecha_registro = DateTime.Now,
                    activo = true
                };
                db.Doctores.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CargarListaHospitales();
            return View(model);
        }

        public ActionResult EditarDoctores(int id)
        {
            var doctor = db.Doctores.Find(id);
            if (doctor == null) return HttpNotFound();

            var vm = new Doctores
            {
                id_doctor = doctor.id_doctor,
                identificacion = doctor.identificacion,
                nombre = doctor.nombre,
                apellidos = doctor.apellidos,
                especialidad = doctor.especialidad,
                email = doctor.email,
                telefono = doctor.telefono,
                id_hospital = doctor.id_hospital,
                fecha_registro = doctor.fecha_registro,
                activo = doctor.activo
            };

            CargarListaHospitales();
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarDoctores(Doctor model)
        {
            if (ModelState.IsValid)
            {
                var doctor = db.Doctores.Find(model.Id_Doctor);
                if (doctor == null) return HttpNotFound();

                doctor.identificacion = model.Identificacion;
                doctor.nombre = model.Nombre;
                doctor.apellidos = model.Apellidos;
                doctor.especialidad = model.Especialidad;
                doctor.email = model.Email;
                doctor.telefono = model.Telefono;
                doctor.id_hospital = model.Id_Hospital;
                doctor.activo = model.Activo;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            CargarListaHospitales();
            return View(model);
        }



        public ActionResult Pacientes(int idDoctor)
        {
            var doctor = db.Doctores.Find(idDoctor);
            if (doctor == null)
            {
                TempData["Mensaje"] = "Paciente no encontrado.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
            var pacientes = db.Pacientes
                .Where(p => p.id_hospital == doctor.id_hospital && p.activo == true)
                .OrderBy(p => p.nombre)
                .ToList();

            ViewBag.Doctor = doctor;
            return View(pacientes);
        }
        
        public ActionResult CrearCita(int idDoctor, int idPaciente) 
        {
            var doctor = db.Doctores.Find(idDoctor);
            var paciente = db.Pacientes.Find(idPaciente);

            if (doctor == null || paciente == null) 
            {
                TempData["Mensaje"] = "Doctor o Paciente no encontrado.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }

            var cita = new Citas
            {
                id_doctor = idDoctor,
                id_paciente = idPaciente,
                id_hospital = doctor.id_hospital,
                fecha = DateTime.Today,
                hora = DateTime.Now.TimeOfDay,
                estado = "programado",
                fecha_creacion = DateTime.Today
            };

            return View(cita);
        }

        [HttpPost]
        public ActionResult CrearCita(Citas model)
        {
            if (ModelState.IsValid)
            {
                model.estado = "programada";
                model.fecha_creacion = DateTime.Now;

                db.Citas.Add(model);
                db.SaveChanges();
                return RedirectToAction("Pacientes", new { idDoctor = model.id_doctor });
            }
            return View(model);
        }


        public ActionResult CrearTratamiento(int idCita) 
        {
            var cita = db.Citas.Find(idCita);
            if (cita == null) 
            {
                TempData["Mensaje"] = "Cita no encontrada.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }
            var tratamiento = new TratamientoCreate
            {
                IdCita = cita.id_cita,
                Medicamentos = db.Medicamentos
                    .Where(m => m.id_hospital == cita.id_hospital && m.activo == true)
                    .Select(m => new MedicamentoSeleccion
                    {
                        IdMedicamento = m.id_medicamento,
                        Nombre = m.nombre,
                        Stock = m.stock,
                        CostoUnidad = m.costo_unidad
                    }).ToList()
            };
            return View(tratamiento);
        }

        [HttpPost]
        public ActionResult CrearTratamiento(TratamientoCreate modelo) 
        {
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Datos inválidos.";
                TempData["TipoMensaje"] = "error";
                return View(modelo);
            }

            var seleccionados = modelo.Medicamentos
                .Where(m => m.Seleccionado && m.Cantidad > 0)
                .ToList();

            if (!seleccionados.Any()) 
            {
                ModelState.AddModelError("", "Debe seleccionar al menos un medicamento.");
                return View(modelo);
            }

            var tratamiento = new Tratamientos 
            {
                id_cita = modelo.IdCita,
                descripcion = modelo.Descripcion,
                fecha_tratamiento = DateTime.Now
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

            return RedirectToAction("DetallesTratamiento", new { id = tratamiento.id_tratamiento });
        }

        public ActionResult DetallesTratamiento(int id) 
        {
            var t = db.Tratamientos
                .Include("Citas.Paciente") 
                .Include("Tratamiento_Medicamento.Medicamentos")
                .FirstOrDefault(x => x.id_tratamiento == id);

            if (t == null) 
            {
                TempData["Mensaje"] = "Tratamiento no encontrado.";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction("Index");
            }

            var tratamiento = new TratamientoDetalle
            {
                IdTratamiento = t.id_tratamiento,
                Paciente = t.Citas.Pacientes.nombre + " " + t.Citas.Pacientes.apellidos,
                Doctor = t.Citas.Doctores.nombre + " " + t.Citas.Doctores.apellidos,
                Descripcion = t.descripcion,
                Fecha = t.fecha_tratamiento.Value.ToString("dd/MM/yyyy"),
                Medicamentos = t.Tratamiento_Medicamento.Select(tm => new TratamientoMedicamentoItem
                {
                    Nombre = tm.Medicamentos.nombre,
                    Cantidad = tm.cantidad,
                    CostoUnidad = tm.Medicamentos.costo_unidad
                }).ToList(),
            };

            tratamiento.Total = tratamiento.Medicamentos.Sum(x => x.Subtotal);

            return View(tratamiento);
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
    }
}