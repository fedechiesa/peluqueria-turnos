using Microsoft.AspNetCore.Mvc;
using TurnosPeluqueria.Data;
using TurnosPeluqueria.Models;

namespace TurnosPeluqueria.Controllers
{
    public class AuthController : Controller
    {
        
        private readonly TurnosContext _context;
        
        public AuthController(TurnosContext context)
        {
            _context = context;
        }

        // GET: /Auth/Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Auth/Registro
        [HttpPost]
        public IActionResult Registro(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                var existe = _context.Clientes.Any(c => c.Email == cliente.Email);
                if (existe)
                {
                    ModelState.AddModelError("Email", "El email ya está registrado.");
                    return View(cliente);
                }

                _context.Clientes.Add(cliente);
                _context.SaveChanges();
                TempData["Exito"] = "¡Cuenta creada con éxito! Ahora podés iniciar sesión.";
                return RedirectToAction("Login");
            }

            return View(cliente);
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string email, string contraseña)
        {
            // Primero intentamos login como Cliente
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == email && c.Password == contraseña);
            if (cliente != null)
            {
                HttpContext.Session.SetInt32("ClienteId", cliente.Id);
                HttpContext.Session.SetString("ClienteNombre", cliente.Nombre);
                TempData.Remove("Error"); 
                return RedirectToAction("MisTurnos", "Cliente");
            }

            // Si no es cliente, intentamos login como Peluquero
            var peluquero = _context.Peluqueros.FirstOrDefault(p => p.Email == email && p.Password == contraseña);
            if (peluquero != null)
            {
                HttpContext.Session.SetInt32("PeluqueroId", peluquero.Id);
                HttpContext.Session.SetString("PeluqueroNombre", peluquero.Nombre);
                TempData.Remove("Error"); 
                return RedirectToAction("MisTurnos", "Peluquero");
            }

            // Si ninguno coincide, mostramos error
            TempData["Error"] = "Email o contraseña incorrectos.";
            return RedirectToAction("Login");
        }



        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

