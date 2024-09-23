using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data;
using ControlAsistencia.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ControlAsistencia.Controllers
{
    public class HorarioTrabajoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HorarioTrabajoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar la lista de horarios de trabajo con filtros.
        public async Task<IActionResult> Index(string searchNombre = "", string searchDia = "")
        {
            var horarios = _context.HorariosTrabajo
                .Include(h => h.Usuario)
                .AsQueryable();

            // Filtro por nombre de usuario.
            if (!string.IsNullOrEmpty(searchNombre))
            {
                horarios = horarios.Where(h => h.Usuario.Nombre.Contains(searchNombre) || h.Usuario.Apellido.Contains(searchNombre));
            }

            // Filtro por día de la semana.
            if (!string.IsNullOrEmpty(searchDia))
            {
                horarios = horarios.Where(h => h.DiaSemana == searchDia);
            }

            ViewBag.searchNombre = searchNombre;
            ViewBag.searchDia = searchDia;

            return View(await horarios.ToListAsync());
        }

        // Acción para mostrar el formulario de creación de un nuevo horario.
        public async Task<IActionResult> Crear()
        {
            await CargarUsuariosConCredencialAsync(); // Cargar usuarios con credencial en ViewBag.
            return View(new HorarioTrabajo());
        }

        // POST: Crear nuevo horario.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(HorarioTrabajo horario)
        {
            if (!ModelState.IsValid)
            {
                await CargarUsuariosConCredencialAsync();
                return View(horario); // Si hay errores, se devuelve el formulario con los datos ingresados.
            }

            try
            {
                _context.HorariosTrabajo.Add(horario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Horario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al crear el horario: " + ex.Message);
                await CargarUsuariosConCredencialAsync();
                return View(horario);
            }
        }

        // Método para cargar solo usuarios con credencial en el ViewBag (async).
        private async Task CargarUsuariosConCredencialAsync()
        {
            var usuariosConCredencial = await _context.Usuarios
                .Where(u => u.Activo && _context.Credencial.Any(c => c.IdUsuario == u.IdUsuario))
                .ToListAsync();

            ViewBag.Usuarios = usuariosConCredencial.Any()
                ? new SelectList(usuariosConCredencial, "IdUsuario", "Nombre")
                : new SelectList(Enumerable.Empty<SelectListItem>());
        }

        // Acción para mostrar el formulario de edición de un horario existente.
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _context.HorariosTrabajo.FindAsync(id);
            if (horario == null)
            {
                return NotFound();
            }

            await CargarUsuariosConCredencialAsync(); // Cargar usuarios en ViewBag.
            return View(horario);
        }

        // POST: Editar horario existente.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, HorarioTrabajo horario)
        {
            if (id != horario.IdHorario)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await CargarUsuariosConCredencialAsync();
                return View(horario);
            }

            try
            {
                _context.HorariosTrabajo.Update(horario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Horario editado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HorarioTrabajoExists(horario.IdHorario))
                {
                    return NotFound();
                }

                ModelState.AddModelError("", "Error de concurrencia. Intente nuevamente.");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al editar el horario: " + ex.Message);
            }

            await CargarUsuariosConCredencialAsync();
            return View(horario);
        }

        // GET: Mostrar vista de confirmación de eliminación.
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var horario = await _context.HorariosTrabajo
                .Include(h => h.Usuario)
                .FirstOrDefaultAsync(h => h.IdHorario == id);

            if (horario == null)
            {
                return NotFound();
            }

            return View(horario);
        }

        // POST: Confirmar eliminación de un horario.
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            var horario = await _context.HorariosTrabajo.FindAsync(id);
            if (horario == null)
            {
                TempData["ErrorMessage"] = "El horario que intentas eliminar no existe.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.HorariosTrabajo.Remove(horario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Horario eliminado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el horario: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Método para verificar si el horario existe.
        private bool HorarioTrabajoExists(int id)
        {
            return _context.HorariosTrabajo.Any(e => e.IdHorario == id);
        }
    }
}
