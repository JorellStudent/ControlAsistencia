using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

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
            var usuarios = _context.Usuarios.Where(u => u.Activo).ToList();
            var roles = _context.Roles.ToList();

            Console.WriteLine($"Usuarios: {usuarios.Count}, Roles: {roles.Count}");

            ViewBag.Usuarios = new SelectList(usuarios, "IdUsuario", "Nombre");
            ViewBag.Roles = new SelectList(roles, "IdRol", "NombreRol");
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
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);  // Puedes cambiar esto para que se muestre en la interfaz o lo manejes en el log.
                }

                CargarUsuariosYRoles();  // Volver a cargar los ViewBags si falla la validación
                return View(credencial);
            }

            try
            {
                _context.Add(credencial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credencial creada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", $"Error al intentar guardar los cambios: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error inesperado: {ex.Message}");
            }

            CargarUsuariosYRoles();  // Volver a cargar los ViewBags si falla la validación
            return View(credencial);
        }


        // Acción para mostrar la lista de credenciales
        public async Task<IActionResult> Index()
        {
            try
            {
                // Cargar las credenciales junto con los roles y usuarios asociados
                var credenciales = await _context.Credencial
                    .Include(c => c.Rol)
                    .Include(c => c.Usuario)
                    .ToListAsync();
                return View(credenciales);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar los datos de las credenciales: {ex.Message}");
                return View();
            }
        }

        // Acción para mostrar el formulario de edición de una credencial
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var credencial = await _context.Credencial.FindAsync(id);
                if (credencial == null)
                {
                    return NotFound();
                }

                CargarUsuariosYRoles();
                return View(credencial);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al intentar cargar los datos de la credencial: {ex.Message}");
                return View();
            }
        }

        // POST: Credencial/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdCredencial,NombreUsuario,Contrasena,IdRol,IdUsuario")] Credencial credencial)
        {
            if (id != credencial.IdCredencial)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);  // Aquí puedes mostrar los errores de validación.
                }

                CargarUsuariosYRoles();  // Volver a cargar los ViewBags si falla la validación
                return View(credencial);
            }

            try
            {
                _context.Update(credencial);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Credencial editada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError("", $"Error al intentar guardar los cambios: {dbEx.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error inesperado: {ex.Message}");
            }

            CargarUsuariosYRoles();  // Volver a cargar los ViewBags si falla la validación
            return View(credencial);
        }



        // Acción para mostrar el formulario de eliminación de una credencial
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
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
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al intentar cargar los datos de la credencial: {ex.Message}");
                return View();
            }
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
                    return NotFound();
                }

                _context.Credencial.Remove(credencial);  // Eliminar la credencial de la base de datos
                await _context.SaveChangesAsync();  // Guardar cambios
                TempData["SuccessMessage"] = "Credencial eliminada exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al intentar eliminar la credencial: {ex.Message}");
                return View();
            }
        }

        // Método para verificar si una credencial existe en la base de datos
        private bool CredencialExiste(int id)
        {
            return _context.Credencial.Any(e => e.IdCredencial == id);
        }
    }
}
