using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lista de usuarios activos con filtro de búsqueda
        public async Task<IActionResult> Index(string searchString)
        {
            var usuarios = await _context.Usuarios.Where(u => u.Activo).ToListAsync(); // Solo usuarios activos

            if (!string.IsNullOrEmpty(searchString))
            {
                // Búsqueda sensible a minúsculas y en memoria (cliente)
                searchString = searchString.ToLower();
                usuarios = usuarios
                    .Where(s => s.Nombre.ToLower().Contains(searchString) ||
                                s.Apellido.ToLower().Contains(searchString) ||
                                s.RUT.Contains(searchString))
                    .ToList();
            }

            return View(usuarios);
        }

        // Detalle de usuario
        public async Task<IActionResult> Detalles(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // Crear usuario (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // Crear usuario (POST) con validación de RUT único y guardado de imagen
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Apellido,Correo,Foto,Sexo,FechaNacimiento,Nacionalidad,RUT,Activo")] Usuario usuario, IFormFile Foto)
        {
            // Verificar que el RUT sea único
            if (_context.Usuarios.Any(u => u.RUT == usuario.RUT))
            {
                ModelState.AddModelError("RUT", "El RUT ya está registrado.");
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Guardar la imagen si está presente
                    if (Foto != null && Foto.Length > 0)
                    {
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        var filePath = Path.Combine(directoryPath, Foto.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Foto.CopyToAsync(stream);
                        }
                        usuario.Foto = "/images/" + Foto.FileName; // Guardar la ruta de la imagen
                    }

                    usuario.Activo = true; // Usuario activo por defecto
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario agregado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al agregar usuario: {ex.Message}";
                }
            }

            return View(usuario);
        }

        // Editar usuario (GET)
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Usuario no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // Editar usuario (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdUsuario,Nombre,Apellido,Correo,Foto,Sexo,FechaNacimiento,Nacionalidad,RUT,Activo")] Usuario usuario, IFormFile Foto)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Guardar nueva imagen si se sube una
                    if (Foto != null && Foto.Length > 0)
                    {
                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        var filePath = Path.Combine(directoryPath, Foto.FileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Foto.CopyToAsync(stream);
                        }
                        usuario.Foto = "/images/" + Foto.FileName;
                    }

                    _context.Update(usuario);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario editado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Usuarios.Any(e => e.IdUsuario == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error al editar usuario: {ex.Message}";
                }
            }
            return View(usuario);
        }

        // Confirmar eliminación del usuario (GET)
        public async Task<IActionResult> Eliminar(int id)
        {
            // Buscar el usuario por ID
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "El usuario no existe o ya ha sido eliminado.";
                return RedirectToAction(nameof(Index));
            }

            // Devolver la vista con la información del usuario
            return View(usuario);
        }

        // Marcar usuario como inactivo (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            // Buscar el usuario por ID
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "El usuario no existe o ya ha sido eliminado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Marca el usuario como inactivo en lugar de eliminarlo físicamente
                usuario.Activo = false;
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                // Mensaje de éxito
                TempData["SuccessMessage"] = "Usuario eliminado con éxito.";
            }
            catch (Exception ex)
            {
                // Manejo de errores y mensaje de error
                TempData["ErrorMessage"] = $"Ocurrió un error al eliminar el usuario: {ex.Message}";
            }

            // Redirigir a la lista de usuarios
            return RedirectToAction(nameof(Index));
        }
    }
}
