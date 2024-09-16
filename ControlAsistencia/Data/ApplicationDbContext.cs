using ControlAsistencia.Models;
using Microsoft.EntityFrameworkCore;

namespace ControlAsistencia.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Definición de DbSets
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Asistencia> Asistencias { get; set; }
        public DbSet<HorarioTrabajo> HorariosTrabajo { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Credencial> Credencial { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Definir claves primarias
            modelBuilder.Entity<Asistencia>().HasKey(a => a.IdAsistencia);
            modelBuilder.Entity<Auditoria>().HasKey(a => a.IdAuditoria);
            modelBuilder.Entity<Credencial>().HasKey(c => c.IdCredencial);
            modelBuilder.Entity<HorarioTrabajo>().HasKey(ht => ht.IdHorario);
            modelBuilder.Entity<Notificacion>().HasKey(n => n.IdNotificacion);
            modelBuilder.Entity<Permiso>().HasKey(p => p.IdPermiso);
            modelBuilder.Entity<Rol>().HasKey(r => r.IdRol);
            modelBuilder.Entity<Usuario>().HasKey(u => u.IdUsuario);

            // Definir relaciones
            modelBuilder.Entity<Credencial>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Credencial)
                .HasForeignKey(c => c.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);  // Eliminar en cascada

            modelBuilder.Entity<Credencial>()
                .HasOne(c => c.Rol)
                .WithMany(r => r.Credencial)
                .HasForeignKey(c => c.IdRol)
                .OnDelete(DeleteBehavior.Cascade);  // Eliminar en cascada

            base.OnModelCreating(modelBuilder);
        }
    }
}
