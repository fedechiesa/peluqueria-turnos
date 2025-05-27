namespace TurnosPeluqueria.Models
{
    public class Peluquero
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Turno> Turnos { get; set; }
    }
}
