using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class CredencialController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CredencialController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista de credenciales con búsqueda y filtro por rol
        public async Task<IActionResult> Index(string searchString, string roleFilter)
        {
            var credencialesQuery = _context.Credencial
                                             .Include(c => c.Usuario)
                                             .Include(c => c.Rol) // Incluir la relación con Rol
                                             .AsQueryable();

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                credencialesQuery = credencialesQuery.Where(c => c.Usuario.Nombre.ToLower().Contains(searchString) ||
                                                                 c.Usuario.Apellido.ToLower().Contains(searchString) ||
                                                                 c.NombreUsuario.ToLower().Contains(searchString));
            }

            // Aplicar filtro por rol
            if (!string.IsNullOrEmpty(roleFilter) && roleFilter != "Todos")
            {
                credencialesQuery = credencialesQuery.Where(c => c.Rol.NombreRol == roleFilter);
            }

            var credenciales = await credencialesQuery.ToListAsync();

            // Enviar la lista de roles a la vista para el filtro
            ViewBag.Roles = new SelectList(new[] { "Todos", "Administrador", "Empleado", "Jefatura" });

            return View(credenciales);
        }

        // Método para cargar Usuarios y Roles
        private void CargarUsuariosYRoles(int? idUsuarioSeleccionado = null, int? idRolSeleccionado = null)
        {
            ViewBag.Usuarios = new SelectList(_context.Usuarios.Where(u => u.Activo), "IdUsuario", "Nombre", idUsuarioSeleccionado);
            ViewBag.Roles = new SelectList(_context.Roles, "IdRol", "NombreRol", idRolSeleccionado);
        }

        // Crear credencial (GET)
        public IActionResult Crear()
        {
            // Cargar usuarios activos y roles disponibles
            CargarUsuariosYRoles();
            return View();
        }

        // Crear credencial (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdUsuario,NombreUsuario,Contrasena,IdRol")] Credencial credencial)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(credencial);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Credencial creada con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    // Manejar errores de actualización de la base de datos
                    ModelState.AddModelError("", $"Error al crear la credencial: {ex.Message}");
                }
            }

            // Si ocurre un error, recargar las listas para usuarios y roles
            CargarUsuariosYRoles(credencial.IdUsuario, credencial.IdRol);
            return View(credencial);
        }

        // Editar credencial (GET)
        public async Task<IActionResult> Editar(int id)
        {
            var credencial = await _context.Credencial.FindAsync(id);
            if (credencial == null)
            {
                TempData["ErrorMessage"] = "Credencial no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            CargarUsuariosYRoles(credencial.IdUsuario, credencial.IdRol);
            return View(credencial);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdCredencial,IdUsuario,NombreUsuario,Contrasena,IdRol")] Credencial credencial)
        {
            if (id != credencial.IdCredencial)
            {
                TempData["ErrorMessage"] = "El ID de la credencial no coincide.";
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credencial);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Credencial editada con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredencialExists(credencial.IdCredencial))
                    {
                        TempData["ErrorMessage"] = "La credencial no existe.";
                        return RedirectToAction(nameof(Index));
                    }
                    throw;
                }
                catch (DbUpdateException ex)
                {
                    TempData["ErrorMessage"] = $"Error al editar la credencial: {ex.Message}";
                }
            }

            CargarUsuariosYRoles(credencial.IdUsuario, credencial.IdRol);
            return View(credencial);
        }

        // Confirmar eliminación de credencial (GET)
        public async Task<IActionResult> Eliminar(int id)
        {
            var credencial = await _context.Credencial
                                           .Include(c => c.Usuario)
                                           .Include(c => c.Rol)
                                           .FirstOrDefaultAsync(c => c.IdCredencial == id);
            if (credencial == null)
            {
                TempData["ErrorMessage"] = "Credencial no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            return View(credencial);
        }

        // Eliminar credencial (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var credencial = await _context.Credencial.FindAsync(id);
            if (credencial == null)
            {
                TempData["ErrorMessage"] = "Credencial no encontrada.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Credencial.Remove(credencial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credencial eliminada con éxito.";
            }
            catch (DbUpdateException ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la credencial: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool CredencialExists(int id)
        {
            return _context.Credencial.Any(e => e.IdCredencial == id);
        }
    }
}
