namespace ControlAsistencia.Models
{
    public class Auditoria
    {
        public int IdAuditoria { get; set; }  // Clave primaria

        public required int IdUsuario { get; set; }  // Obligatorio, clave foránea a Usuario
        public required string Accion { get; set; }  // Obligatorio
        public required DateTime Fecha { get; set; }  // Obligatorio
        public required TimeSpan Hora { get; set; }  // Obligatorio

        // Relación con Usuario
        public required Usuario Usuario { get; set; }
    }
}
