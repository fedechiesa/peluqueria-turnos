using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TurnosPeluqueria.Data;
using TurnosPeluqueria.Models;

namespace TurnosPeluqueria.Controllers
{
    public class PeluqueroController : Controller
    {
        private readonly TurnosContext _context;

        public PeluqueroController(TurnosContext context)
        {
            _context = context;
        }

        public IActionResult MisTurnos()
        {
            var peluqueroId = HttpContext.Session.GetInt32("PeluqueroId");
            if (peluqueroId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var turnos = _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Servicio)
                .Where(t => t.PeluqueroId == peluqueroId && t.Estado != EstadoTurno.Cancelado)
                .OrderBy(t => t.FechaHora)
                .ToList();

            return View(turnos);
        }
    }
}
