using Microsoft.AspNetCore.Mvc;
using TurnosPeluqueria.Data;
using TurnosPeluqueria.Models;

public class HorarioController : Controller
{
    private readonly TurnosContext _context;

    public HorarioController(TurnosContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        var peluqueroId = HttpContext.Session.GetInt32("PeluqueroId");
        if (peluqueroId == null)
            return RedirectToAction("Login", "Auth");

        var horarios = _context.HorariosPeluqueros
            .Where(h => h.PeluqueroId == peluqueroId)
            .OrderBy(h => h.Dia)
            .ToList();

        return View(horarios);
    }

    [HttpGet]
    public IActionResult Crear()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Crear(HorarioPeluquero horario)
    {
        var peluqueroId = HttpContext.Session.GetInt32("PeluqueroId");
        if (peluqueroId == null)
            return RedirectToAction("Login", "Auth");

        horario.PeluqueroId = peluqueroId.Value;
        _context.HorariosPeluqueros.Add(horario);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Eliminar(int id)
    {
        var horario = _context.HorariosPeluqueros.Find(id);
        if (horario != null)
        {
            _context.HorariosPeluqueros.Remove(horario);
            _context.SaveChanges();
        }

        return RedirectToAction("Index");
    }
}
