using System.ComponentModel.DataAnnotations;

namespace ControlAsistencia.Models
{
    public class Credencial
    {
        public int IdCredencial { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public required string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public required string Contrasena { get; set; }

        [Required(ErrorMessage = "El usuario es obligatorio.")]
        public int IdUsuario { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public int IdRol { get; set; }

        // Relación con otras tablas (opcional)
        public required Usuario Usuario { get; set; }
        public required Rol Rol { get; set; }
    }
}