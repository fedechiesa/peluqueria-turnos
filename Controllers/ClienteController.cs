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
                .Where(t => t.ClienteId == clienteId)
                .OrderBy(t => t.FechaHora)
                .ToList();

            return View(turnos);
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
            var peluqueros = new List<Peluquero>
    {
        new Peluquero
        {
            Id = 1,
            Nombre = "Juan",
            Imagen = "juan.jpg",
            HorariosDisponibles = new List<string> { "10:00", "11:30", "14:00" }
        },
        new Peluquero
        {
            Id = 2,
            Nombre = "Marta",
            Imagen = "marta.jpg",
            HorariosDisponibles = new List<string> { "09:30", "12:00", "15:30" }
        },
        new Peluquero
        {
            Id = 3,
            Nombre = "Lucas",
            Imagen = "lucas.jpg",
            HorariosDisponibles = new List<string> { "13:00", "16:00", "18:30" }
        }
    };

            return View(peluqueros);
        }


        [HttpGet]
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

            return RedirectToAction("MisTurnos");
        }



    }
}
