using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlAsistencia.Controllers
{
    public class ReporteController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Reporte de asistencia por usuario
        public async Task<IActionResult> ReporteAsistencia(int idUsuario)
        {
            var asistencias = await _context.Asistencias
                .Where(a => a.IdUsuario == idUsuario)
                .Include(a => a.Usuario)
                .ToListAsync();

            return View(asistencias);
        }

        // Reporte de faltas y atrasos
        public async Task<IActionResult> ReporteFaltas()
        {
            var hoy = DateTime.Now.Date;
            var usuariosSinEntrada = await _context.Usuarios
                .Where(u => !_context.Asistencias.Any(a => a.IdUsuario == u.IdUsuario && a.Fecha.Date == hoy))
                .ToListAsync();

            return View(usuariosSinEntrada);
        }
    }
}
