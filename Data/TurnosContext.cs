using Microsoft.EntityFrameworkCore;
using TurnosPeluqueria.Models;

namespace TurnosPeluqueria.Data
{
    public class TurnosContext : DbContext
    {
        public TurnosContext(DbContextOptions<TurnosContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Peluquero> Peluqueros { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Turno> Turnos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relacionar Turno con Cliente
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Cliente)
                .WithMany(c => c.Turnos)
                .HasForeignKey(t => t.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionar Turno con Peluquero
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Peluquero)
                .WithMany(p => p.Turnos)
                .HasForeignKey(t => t.PeluqueroId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionar Turno con Servicio
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Servicio)
                .WithMany(s => s.Turnos)
                .HasForeignKey(t => t.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
