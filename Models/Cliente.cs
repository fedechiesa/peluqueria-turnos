namespace TurnosPeluqueria.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Apellido { get; set; }

        public string Telefono { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        // Relación con Turnos (opcional)
        public List<Turno> Turnos { get; set; }
    }
}
