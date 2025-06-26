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


        public IActionResult Reservar()
        {
            // Podés cargar peluqueros, servicios, fechas disponibles, etc.
            return View();
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
        public IActionResult SeleccionarPeluquero()
        {
            // Leemos los peluqueros desde la base
            var peluqueros = _context.Peluqueros.ToList();

            // Asignamos horarios disponibles SOLO en tiempo de ejecución (no se guardan en base)
            foreach (var p in peluqueros)
            {
                switch (p.Nombre)
                {
                    case "Juan":
                        p.HorariosDisponibles = new List<string> { "10:00", "11:30", "14:00" };
                        break;
                    case "Marta":
                        p.HorariosDisponibles = new List<string> { "09:30", "12:00", "15:30" };
                        break;
                    case "Lucas":
                        p.HorariosDisponibles = new List<string> { "13:00", "16:00", "18:30" };
                        break;
                }
            }

            return View(peluqueros);
        }



        [HttpGet]
        public IActionResult ConfirmarTurno(string peluquero, string hora)
        {
            var servicios = _context.Servicios.ToList();

            ViewBag.Peluquero = peluquero;
            ViewBag.Hora = hora;
            ViewBag.Servicios = new SelectList(servicios, "Id", "Nombre");

            return View();
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
                t.ClienteId == cliente.Id && t.FechaHora == fechaHoraTurno);

            if (clienteYaTieneTurno)
            {
                TempData["Error"] = "Ya tenés un turno reservado en ese horario.";
                return RedirectToAction("MisTurnos");
            }

            // VALIDACIÓN 2: Ese turno ya lo tiene reservado alguien más con ese peluquero
            bool turnoOcupado = _context.Turnos.Any(t =>
                t.PeluqueroId == peluqueroEntity.Id && t.FechaHora == fechaHoraTurno);

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
