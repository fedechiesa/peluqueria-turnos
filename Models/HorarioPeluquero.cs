using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TurnosPeluqueria.Models
{
    public class HorarioPeluquero
    {
        public int Id { get; set; }

        [Required]
        public int PeluqueroId { get; set; }

        [Required]
        public DayOfWeek Dia { get; set; }

        [Required]
        public TimeSpan Desde { get; set; }

        [Required]
        public TimeSpan Hasta { get; set; }

        [ForeignKey("PeluqueroId")]
        public Peluquero Peluquero { get; set; }
    }
}
