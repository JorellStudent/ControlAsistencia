using System.ComponentModel.DataAnnotations;

namespace ControlAsistencia.Models
{
    public class Rol
    {
        public int IdRol { get; set; }  // Clave primaria

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre del rol debe tener un máximo de 100 caracteres")]
        public required string NombreRol { get; set; }  // Obligatorio

        // Relación con Credenciales
        public ICollection<Credencial> Credencial { get; set; } = new List<Credencial>();  // Relación con Credenciales
    }
}
