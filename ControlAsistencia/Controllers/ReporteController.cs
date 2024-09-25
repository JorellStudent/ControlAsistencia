using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data;
using ControlAsistencia.Models;
using OfficeOpenXml;
using Rotativa.AspNetCore;

namespace ControlAsistencia.Controllers
{
    public class ReporteController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReporteController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Acción para mostrar el reporte general de asistencias con filtros
        public async Task<IActionResult> ReporteAsistenciaGeneral(string searchNombre, string fechaInicio, string fechaFin)
        {
            // Consulta inicial de todas las asistencias con el usuario relacionado
            var asistencias = _context.Asistencias.Include(a => a.Usuario).AsQueryable();

            // Filtro de búsqueda por nombre (nombre o apellido)
            if (!string.IsNullOrEmpty(searchNombre))
            {
                searchNombre = searchNombre.ToLower();
                asistencias = asistencias.Where(a => a.Usuario.Nombre.ToLower().Contains(searchNombre)
                                                   || a.Usuario.Apellido.ToLower().Contains(searchNombre));
            }

            // Filtro por fecha de inicio
            if (!string.IsNullOrEmpty(fechaInicio))
            {
                if (DateTime.TryParse(fechaInicio, out DateTime fechaInicioParsed))
                {
                    asistencias = asistencias.Where(a => a.Fecha >= fechaInicioParsed);
                }
                else
                {
                    ModelState.AddModelError("fechaInicio", "Fecha de inicio inválida.");
                }
            }

            // Filtro por fecha de fin
            if (!string.IsNullOrEmpty(fechaFin))
            {
                if (DateTime.TryParse(fechaFin, out DateTime fechaFinParsed))
                {
                    asistencias = asistencias.Where(a => a.Fecha <= fechaFinParsed);
                }
                else
                {
                    ModelState.AddModelError("fechaFin", "Fecha de fin inválida.");
                }
            }

            // Guardar los filtros aplicados en el ViewBag para mantenerlos en la vista
            ViewBag.searchNombre = searchNombre;
            ViewBag.fechaInicio = fechaInicio;
            ViewBag.fechaFin = fechaFin;

            // Ejecutar la consulta y devolver la lista a la vista
            var listaAsistencias = await asistencias.ToListAsync();

            return View(listaAsistencias);
        }

        // Acción para exportar a Excel
        public IActionResult ExportarExcel()
        {
            var asistencias = _context.Asistencias.Include(a => a.Usuario).ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Asistencias");

                // Escribir encabezados
                worksheet.Cells[1, 1].Value = "Nombre del Usuario";
                worksheet.Cells[1, 2].Value = "Fecha";
                worksheet.Cells[1, 3].Value = "Hora de Entrada";
                worksheet.Cells[1, 4].Value = "Hora de Salida";
                worksheet.Cells[1, 5].Value = "Estado";

                int row = 2;
                foreach (var asistencia in asistencias)
                {
                    worksheet.Cells[row, 1].Value = $"{asistencia.Usuario.Nombre} {asistencia.Usuario.Apellido}";
                    worksheet.Cells[row, 2].Value = asistencia.Fecha.ToString("dd/MM/yyyy");

                    // Corregir el formateo de TimeSpan para evitar el error CS1501
                    worksheet.Cells[row, 3].Value = asistencia.HoraEntrada != TimeSpan.Zero
                        ? asistencia.HoraEntrada.ToString() // Usar ToString() sin argumentos
                        : "No Registrada";

                    worksheet.Cells[row, 4].Value = asistencia.HoraSalida != TimeSpan.Zero
                        ? asistencia.HoraSalida.ToString() // Usar ToString() sin argumentos
                        : "No Registrada";

                    worksheet.Cells[row, 5].Value = asistencia.HoraSalida == TimeSpan.Zero ? "En Progreso" : "Completado";
                    row++;
                }

                // Convertir a bytes y retornar el archivo Excel
                var fileContents = package.GetAsByteArray();
                return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteAsistencia.xlsx");
            }
        }

        // Acción para exportar a PDF
        public IActionResult ExportarPDF()
        {
            var asistencias = _context.Asistencias.Include(a => a.Usuario).ToList();

            // Retorna la vista "ReporteAsistenciaGeneral" como PDF
            return new ViewAsPdf("ReporteAsistenciaGeneral", asistencias)
            {
                FileName = "ReporteAsistencia.pdf", // Nombre del archivo PDF
                PageSize = Rotativa.AspNetCore.Options.Size.A4, // Tamaño de la página
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait, // Orientación de la página
                PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10) // Márgenes
            };
        }
    }
}
