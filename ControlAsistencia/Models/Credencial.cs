namespace ControlAsistencia.Models
{
    public class Credencial
    {
        public int IdCredencial { get; set; }  // Clave primaria

        public required int IdUsuario { get; set; }  // Clave foránea a Usuario
        public required string NombreUsuario { get; set; }  // Obligatorio
        public required string Contrasena { get; set; }  // Obligatorio
        public required int IdRol { get; set; }  // Clave foránea a Rol

        // Relaciones con Usuario y Rol
        public required Usuario Usuario { get; set; }
        public required Rol Rol { get; set; }
    }
}
