using System;
using System.ComponentModel.DataAnnotations;

namespace ControlAsistencia.Models
{
    public class HorarioTrabajo
    {
        // Clave primaria
        public int IdHorario { get; set; }

        // Clave foránea a Usuario
        [Required(ErrorMessage = "El usuario es obligatorio")]
        public int IdUsuario { get; set; }

        // Obligatorio, debe ser un valor de tiempo válido
        [Required(ErrorMessage = "La hora de entrada es obligatoria")]
        [DataType(DataType.Time, ErrorMessage = "La hora de entrada debe ser un valor de tiempo válido")]
        public TimeSpan HoraEntrada { get; set; }

        // Obligatorio, debe ser un valor de tiempo válido
        [Required(ErrorMessage = "La hora de salida es obligatoria")]
        [DataType(DataType.Time, ErrorMessage = "La hora de salida debe ser un valor de tiempo válido")]
        public TimeSpan HoraSalida { get; set; }

        // Obligatorio, inicializado para evitar null
        [Required(ErrorMessage = "El día de la semana es obligatorio")]
        [StringLength(15, ErrorMessage = "El día de la semana no puede tener más de 15 caracteres")]
        public string DiaSemana { get; set; } = string.Empty;

        // Relación con Usuario, nullable para evitar errores cuando no esté asignado
        public virtual Usuario? Usuario { get; set; }
    }
}
