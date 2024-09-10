using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ControlAsistencia.Controllers
{
    public class HomeController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Acción para la página de inicio (marcación)
        public IActionResult Index()
        {
            return View();
        }

        // Acción para manejar la marcación de entrada
        [HttpPost]
        public IActionResult MarcarEntrada(string NombreUsuario, string Contrasena)
        {
            // Buscar la credencial del usuario en la base de datos
            var credencial = _context.Credencial
                .FirstOrDefault(c => c.NombreUsuario == NombreUsuario && c.Contrasena == Contrasena);

            if (credencial == null)
            {
                ViewBag.Message = "Credenciales inválidas. Usuario o contraseña incorrectos.";
                ViewBag.EntradaMarcada = false;
                return View("Index");
            }

            // Buscar el usuario asociado a las credenciales
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == credencial.IdUsuario);
            if (usuario == null)
            {
                ViewBag.Message = "Usuario no encontrado.";
                ViewBag.EntradaMarcada = false;
                return View("Index");
            }

            // Verificar si ya marcó entrada hoy
            var asistenciaHoy = _context.Asistencias
                .FirstOrDefault(a => a.IdUsuario == usuario.IdUsuario && a.Fecha.Date == DateTime.Now.Date);

            if (asistenciaHoy != null)
            {
                ViewBag.Message = "Ya has marcado tu entrada hoy.";
                ViewBag.EntradaMarcada = true;
                ViewBag.NombreUsuario = NombreUsuario;
                ViewBag.Contrasena = Contrasena;
                return View("Index");
            }

            // Si no ha marcado entrada, registrar la entrada
            var asistencia = new Asistencia
            {
                IdUsuario = usuario.IdUsuario,
                Usuario = usuario,  // Asignamos el objeto Usuario completo
                Fecha = DateTime.Now,
                HoraEntrada = DateTime.Now.TimeOfDay
            };
            _context.Asistencias.Add(asistencia);
            _context.SaveChanges();

            ViewBag.Message = "Entrada marcada correctamente.";
            ViewBag.EntradaMarcada = true;
            ViewBag.NombreUsuario = NombreUsuario;
            ViewBag.Contrasena = Contrasena;

            return View("Index");
        }

        // Acción para manejar la marcación de salida
        [HttpPost]
        public IActionResult MarcarSalida(string NombreUsuario, string Contrasena)
        {
            var credencial = _context.Credencial
                .FirstOrDefault(c => c.NombreUsuario == NombreUsuario && c.Contrasena == Contrasena);

            if (credencial == null)
            {
                ViewBag.Message = "Credenciales inválidas.";
                ViewBag.EntradaMarcada = false;
                return View("Index");
            }

            var asistencia = _context.Asistencias
                .Where(a => a.IdUsuario == credencial.IdUsuario && a.Fecha.Date == DateTime.Now.Date && a.HoraSalida == null)
                .FirstOrDefault();

            if (asistencia == null)
            {
                ViewBag.Message = "No has marcado tu entrada hoy o ya has registrado tu salida.";
                ViewBag.EntradaMarcada = false;
                return View("Index");
            }

            // Registrar salida
            asistencia.HoraSalida = DateTime.Now.TimeOfDay;
            _context.SaveChanges();

            ViewBag.Message = "Salida marcada correctamente.";
            ViewBag.EntradaMarcada = false;

            return View("Index");
        }

        // Acción para el login de administrador
        [HttpPost]
        public IActionResult AdminLogin(string NombreUsuario, string Contrasena)
        {
            var credencial = _context.Credencial
                .Include(c => c.Rol)
                .FirstOrDefault(c => c.NombreUsuario == NombreUsuario && c.Contrasena == Contrasena);

            if (credencial == null || credencial.Rol.NombreRol != "Administrador")
            {
                ViewBag.Message = "Credenciales de administrador inválidas.";
                return View("Index");
            }

            // Redirigir a la página de administración si las credenciales son válidas
            return RedirectToAction("AdminDashboard", "Admin");
        }
    }
}
