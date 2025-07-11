using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using TurnosPeluqueria.Data;
using TurnosPeluqueria.Models;
using TurnosPeluqueria.Services;





namespace TurnosPeluqueria.Controllers
{
    public class PeluqueroController : Controller
    {
        private readonly TurnosContext _context;
        private readonly TurnoService _turnoService;

        public PeluqueroController(TurnosContext context, TurnoService turnoService)
        {
            _context = context;
            _turnoService = turnoService;
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
        public IActionResult GenerarTurnos()
        {
            var peluqueroId = HttpContext.Session.GetInt32("PeluqueroId");
            if (peluqueroId == null)
                return RedirectToAction("Login", "Auth");

            _turnoService.GenerarTurnosAutomaticos(peluqueroId.Value);

            TempData["Exito"] = "Turnos generados exitosamente.";
            return RedirectToAction("MisTurnos");
        }

        public IActionResult Configuracion()
        {
            int? peluqueroId = HttpContext.Session.GetInt32("PeluqueroId");
            if (peluqueroId == null)
                return RedirectToAction("Login", "Auth");

            // Cargar los horarios actuales para el ViewBag (solo para mostrarlos arriba)
            var horariosActuales = _context.HorariosPeluqueros
                .Where(h => h.PeluqueroId == peluqueroId)
                .OrderBy(h => h.Dia)
                .ToList();

            ViewBag.HorariosActuales = horariosActuales;

            // También armar el modelo para el formulario
            var viewModel = new HorarioPersonalizadoViewModel();

            foreach (DayOfWeek dia in Enum.GetValues(typeof(DayOfWeek)))
            {
                if ((int)dia >= 1 && (int)dia <= 6) // Lunes a Sábado
                {
                    var horario = horariosActuales.FirstOrDefault(h => h.Dia == dia);
                    viewModel.Dias.Add(new HorarioDia
                    {
                        Dia = dia,
                        Trabaja = horario != null,
                        Desde = horario?.Desde ?? new TimeSpan(9, 0, 0),
                        Hasta = horario?.Hasta ?? new TimeSpan(17, 0, 0)
                    });
                }
            }

            return View(viewModel);
        }




        [HttpPost]
        public IActionResult ReiniciarHorarios()
        {
            int? peluqueroId = HttpContext.Session.GetInt32("PeluqueroId");
            if (peluqueroId == null)
                return RedirectToAction("Login", "Auth");

            // 1. Eliminar todos los horarios existentes
            var horarios = _context.HorariosPeluqueros
                .Where(h => h.PeluqueroId == peluqueroId.Value)
                .ToList();

            if (horarios.Any())
            {
                _context.HorariosPeluqueros.RemoveRange(horarios);
                _context.SaveChanges();
            }

            // 2. Crear los nuevos horarios por defecto (lunes a sábado de 9 a 17)
            for (int dia = 1; dia <= 6; dia++) // DayOfWeek: lunes = 1, sábado = 6
            {
                _context.HorariosPeluqueros.Add(new HorarioPeluquero
                {
                    PeluqueroId = peluqueroId.Value,
                    Dia = (DayOfWeek)dia,
                    Desde = new TimeSpan(9, 0, 0),
                    Hasta = new TimeSpan(17, 0, 0)
                });
            }

            _context.SaveChanges(); // 🔴 Importante: guardar los nuevos horarios

            TempData["Exito"] = "Horarios reiniciados correctamente.";
            return RedirectToAction("Configuracion");
        }




        [HttpPost]
        public IActionResult GuardarHorariosPersonalizados(HorarioPersonalizadoViewModel model)
        {
            int? peluqueroId = HttpContext.Session.GetInt32("PeluqueroId");
            if (peluqueroId == null)
                return RedirectToAction("Login", "Auth");

            // Eliminar horarios anteriores
            var existentes = _context.HorariosPeluqueros
                .Where(h => h.PeluqueroId == peluqueroId.Value);
            _context.HorariosPeluqueros.RemoveRange(existentes);

            // Agregar los horarios nuevos marcados como activos
            foreach (var dia in model.Dias.Where(d => d.Trabaja))
            {
                _context.HorariosPeluqueros.Add(new HorarioPeluquero
                {
                    PeluqueroId = peluqueroId.Value,
                    Dia = dia.Dia,
                    Desde = dia.Desde,
                    Hasta = dia.Hasta
                });
            }

            _context.SaveChanges();
            TempData["Exito"] = "Horarios personalizados guardados correctamente.";
            return RedirectToAction("Configuracion");
        }


    }
}
