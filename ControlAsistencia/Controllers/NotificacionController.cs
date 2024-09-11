using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class NotificacionController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Listar notificaciones
        public async Task<IActionResult> Index()
        {
            var notificaciones = await _context.Notificaciones
                                               .Include(n => n.Usuario)
                                               .ToListAsync();
            return View(notificaciones);
        }

        // Crear notificación (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // Crear notificación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdUsuario,TipoNotificacion,FechaEnvio,Estado")] Notificacion notificacion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(notificacion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(notificacion);
        }

        // Editar notificación (GET)
        public async Task<IActionResult> Editar(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion == null)
            {
                return NotFound();
            }
            return View(notificacion);
        }

        // Editar notificación (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdNotificacion,IdUsuario,TipoNotificacion,FechaEnvio,Estado")] Notificacion notificacion)
        {
            if (id != notificacion.IdNotificacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(notificacion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Notificaciones.Any(e => e.IdNotificacion == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(notificacion);
        }

        // Cambiar estado de notificación a "Leída" (Eliminar lógico)
        public async Task<IActionResult> CambiarEstado(int id)
        {
            var notificacion = await _context.Notificaciones.FindAsync(id);
            if (notificacion != null)
            {
                // Cambiamos el estado a true para marcarlo como "Leída"
                notificacion.Estado = true; // Estado es de tipo bool
                _context.Update(notificacion);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
