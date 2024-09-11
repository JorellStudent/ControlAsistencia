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
    public class UsuarioController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Lista de usuarios activos con filtro de búsqueda
        public async Task<IActionResult> Index(string searchString)
        {
            var usuarios = _context.Usuarios.Where(u => u.Activo == true); // Solo usuarios activos

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                usuarios = usuarios.Where(s => s.Nombre.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
                                               s.Apellido.Contains(searchString, StringComparison.CurrentCultureIgnoreCase) ||
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
                return NotFound();
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
            return View(usuario);
        }

        // Editar usuario (GET)
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
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
            }
            return View(usuario);
        }

        // Marcar usuario como inactivo (GET)
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            return View(usuario);
        }

        // Marcar usuario como inactivo (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                usuario.Activo = false; // Marcamos el usuario como inactivo
                _context.Update(usuario);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Usuario eliminado con éxito.";
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
