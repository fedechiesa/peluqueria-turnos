namespace TurnosPeluqueria.Models
{
    public class Cliente
    {
        private int Id { get; set; }

        private string Nombre { get; set; }
        private string Apellido { get; set; }

        private string Telefono { get; set; }

        private string Email { get; set; }

        private string Password { get; set; }

        // Relación con Turnos (opcional)
        private List<Turno> Turnos { get; set; }


        public List<Turno> ObtenerTurnosFuturos()
        {
            return Turnos.Where(t => t.FechaHora > DateTime.Now).ToList();
        }

        // Verificar si ya tiene un turno en cierta fecha
        public bool TieneTurnoEnFecha(DateTime fecha)
        {
            return Turnos.Any(t => t.FechaHora == fecha);
        }
    }
}
