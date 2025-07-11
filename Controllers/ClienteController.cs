using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult MisTurnos()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                return RedirectToAction("Login");
            }

            var turnos = _context.Turnos
                .Include(t => t.Peluquero)
                .Include(t => t.Servicio)
                .Where(t => t.ClienteId == clienteId && t.Estado != EstadoTurno.Cancelado)
                .OrderBy(t => t.FechaHora)
                .ToList();

            return View(turnos);
        }


        [HttpPost]
        public IActionResult CancelarTurno(int id)
        {
            var turno = _context.Turnos.Find(id);
            if (turno == null)
            {
                TempData["Error"] = "Turno no encontrado.";
                return RedirectToAction("MisTurnos");
            }

            turno.Estado = EstadoTurno.Cancelado;
            _context.SaveChanges();

            TempData["Exito"] = "El turno fue cancelado correctamente.";
            return RedirectToAction("MisTurnos");
        }


        public IActionResult Reservar(int id)
        {
            var turnosDisponibles = _context.Turnos
        .Include(t => t.Servicio)
        .Where(t => t.PeluqueroId == id && t.Estado == EstadoTurno.Pendiente && t.ClienteId == null)
        .OrderBy(t => t.FechaHora)
        .ToList();

            var peluquero = _context.Peluqueros.FirstOrDefault(p => p.Id == id);
            ViewBag.PeluqueroNombre = peluquero?.Nombre ?? "Peluquero";

            return View(turnosDisponibles);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string nombre, string telefono)
        {
            // Acá luego validaremos si el cliente existe o lo creamos.
            // Por ahora, redirigimos a la selección de peluquero.
            return RedirectToAction("SeleccionarPeluquero");
        }

        [HttpGet]
        public IActionResult SeleccionarPeluquero(DateTime? fecha)
        {
            var diaSeleccionado = fecha?.Date ?? DateTime.Today;

            var peluqueros = _context.Peluqueros.ToList();

            foreach (var p in peluqueros)
            {
                var horarios = _context.Turnos
    .Where(t => t.PeluqueroId == p.Id &&
                t.Estado == EstadoTurno.Pendiente &&
                t.ClienteId == null &&
                t.FechaHora.Date == diaSeleccionado &&
                t.FechaHora > DateTime.Now)
    .OrderBy(t => t.FechaHora)
    .ToList();

                p.HorariosDisponibles = horarios;
            }
            ViewBag.FechaSeleccionada = diaSeleccionado.ToString("yyyy-MM-dd");
            return View(peluqueros);
        }





        [HttpGet]
        public IActionResult ConfirmarTurno(int turnoId)
        {
            var turno = _context.Turnos
                .Include(t => t.Peluquero)
                .FirstOrDefault(t => t.Id == turnoId);

            if (turno == null)
            {
                return NotFound();
            }

            
            ViewBag.Servicios = _context.Servicios.ToList();

            return View(turno);
        }

        [HttpPost]
        public IActionResult ConfirmarTurno(int turnoId, int servicioId, string comentario)
        {
            var turno = _context.Turnos.FirstOrDefault(t => t.Id == turnoId);
            if (turno == null)
                return NotFound();

            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
                return RedirectToAction("Login");

            turno.ClienteId = clienteId;
            turno.ServicioId = servicioId;
            turno.Estado = EstadoTurno.Confirmado;

            _context.SaveChanges();

            return RedirectToAction("MisTurnos");
        }


        [HttpPost]
        public IActionResult GuardarTurno(string peluquero, string hora, int servicioId)
        {
            var peluqueroEntity = _context.Peluqueros.FirstOrDefault(p => p.Nombre == peluquero);


            if (peluqueroEntity == null)
                return NotFound("Peluquero no encontrado");

            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
                return RedirectToAction("Login");

            var cliente = _context.Clientes.Find(clienteId);
            if (cliente == null)
                return NotFound("Cliente no encontrado");

            if (!DateTime.TryParse(hora, out var horaParsed))
                return BadRequest("Hora inválida");

            DateTime fechaHoraTurno = DateTime.Today.Add(horaParsed.TimeOfDay);

            // VALIDACIÓN 1: Ya tiene un turno en ese horario
            bool clienteYaTieneTurno = _context.Turnos.Any(t =>
                t.ClienteId == cliente.Id && t.FechaHora == fechaHoraTurno && t.Estado != EstadoTurno.Cancelado);

            if (clienteYaTieneTurno)
            {
                TempData["Error"] = "Ya tenés un turno reservado en ese horario.";
                return RedirectToAction("MisTurnos");
            }

            // VALIDACIÓN 2: Ese turno ya lo tiene reservado alguien más con ese peluquero
            bool turnoOcupado = _context.Turnos.Any(t =>
                t.PeluqueroId == peluqueroEntity.Id && t.FechaHora == fechaHoraTurno && t.Estado != EstadoTurno.Cancelado);

            if (turnoOcupado)
            {
                TempData["Error"] = "Ese turno ya fue reservado por otra persona.";
                return RedirectToAction("SeleccionarPeluquero");
            }

            // Si todo OK, se crea el turno
            var nuevoTurno = new Turno
            {
                ClienteId = cliente.Id,
                PeluqueroId = peluqueroEntity.Id,
                ServicioId = servicioId,
                FechaHora = fechaHoraTurno,
                Estado = EstadoTurno.Confirmado
            };

            _context.Turnos.Add(nuevoTurno);
            _context.SaveChanges();

            TempData["Exito"] = "¡Turno reservado con éxito!";
            return RedirectToAction("MisTurnos");
        }




    }
}
