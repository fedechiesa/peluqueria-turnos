using System;
using System.Collections.Generic;

namespace TurnosPeluqueria.Models
{
    public class HorarioDia
    {
        public DayOfWeek Dia { get; set; }
        public bool Trabaja { get; set; }
        public TimeSpan Desde { get; set; }
        public TimeSpan Hasta { get; set; }
    }

    public class HorarioPersonalizadoViewModel
    {
        public List<HorarioDia> Dias { get; set; } = new List<HorarioDia>();
    }
}
