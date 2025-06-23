using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TurnosPeluqueria.Data;
using TurnosPeluqueria.Models;

namespace TurnosPeluqueria.Controllers
{
    public class TurnoesController : Controller
    {
        private readonly TurnosContext _context;

        public TurnoesController(TurnosContext context)
        {
            _context = context;
        }

        // GET: Turnoes
        public async Task<IActionResult> Index()
        {
            var turnosActivos = _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Peluquero)
                .Include(t => t.Servicio)
                .Where(t => t.Estado != EstadoTurno.Cancelado);

            return View(await turnosActivos.ToListAsync());
        }

        // GET: Turnoes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var turno = await _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Peluquero)
                .Include(t => t.Servicio)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (turno == null) return NotFound();

            return View(turno);
        }

        // GET: Turnoes/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido");
            ViewData["PeluqueroId"] = new SelectList(_context.Peluqueros, "Id", "Nombre");
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "Id", "Nombre");
            return View();
        }

        // POST: Turnoes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FechaHora,ClienteId,PeluqueroId,ServicioId,Estado")] Turno turno)
        {
            if (ModelState.IsValid)
            {
                _context.Add(turno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", turno.ClienteId);
            ViewData["PeluqueroId"] = new SelectList(_context.Peluqueros, "Id", "Nombre", turno.PeluqueroId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "Id", "Nombre", turno.ServicioId);
            return View(turno);
        }

        // GET: Turnoes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", turno.ClienteId);
            ViewData["PeluqueroId"] = new SelectList(_context.Peluqueros, "Id", "Nombre", turno.PeluqueroId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "Id", "Nombre", turno.ServicioId);
            return View(turno);
        }

        // POST: Turnoes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FechaHora,ClienteId,PeluqueroId,ServicioId,Estado")] Turno turno)
        {
            if (id != turno.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(turno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TurnoExists(turno.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ClienteId"] = new SelectList(_context.Clientes, "Id", "Apellido", turno.ClienteId);
            ViewData["PeluqueroId"] = new SelectList(_context.Peluqueros, "Id", "Nombre", turno.PeluqueroId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "Id", "Nombre", turno.ServicioId);
            return View(turno);
        }

        // GET: Turnoes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var turno = await _context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Peluquero)
                .Include(t => t.Servicio)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (turno == null) return NotFound();

            return View(turno);
        }

        // POST: Turnoes/Delete/5 (Ahora se cancela en lugar de eliminar)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarConfirmed(int id)
        {
            var turno = await _context.Turnos.FindAsync(id);
            if (turno == null) return NotFound();

            turno.Estado = EstadoTurno.Cancelado;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool TurnoExists(int id)
        {
            return _context.Turnos.Any(e => e.Id == id);
        }

        public IActionResult MisTurnos()
        {
            int? clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var misTurnos = _context.Turnos
                .Include(t => t.Peluquero)
                .Include(t => t.Servicio)
                .Where(t => t.ClienteId == clienteId && t.Estado != EstadoTurno.Cancelado)
                .OrderBy(t => t.FechaHora)
                .ToList();

            return View(misTurnos);
        }

        public IActionResult Reservar()
        {
            int? clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["PeluqueroId"] = new SelectList(_context.Peluqueros, "Id", "Nombre");
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Reservar([Bind("FechaHora,PeluqueroId,ServicioId")] Turno turno)
        {
            int? clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Validación: ¿ya hay un turno con este peluquero en esa fecha y hora?
            bool ocupado = _context.Turnos.Any(t =>
                t.PeluqueroId == turno.PeluqueroId &&
                t.FechaHora == turno.FechaHora &&
                t.Estado != EstadoTurno.Cancelado
            );

            if (ocupado)
            {
                ModelState.AddModelError("FechaHora", "El peluquero ya tiene un turno en ese horario.");
            }

            if (ModelState.IsValid)
            {
                turno.ClienteId = clienteId.Value;
                turno.Estado = EstadoTurno.Pendiente;

                _context.Turnos.Add(turno);
                _context.SaveChanges();

                return RedirectToAction("MisTurnos");
            }

            ViewData["PeluqueroId"] = new SelectList(_context.Peluqueros, "Id", "Nombre", turno.PeluqueroId);
            ViewData["ServicioId"] = new SelectList(_context.Servicios, "Id", "Nombre", turno.ServicioId);
            return View(turno);
        }



    }
}

