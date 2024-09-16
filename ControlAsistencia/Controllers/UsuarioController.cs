using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ControlAsistencia.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsuarioController> _logger; // Para registrar errores

        public UsuarioController(ApplicationDbContext context, ILogger<UsuarioController> logger)
        {
            _context = context;
            _logger = logger; // Inicialización del logger
        }

        // Lista de usuarios activos con filtro de búsqueda
        public async Task<IActionResult> Index(string searchString)
        {
            var usuarios = _context.Usuarios.Where(u => u.Activo); // Solo usuarios activos

            if (!string.IsNullOrEmpty(searchString))
            {
                // Búsqueda directamente en la base de datos (LINQ to Entities)
                searchString = searchString.ToLower();
                usuarios = usuarios.Where(s => s.Nombre.ToLower().Contains(searchString) ||
                                               s.Apellido.ToLower().Contains(searchString) ||
                                               s.RUT.Contains(searchString));
            }

            return View(await usuarios.ToListAsync());
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
                        // Limitar el tamaño del archivo (ejemplo: 2 MB)
                        if (Foto.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Foto", "El tamaño del archivo de imagen no debe superar los 2 MB.");
                            return View(usuario);
                        }

                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        // Generar un nombre de archivo único
                        var fileName = Guid.NewGuid() + Path.GetExtension(Foto.FileName);
                        var filePath = Path.Combine(directoryPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Foto.CopyToAsync(stream);
                        }
                        usuario.Foto = "/images/" + fileName; // Guardar la ruta de la imagen
                    }

                    usuario.Activo = true; // Usuario activo por defecto
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario agregado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Registrar el error
                    _logger.LogError(ex, "Error al agregar el usuario");
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
                        // Limitar el tamaño del archivo (ejemplo: 2 MB)
                        if (Foto.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Foto", "El tamaño del archivo de imagen no debe superar los 2 MB.");
                            return View(usuario);
                        }

                        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
                        if (!Directory.Exists(directoryPath))
                        {
                            Directory.CreateDirectory(directoryPath);
                        }

                        var fileName = Guid.NewGuid() + Path.GetExtension(Foto.FileName);
                        var filePath = Path.Combine(directoryPath, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await Foto.CopyToAsync(stream);
                        }
                        usuario.Foto = "/images/" + fileName;
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
                    else
                    {
                        TempData["ErrorMessage"] = "El usuario fue modificado por otro usuario. Vuelve a intentarlo.";
                        return View(usuario);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al editar el usuario");
                    TempData["ErrorMessage"] = $"Error al editar usuario: {ex.Message}";
                }
            }
            return View(usuario);
        }

        // Confirmar eliminación del usuario (GET)
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "El usuario no existe o ya ha sido eliminado.";
                return RedirectToAction(nameof(Index));
            }

            return View(usuario);
        }

        // Marcar usuario como inactivo (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
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

                TempData["SuccessMessage"] = "Usuario eliminado con éxito.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario");
                TempData["ErrorMessage"] = $"Ocurrió un error al eliminar el usuario: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
