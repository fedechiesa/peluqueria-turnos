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
        public List<Turno> ObtenerTurnosPorCliente(int clienteId)
        {
            return _context.Turnos
                .Where(t => t.ClienteId == clienteId)
                .OrderBy(t => t.FechaHora)
                .ToList();
        }
    }
}
