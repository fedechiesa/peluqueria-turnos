using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurnosPeluqueria.Data;
using Microsoft.Extensions.Logging;
using TurnosPeluqueria.Models;

namespace TurnosPeluqueria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TurnosContext _context;

        public HomeController(ILogger<HomeController> logger, TurnosContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
                return View(); // Usuario no logueado → mostrar bienvenida simple

            var cliente = _context.Clientes.Find(clienteId);

            ViewBag.ClienteNombre = cliente?.Nombre ?? "Cliente";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    

}
}
