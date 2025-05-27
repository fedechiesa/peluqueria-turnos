namespace TurnosPeluqueria.Models
{
    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public List<Turno> Turnos { get; set; }
    }
}
