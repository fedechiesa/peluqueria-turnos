using System;
using System.ComponentModel.DataAnnotations;

namespace TurnosPeluqueria.Models
{
    public class Turno
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Debe ingresar la fecha y hora del turno.")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Fecha y Hora")]
        public DateTime FechaHora { get; set; }

       
        [Display(Name = "Cliente")]
        public int? ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [Required]
        [Display(Name = "Peluquero")]
        public int PeluqueroId { get; set; }
        public Peluquero Peluquero { get; set; }

        [Required]
        [Display(Name = "Servicio")]
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        [Display(Name = "Estado")]
        public EstadoTurno Estado { get; set; } = EstadoTurno.Pendiente;
    }

    public enum EstadoTurno
    {
        Pendiente,
        Confirmado,
        Cancelado
    }
}
