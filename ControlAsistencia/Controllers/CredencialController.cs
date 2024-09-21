using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAsistencia.Controllers
{
    public class CredencialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CredencialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para cargar ViewBags de Usuarios y Roles
        private void CargarUsuariosYRoles()
        {
            // Filtrar usuarios que no tienen credencial asociada
            var usuariosSinCredencial = _context.Usuarios
                .Where(u => u.Activo && !_context.Credencial.Any(c => c.IdUsuario == u.IdUsuario))
                .ToList();

            ViewBag.Usuarios = new SelectList(usuariosSinCredencial, "IdUsuario", "Nombre");
            ViewBag.Roles = new SelectList(_context.Roles.ToList(), "IdRol", "NombreRol");
        }

        // Acción para mostrar el formulario de creación de una nueva credencial
        public IActionResult Crear()
        {
            CargarUsuariosYRoles();
            return View();
        }

        // POST: Credencial/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Credencial credencial)
        {
            if (!ModelState.IsValid)
            {
                CargarUsuariosYRoles();
                return View(credencial);
            }

            // Validar que no exista un nombre de usuario duplicado
            if (_context.Credencial.Any(c => c.NombreUsuario == credencial.NombreUsuario))
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está en uso.");
                CargarUsuariosYRoles();
                return View(credencial);
            }

            try
            {
                _context.Add(credencial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credencial creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al intentar guardar los cambios: {ex.Message}");
            }

            CargarUsuariosYRoles();
            return View(credencial);
        }

        // Acción para mostrar el formulario de edición de una credencial
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credencial = await _context.Credencial.FindAsync(id);
            if (credencial == null)
            {
                return NotFound();
            }

            CargarUsuariosYRoles();
            return View(credencial);
        }

        // POST: Credencial/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Credencial credencial)
        {
            if (id != credencial.IdCredencial)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                CargarUsuariosYRoles();
                return View(credencial);
            }

            // Validar que no se repita el nombre de usuario si se está editando
            if (_context.Credencial.Any(c => c.NombreUsuario == credencial.NombreUsuario && c.IdCredencial != id))
            {
                ModelState.AddModelError("NombreUsuario", "El nombre de usuario ya está en uso por otra credencial.");
                CargarUsuariosYRoles();
                return View(credencial);
            }

            try
            {
                _context.Update(credencial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credencial editada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al intentar guardar los cambios: {ex.Message}");
            }

            CargarUsuariosYRoles();
            return View(credencial);
        }

        // Acción para mostrar la lista de credenciales
        public async Task<IActionResult> Index()
        {
            var credenciales = await _context.Credencial
                .Include(c => c.Rol)
                .Include(c => c.Usuario)
                .ToListAsync();

            return View(credenciales);
        }

        // Acción para mostrar el formulario de eliminación de una credencial
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credencial = await _context.Credencial
                .Include(c => c.Rol)
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.IdCredencial == id);

            if (credencial == null)
            {
                return NotFound();
            }

            return View(credencial);
        }

        // POST: Credencial/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var credencial = await _context.Credencial.FindAsync(id);
                if (credencial == null)
                {
                    ModelState.AddModelError("", "No se encontró la credencial que intentas eliminar.");
                    return RedirectToAction(nameof(Index));
                }

                _context.Credencial.Remove(credencial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credencial eliminada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al intentar eliminar la credencial: {ex.Message}");
                return View();
            }
        }
    }
}
