using TurnosPeluqueria.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TurnosPeluqueria.Data;

namespace TurnosPeluqueria.Services

{
    public class TurnoService
    {
        private readonly TurnosContext _context;


        public TurnoService(TurnosContext context)
        {
            _context = context;
        }


        // Crear un nuevo turno si no hay conflicto
        public bool CrearTurno(int clienteId, int servicioId, DateTime fechaHora)
        {
            bool ocupado = _context.Turnos.Any(t =>
                t.ClienteId == clienteId && t.FechaHora == fechaHora);

            if (ocupado)
                return false;

            var nuevoTurno = new Turno
            {
                ClienteId = clienteId,
                ServicioId = servicioId,
                FechaHora = fechaHora
            };

            _context.Turnos.Add(nuevoTurno);
            _context.SaveChanges();

            return true;
        }

        // Cancelar turno por ID
        public bool CancelarTurno(int turnoId)
        {
            var turno = _context.Turnos.Find(turnoId);
            if (turno == null)
                return false;

            _context.Turnos.Remove(turno);
            _context.SaveChanges();

            return true;
        }


        // Obtener turnos por cliente
        public void GenerarTurnosAutomaticos(int peluqueroId)
        {
            // Obtener horarios cargados del peluquero
            var horarios = _context.HorariosPeluqueros
                .Where(h => h.PeluqueroId == peluqueroId)
                .ToList();

            if (!horarios.Any())
                return; // No hay horarios cargados, salimos

            var fechaHoy = DateTime.Today;

            // Calcular el lunes de esta semana
            var lunes = fechaHoy.AddDays(-(int)fechaHoy.DayOfWeek + (fechaHoy.DayOfWeek == DayOfWeek.Sunday ? -6 : 1));

            // Generar turnos para lunes a sábado
            for (int i = 0; i < 6; i++)
            {
                var fecha = lunes.AddDays(i);

                foreach (var horario in horarios)
                {
                    if (fecha.DayOfWeek != horario.Dia)
                        continue;

                    for (var hora = horario.Desde; hora < horario.Hasta; hora += TimeSpan.FromMinutes(30))
                    {
                        var fechaHora = fecha.Date + hora;

                        bool yaExiste = _context.Turnos.Any(t =>
                            t.PeluqueroId == peluqueroId &&
                            t.FechaHora == fechaHora &&
                            t.Estado != EstadoTurno.Cancelado); // solo bloquea si NO está cancelado

                        if (!yaExiste)
                        {
                            var turno = new Turno
                            {
                                FechaHora = fechaHora,
                                PeluqueroId = peluqueroId,
                                ServicioId = 1, // 👈 Asegurate de que este ID exista
                                Estado = EstadoTurno.Pendiente
                            };

                            _context.Turnos.Add(turno);
                        }
                    }
                }
            }

            _context.SaveChanges();
        }




    }
}
