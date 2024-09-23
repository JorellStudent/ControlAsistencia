using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ControlAsistencia.Models
{
    public class Usuario
    {
        [Key]
        public int IdUsuario { get; set; }  // Clave primaria

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre debe tener un máximo de 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;  // Inicializado para evitar null

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido debe tener un máximo de 50 caracteres")]
        public string Apellido { get; set; } = string.Empty;  // Inicializado para evitar null

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        [StringLength(255, ErrorMessage = "El correo debe tener un máximo de 255 caracteres")]
        public string Correo { get; set; } = string.Empty;  // Inicializado para evitar null

        [StringLength(250, ErrorMessage = "La URL de la foto debe tener un máximo de 250 caracteres")]
        public string? Foto { get; set; }  // Opcional (puede ser nulo)

        [Required(ErrorMessage = "El sexo es obligatorio")]
        [StringLength(10, ErrorMessage = "El sexo debe tener un máximo de 10 caracteres")]
        public string Sexo { get; set; } = string.Empty;  // Inicializado para evitar null

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de nacimiento debe ser una fecha válida")]
        public DateTime FechaNacimiento { get; set; }  // Obligatorio

        [Required(ErrorMessage = "La nacionalidad es obligatoria")]
        [StringLength(100, ErrorMessage = "La nacionalidad debe tener un máximo de 100 caracteres")]
        public string Nacionalidad { get; set; } = string.Empty;  // Inicializado para evitar null

        [Required(ErrorMessage = "El RUT es obligatorio")]
        [StringLength(20, ErrorMessage = "El RUT debe tener un máximo de 20 caracteres")]
        public string RUT { get; set; } = string.Empty;  // Inicializado para evitar null

        public bool Activo { get; set; } = true;  // Por defecto está activo

        // Relaciones con otras tablas
        public ICollection<Asistencia>? Asistencias { get; set; } = new List<Asistencia>();  // Inicializado para evitar null
        public ICollection<Permiso>? Permisos { get; set; } = new List<Permiso>();  // Inicializado para evitar null
        public ICollection<Auditoria>? Auditorias { get; set; } = new List<Auditoria>();  // Inicializado para evitar null

        // Relación con Credencial
        public ICollection<Credencial> Credencial { get; set; } = new List<Credencial>();  // Inicializado para evitar null

        // Relación con HorarioTrabajo (un Usuario puede tener muchos HorariosTrabajo)
        public ICollection<HorarioTrabajo> HorariosTrabajo { get; set; } = new List<HorarioTrabajo>();  // Inicializado para evitar null
    }
}
