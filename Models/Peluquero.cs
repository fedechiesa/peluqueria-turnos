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

        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        
        [NotMapped]
        public List<Turno> HorariosDisponibles { get; set; } = new();

    }


}
