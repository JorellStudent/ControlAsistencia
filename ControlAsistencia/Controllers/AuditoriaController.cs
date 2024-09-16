using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class AuditoriaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuditoriaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listado de auditorías
        public async Task<IActionResult> Index()
        {
            var auditorias = await _context.Auditorias.Include(a => a.Usuario).ToListAsync();
            return View(auditorias);
        }

        // Detalles de una auditoría
        public async Task<IActionResult> Detalles(int id)
        {
            var auditoria = await _context.Auditorias
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.IdAuditoria == id);

            if (auditoria == null)
            {
                return NotFound();
            }

            return View(auditoria);
        }

        // Eliminar un registro de auditoría
        public async Task<IActionResult> Eliminar(int id)
        {
            var auditoria = await _context.Auditorias.FindAsync(id);
            if (auditoria == null)
            {
                return NotFound();
            }

            return View(auditoria);
        }

        // Confirmar eliminación de un registro de auditoría
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var auditoria = await _context.Auditorias.FindAsync(id);
            if (auditoria != null)
            {
                _context.Auditorias.Remove(auditoria);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
