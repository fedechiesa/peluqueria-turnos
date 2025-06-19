using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TurnosPeluqueria.Models
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public string Apellido { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public List<Turno> Turnos { get; set; } = new();

        public List<Turno> ObtenerTurnosFuturos() =>
            Turnos?.Where(t => t.FechaHora > DateTime.Now).ToList() ?? new List<Turno>();

        public bool TieneTurnoEnFecha(DateTime fecha) =>
            Turnos?.Any(t => t.FechaHora == fecha) ?? false;
    }
}
