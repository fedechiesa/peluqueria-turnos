using Microsoft.EntityFrameworkCore;
using TurnosPeluqueria.Models;

namespace TurnosPeluqueria.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Turno> Turnos { get; set; }
        public DbSet<Peluquero> Peluqueros { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

    }
}