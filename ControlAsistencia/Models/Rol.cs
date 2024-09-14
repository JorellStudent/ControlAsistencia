namespace ControlAsistencia.Models
{
    public class Rol
    {
        public int IdRol { get; set; }  // Clave primaria

        public required string NombreRol { get; set; }  // Obligatorio

        // Relación con Credenciales
        public ICollection<Credencial> Credencial { get; set; } = new List<Credencial>();
    }
}
