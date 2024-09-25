using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;

namespace ControlAsistencia.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(ApplicationDbContext context, ILogger<UsuarioController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Lista de usuarios activos con filtro de búsqueda
        public async Task<IActionResult> Index(string searchString)
        {
            var usuarios = _context.Usuarios.Where(u => u.Activo);

            if (!string.IsNullOrEmpty(searchString))
            {
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

        // Crear usuario (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("Nombre,Apellido,Correo,Sexo,FechaNacimiento,Nacionalidad,RUT,Activo")] Usuario usuario, IFormFile Foto)
        {
            if (_context.Usuarios.Any(u => u.RUT == usuario.RUT))
            {
                ModelState.AddModelError("RUT", "El RUT ya está registrado.");
                return View(usuario);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Si no se sube una foto, establecer una imagen predeterminada
                    if (Foto == null || Foto.Length == 0)
                    {
                        usuario.Foto = "/images/default-user.png"; // Ruta de la imagen predeterminada
                    }
                    else
                    {
                        usuario.Foto = await GuardarFoto(Foto);
                    }

                    usuario.Activo = true; // Usuario activo por defecto
                    _context.Add(usuario);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Usuario agregado con éxito.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
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
        public async Task<IActionResult> Editar(int id, [Bind("IdUsuario,Nombre,Apellido,Correo,Sexo,FechaNacimiento,Nacionalidad,RUT,Activo")] Usuario usuario, IFormFile Foto)
        {
            if (id != usuario.IdUsuario)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Solo actualizar la foto si se ha proporcionado una nueva
                    if (Foto != null && Foto.Length > 0)
                    {
                        usuario.Foto = await GuardarFoto(Foto);
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

        // Método auxiliar para guardar la foto del usuario
        private async Task<string> GuardarFoto(IFormFile Foto)
        {
            // Verificar el tamaño del archivo
            if (Foto.Length > 2 * 1024 * 1024)
            {
                throw new Exception("El tamaño del archivo de imagen no debe superar los 2 MB.");
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

            return "/images/" + fileName; // Devolver la ruta de la imagen
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
