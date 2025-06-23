using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosPeluqueria.Data;
using TurnosPeluqueria.Models;

namespace TurnosPeluqueria.Controllers
{
    
    public class ClienteController : Controller
    {
        private readonly TurnosContext _context;

        public ClienteController(TurnosContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> MisTurnos()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
                return RedirectToAction("Login", "Auth", new { area = "Auth" });

            var turnos = await _context.Turnos
                .Include(t => t.Peluquero)
                .Include(t => t.Servicio)
                .Where(t => t.ClienteId == clienteId)
                .ToListAsync();

            return View(turnos);
        }

        public IActionResult Reservar()
        {
            // Podés cargar peluqueros, servicios, fechas disponibles, etc.
            return View();
        }

      
    }
}
