﻿using System;
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
        public required string Nombre { get; set; }  // Obligatorio

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido debe tener un máximo de 50 caracteres")]
        public required string Apellido { get; set; }  // Obligatorio

        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ingresar un correo válido")]
        public required string Correo { get; set; }  // Obligatorio

        [StringLength(250, ErrorMessage = "La URL de la foto debe tener un máximo de 250 caracteres")]
        public string? Foto { get; set; }  // Opcional

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public required string Sexo { get; set; }  // Obligatorio

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date, ErrorMessage = "La fecha de nacimiento debe ser una fecha válida")]
        public DateTime FechaNacimiento { get; set; }  // Obligatorio

        [Required(ErrorMessage = "La nacionalidad es obligatoria")]
        [StringLength(100, ErrorMessage = "La nacionalidad debe tener un máximo de 100 caracteres")]
        public required string Nacionalidad { get; set; }  // Obligatorio

        [Required(ErrorMessage = "El RUT es obligatorio")]
        [StringLength(20, ErrorMessage = "El RUT debe tener un máximo de 20 caracteres")]
        public required string RUT { get; set; }  // Obligatorio

        public bool Activo { get; set; } = true;  // Valor por defecto

        // Relaciones con otras tablas
        public ICollection<Asistencia> Asistencias { get; set; } = [];
        public ICollection<Permiso> Permisos { get; set; } = [];
        public ICollection<Auditoria> Auditorias { get; set; } = [];

        // Relación con Credencial
        public ICollection<Credencial> Credencial { get; set; } = [];
    }
}
