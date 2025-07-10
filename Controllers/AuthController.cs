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
               
                cliente.Rol = "Cliente";

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
        public IActionResult Login(string email, string contraseña)
        {
            var cliente = _context.Clientes.FirstOrDefault(c => c.Email == email && c.Password == contraseña);
            if (cliente != null)
            {
                HttpContext.Session.SetInt32("ClienteId", cliente.Id);
                HttpContext.Session.SetString("ClienteNombre", cliente.Nombre);
                HttpContext.Session.SetString("Rol", cliente.Rol);

                if (cliente.Rol == "Peluquero")
                    return RedirectToAction("PanelPeluquero", "Home");
                else
                    return RedirectToAction("Index", "Home"); // cliente común


                
            }

            ViewBag.Error = "Credenciales inválidas.";
            return View();
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}

