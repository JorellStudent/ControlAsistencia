using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class PermisoController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        // Listar permisos
        public async Task<IActionResult> Index()
        {
            var permisos = await _context.Permisos.Include(p => p.Usuario).ToListAsync();
            return View(permisos);
        }

        // Crear permiso (GET)
        public IActionResult Crear()
        {
            return View();
        }

        // Crear permiso (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([Bind("IdUsuario,FechaInicio,FechaFin,TipoPermiso")] Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(permiso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(permiso);
        }

        // Editar permiso (GET)
        public async Task<IActionResult> Editar(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso == null)
            {
                return NotFound();
            }
            return View(permiso);
        }

        // Editar permiso (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, [Bind("IdPermiso,IdUsuario,FechaInicio,FechaFin,TipoPermiso")] Permiso permiso)
        {
            if (id != permiso.IdPermiso)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permiso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Permisos.Any(e => e.IdPermiso == id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(permiso);
        }

        // Cambiar estado de permiso a "Cancelado" (Eliminar lógico)
        public async Task<IActionResult> Cancelar(int id)
        {
            var permiso = await _context.Permisos.FindAsync(id);
            if (permiso != null)
            {
                // En lugar de eliminar el permiso, cambiar su estado
                // Aquí puedes agregar una propiedad Estado si no la tienes (ejemplo: Estado = "Cancelado")
                _context.Permisos.Remove(permiso); // O lógica para cambiar el estado
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
