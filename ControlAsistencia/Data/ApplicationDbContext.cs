using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Models;

namespace ControlAsistencia.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {

        // Definición de DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public new DbSet<Rol> Roles { get; set; }  // Usamos new para ocultar la propiedad heredada
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<HorarioTrabajo> HorariosTrabajo { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Credencial> Credencial { get; set; }  // Cambiado a plural por consistencia

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definir claves primarias
            modelBuilder.Entity<Asistencia>().HasKey(a => a.IdAsistencia);  // Clave primaria para Asistencia
            modelBuilder.Entity<Auditoria>().HasKey(a => a.IdAuditoria);  // Clave primaria para Auditoria
            modelBuilder.Entity<Credencial>().HasKey(c => c.IdCredencial);  // Clave primaria para Credencial
            modelBuilder.Entity<HorarioTrabajo>().HasKey(ht => ht.IdHorario);  // Clave primaria para HorarioTrabajo
            modelBuilder.Entity<Notificacion>().HasKey(n => n.IdNotificacion);  // Clave primaria para Notificacion
            modelBuilder.Entity<Permiso>().HasKey(p => p.IdPermiso);  // Clave primaria para Permiso
            modelBuilder.Entity<Rol>().HasKey(r => r.IdRol);  // Clave primaria para Rol
            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);  // Clave primaria para Usuario

            // Definir relaciones
            modelBuilder.Entity<Credencial>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Credencial)
                .HasForeignKey(c => c.IdUsuario);  // Relación entre Credencial y Usuario

            modelBuilder.Entity<Credencial>()
                .HasOne(c => c.Rol)
                .WithMany(r => r.Credencial)
                .HasForeignKey(c => c.IdRol);  // Relación entre Credencial y Rol

            base.OnModelCreating(modelBuilder);
        }
    }
}
