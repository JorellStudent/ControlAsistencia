using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ControlAsistencia.Controllers
{
    public class HorarioTrabajoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HorarioTrabajoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Método para cargar ViewBags de Usuarios (empleados)
        private void CargarUsuarios()
        {
            ViewBag.Usuarios = new SelectList(_context.Usuarios.Where(u => u.Activo), "IdUsuario", "Nombre");
        }

        // Acción para listar los horarios de trabajo (Index)
        public async Task<IActionResult> Index()
        {
            try
            {
                var horarios = await _context.HorariosTrabajo
                    .Include(h => h.Usuario)
                    .ToListAsync();
                return View(horarios);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar los horarios: {ex.Message}");
                return View("Error");
            }
        }

        // Acción para mostrar el formulario de creación de un nuevo horario (GET)
        public IActionResult Crear()
        {
            CargarUsuarios();
            return View();
        }

        // Acción para procesar la creación de un nuevo horario (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(HorarioTrabajo horario)
        {
            if (!ModelState.IsValid)
            {
                CargarUsuarios();
                return View(horario);
            }

            try
            {
                _context.Add(horario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Horario de trabajo creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al intentar crear el horario: {ex.Message}");
                CargarUsuarios();
                return View(horario);
            }
        }

        // Acción para mostrar el formulario de edición de un horario existente (GET)
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID de horario no proporcionado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var horario = await _context.HorariosTrabajo.FindAsync(id);
                if (horario == null)
                {
                    TempData["ErrorMessage"] = "Horario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                CargarUsuarios();
                return View(horario);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar el horario: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // Acción para procesar la edición de un horario (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, HorarioTrabajo horario)
        {
            if (id != horario.IdHorario)
            {
                TempData["ErrorMessage"] = "ID de horario no coincide.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                CargarUsuarios();
                return View(horario);
            }

            try
            {
                _context.Update(horario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Horario de trabajo editado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.HorariosTrabajo.Any(e => e.IdHorario == id))
                {
                    TempData["ErrorMessage"] = "Horario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al intentar editar el horario: {ex.Message}");
                CargarUsuarios();
                return View(horario);
            }
        }

        // Acción para mostrar el formulario de eliminación de un horario (GET)
        public async Task<IActionResult> Eliminar(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID de horario no proporcionado.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var horario = await _context.HorariosTrabajo
                    .Include(h => h.Usuario)
                    .FirstOrDefaultAsync(h => h.IdHorario == id);

                if (horario == null)
                {
                    TempData["ErrorMessage"] = "Horario no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                return View(horario);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al cargar el horario: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }

        // Acción para confirmar la eliminación de un horario (POST)
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var horario = await _context.HorariosTrabajo.FindAsync(id);
                if (horario == null)
                {
                    TempData["ErrorMessage"] = "No se encontró el horario que intentas eliminar.";
                    return RedirectToAction(nameof(Index));
                }

                _context.HorariosTrabajo.Remove(horario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Horario de trabajo eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocurrió un error al intentar eliminar el horario: {ex.Message}");
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
