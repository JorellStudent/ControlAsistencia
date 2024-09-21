using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Obtener las asistencias con los datos del usuario
            var asistencias = await _context.Asistencias
                .Include(a => a.Usuario)
                .ToListAsync();

            return View(asistencias);
        }

        public async Task<IActionResult> Detalles(int id)
        {
            var asistencia = await _context.Asistencias
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.IdAsistencia == id);

            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }
    }
}
