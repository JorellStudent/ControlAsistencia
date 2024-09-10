using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ControlAsistencia.Controllers
{
    public class AdminController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Mostrar el formulario de inicio de sesión del administrador
        public IActionResult Login()
        {
            return View();
        }

        // Manejar la validación del inicio de sesión del administrador
        [HttpPost]
        public IActionResult Login(string NombreUsuario, string Contrasena)
        {
            var credencial = _context.Credencial
                .Include(c => c.Rol)
                .FirstOrDefault(c => c.NombreUsuario == NombreUsuario && c.Contrasena == Contrasena);

            if (credencial == null || credencial.Rol.NombreRol != "Administrador")
            {
                ViewBag.Message = "Credenciales de administrador inválidas.";
                return View();
            }

            // Si las credenciales son válidas, redirigir al dashboard del administrador
            return RedirectToAction("Dashboard");
        }

        // Página principal del administrador
        public IActionResult Dashboard()
        {
            // Aquí iría el contenido para la administración
            return View();
        }
    }
}
