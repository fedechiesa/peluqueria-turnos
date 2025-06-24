using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TurnosPeluqueria.Models
{
    public class Peluquero
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Imagen { get; set; }

        // Lista de horarios disponibles para ese peluquero
        public List<string> HorariosDisponibles { get; set; } = new();

    }

}
