using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using system_cosasapup.Data;
using system_cosasapup.Models;
using System.Linq;

namespace system_cosasapup.Controllers
{
    public class AuthController : Controller
    {
        private readonly AplicationDbContext _context;

        public AuthController(AplicationDbContext context)
        {
            _context = context;
        }

        // Mostrar formulario Login
        public IActionResult Login()
        {
            return View();
        }

        // POST Login
        [HttpPost]
        public IActionResult Login(usuarios model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var usuario = _context.usuarios
                .FirstOrDefault(u => u.correo == model.correo && u.contraseña == model.contraseña);

            if (usuario != null)
            {
                
                HttpContext.Session.SetInt32("UsuarioId", usuario.id);
                HttpContext.Session.SetString("CorreoUsuario", usuario.correo);
                return RedirectToAction("Index", "Home"); 
            }

            ModelState.AddModelError(string.Empty, "Credenciales inválidas");
            return View(model);
        }

     
        public IActionResult Register()
        {
            return View();
        }

     
        [HttpPost]
        public IActionResult Register(usuarios usuario)
        {
            if (!ModelState.IsValid)
            {
                return View(usuario);
            }

           
            if (_context.usuarios.Any(u => u.correo == usuario.correo))
            {
                ModelState.AddModelError("correo", "El correo ya está registrado.");
                return View(usuario);
            }

            _context.usuarios.Add(usuario);
            _context.SaveChanges();

            return RedirectToAction("Login");
        }

   
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

   
        public bool EstaLogueado()
        {
            return HttpContext.Session.GetInt32("UsuarioId") != null;
        }
    }
}
