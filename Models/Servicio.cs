using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TurnosPeluqueria.Models
{
    public class Servicio
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Precio { get; set; }

        public List<Turno> Turnos { get; set; } = new();
    }
}
