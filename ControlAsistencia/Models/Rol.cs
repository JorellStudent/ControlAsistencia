using System.ComponentModel.DataAnnotations;

namespace ControlAsistencia.Models
{
    public class Rol
    {
        public int IdRol { get; set; }  // Clave primaria

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre del rol debe tener un máximo de 100 caracteres")]
        public string NombreRol { get; set; } = string.Empty;  // Inicializar para evitar null

        // Relación con Credenciales
        public ICollection<Credencial> Credencial { get; set; } = new List<Credencial>();  // Inicializado para evitar null
    }
}
