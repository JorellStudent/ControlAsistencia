using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar el formulario de inicio de sesión del administrador
        public IActionResult Login()
        {
            return View();
        }

        // Manejar la validación del inicio de sesión del administrador
        [HttpPost]
        public async Task<IActionResult> Login(string NombreUsuario, string Contrasena)
        {
            // Validar si los campos están vacíos
            if (string.IsNullOrEmpty(NombreUsuario) || string.IsNullOrEmpty(Contrasena))
            {
                ViewBag.Message = "Por favor, ingrese un nombre de usuario y una contraseña.";
                return View();
            }

            // Buscar la credencial en la base de datos con el rol correspondiente
            var credencial = await _context.Credencial
                .Include(c => c.Rol)
                .FirstOrDefaultAsync(c => c.NombreUsuario == NombreUsuario && c.Contrasena == Contrasena);

            if (credencial == null || credencial.Rol.NombreRol != "Administrador")
            {
                ViewBag.Message = "Credenciales de administrador inválidas.";
                return View();
            }

            // Si las credenciales son válidas, redirigir al panel de administración
            return RedirectToAction("Dashboard");
        }

        // Acción para mostrar el panel de administración
        public IActionResult Dashboard()
        {
            return View();
        }

        // Acción para manejar la gestión de usuarios
        public async Task<IActionResult> GestionUsuarios()
        {
            try
            {
                var usuarios = await _context.Usuarios.ToListAsync();
                return View(usuarios);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar la lista de usuarios: " + ex.Message;
                return View("Error");
            }
        }

        // Acción para manejar la gestión de credenciales
        public async Task<IActionResult> GestionCredenciales()
        {
            try
            {
                var credenciales = await _context.Credencial.ToListAsync();
                return View(credenciales);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar la lista de credenciales: " + ex.Message;
                return View("Error");
            }
        }

        // Acción para gestionar horarios de trabajo
        public async Task<IActionResult> HorariosTrabajo()
        {
            try
            {
                var horarios = await _context.HorariosTrabajo
                    .Include(h => h.Usuario)
                    .ToListAsync();
                return View(horarios);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar los horarios de trabajo: " + ex.Message;
                return View("Error");
            }
        }

        // Acción para ver reportes de asistencia
        public async Task<IActionResult> ReportesAsistencia()
        {
            try
            {
                var asistencias = await _context.Asistencias
                    .Include(a => a.Usuario)
                    .ToListAsync();
                return View(asistencias);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar los reportes de asistencia: " + ex.Message;
                return View("Error");
            }
        }
    }
}
