namespace ControlAsistencia.Models
{
    public class HorarioTrabajo
    {
        public int IdHorario { get; set; }  // Clave primaria

        public required int IdUsuario { get; set; }  // Clave foránea a Usuario
        public required TimeSpan HoraEntrada { get; set; }  // Obligatorio
        public required TimeSpan HoraSalida { get; set; }  // Obligatorio
        public required string DiaSemana { get; set; }  // Obligatorio

        // Relación con Usuario
        public required Usuario Usuario { get; set; }
    }
}
