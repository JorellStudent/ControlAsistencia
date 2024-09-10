namespace ControlAsistencia.Models
{
    public class Asistencia
    {
        public int IdAsistencia { get; set; }  // Clave primaria

        public required int IdUsuario { get; set; }  // Clave foránea a Usuario
        public required DateTime Fecha { get; set; }
        public required TimeSpan HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }  // Opcional

        // Relación con Usuario
        public required Usuario Usuario { get; set; }
    }
}
