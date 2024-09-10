using ControlAsistencia.Data;
using ControlAsistencia.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControlAsistencia.Controllers
{
    public class AsistenciaController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        [HttpPost]
        public async Task<IActionResult> MarcarEntrada(int idUsuario)
        {
            var usuario = await _context.Usuarios.FindAsync(idUsuario); // Busca el usuario en la base de datos

            if (usuario == null)
            {
                return BadRequest("Usuario no encontrado.");
            }

            var asistencia = new Asistencia
            {
                IdUsuario = idUsuario,
                Usuario = usuario,  // Asignar el usuario encontrado a la propiedad Usuario
                Fecha = DateTime.Now,
                HoraEntrada = DateTime.Now.TimeOfDay
            };

            _context.Asistencias.Add(asistencia);
            await _context.SaveChangesAsync();

            return Ok("Entrada registrada correctamente.");
        }


        // Marcar salida
        [HttpPost]
        public async Task<IActionResult> MarcarSalida(int idUsuario)
        {
            var asistencia = await _context.Asistencias
                .FirstOrDefaultAsync(a => a.IdUsuario == idUsuario && a.Fecha.Date == DateTime.Now.Date);

            if (asistencia != null)
            {
                asistencia.HoraSalida = DateTime.Now.TimeOfDay;
                await _context.SaveChangesAsync();
                return Ok("Salida registrada correctamente.");
            }

            return BadRequest("No se encontró la entrada para el día de hoy.");
        }

        // Lista de asistencias
        public async Task<IActionResult> Index()
        {
            return View(await _context.Asistencias.Include(a => a.Usuario).ToListAsync());
        }
    }
}
