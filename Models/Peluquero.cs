using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TurnosPeluqueria.Models
{
    public class Peluquero
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        public List<Turno> Turnos { get; set; } = new();
    }
}
