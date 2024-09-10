namespace ControlAsistencia.Models
{
    public class Permiso
    {
        public int IdPermiso { get; set; }  // Clave primaria

        public required int IdUsuario { get; set; }  // Clave foránea a Usuario
        public required DateTime FechaInicio { get; set; }  // Obligatorio
        public required DateTime FechaFin { get; set; }  // Obligatorio
        public required string TipoPermiso { get; set; }  // Obligatorio

        // Relación con Usuario
        public required Usuario Usuario { get; set; }
    }
}
