using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurnosPeluqueria.Models
{
    public class Peluquero
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }

        public string? Imagen { get; set; }


        [NotMapped]
        // Lista de horarios disponibles para ese peluquero
        public List<string> HorariosDisponibles { get; set; } = new();

    }

}
