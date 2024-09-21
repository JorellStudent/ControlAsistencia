using System.ComponentModel.DataAnnotations;

namespace ControlAsistencia.Models
{
    public class Credencial
    {
        public int IdCredencial { get; set; }  // Clave primaria

        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de usuario debe tener un máximo de 100 caracteres")]
        public string NombreUsuario { get; set; } = string.Empty;  // Obligatorio

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255, ErrorMessage = "La contraseña debe tener un máximo de 255 caracteres")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; } = string.Empty;  // Obligatorio

        [Required(ErrorMessage = "El ID del rol es obligatorio")]
        public int IdRol { get; set; }  // Obligatorio (Clave foránea para Rol)

        [Required(ErrorMessage = "El ID del usuario es obligatorio")]
        public int IdUsuario { get; set; }  // Obligatorio (Clave foránea para Usuario)

        // Relación con Usuario
        public virtual Usuario? Usuario { get; set; }  // Relación con Usuario (opcional)

        // Relación con Rol
        public virtual Rol? Rol { get; set; }  // Relación con Rol (opcional)
    }
}
