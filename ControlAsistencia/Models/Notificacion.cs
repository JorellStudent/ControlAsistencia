namespace ControlAsistencia.Models
{
    public class Notificacion
    {
        public int IdNotificacion { get; set; }  // Clave primaria

        public required int IdUsuario { get; set; }  // Clave foránea a Usuario
        public required string TipoNotificacion { get; set; }  // Obligatorio
        public required DateTime FechaEnvio { get; set; }  // Obligatorio
        public bool Estado { get; set; } = false;  // Valor por defecto (no enviada)

        // Relación con Usuario
        public required Usuario Usuario { get; set; }
    }
}
