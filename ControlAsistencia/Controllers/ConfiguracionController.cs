using Microsoft.AspNetCore.Mvc;

namespace ControlAsistencia.Controllers
{
    public class ConfiguracionController : Controller
    {
        // Página principal de configuración
        public IActionResult Index()
        {
            return View();
        }

        // Configuración de notificaciones
        public IActionResult Notificaciones()
        {
            // Aquí podrías gestionar las configuraciones específicas de notificaciones
            return View();
        }

        // Configuración de roles y permisos
        public IActionResult RolesPermisos()
        {
            // Aquí podrías gestionar las configuraciones de roles y permisos
            return View();
        }
    }
}
