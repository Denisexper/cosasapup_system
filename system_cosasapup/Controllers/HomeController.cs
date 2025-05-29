using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using system_cosasapup.Data;
using system_cosasapup.Models;

namespace system_cosasapup.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, AplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Trae la lista de pegues junto con sus pagos relacionados
            var listaPegues = await _context.pegues
                .Include(p => p.pagos)
                .ToListAsync();

            return View(listaPegues); // Enviamos la lista a la vista Index.cshtml
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

        //motodo para dirigir a la lista de pegues
        public async Task<IActionResult> ListaPegues()
        {
            var listaPegues = await _context.pegues
                .Include(p => p.pagos)
                .ToListAsync();

            return View("ListaPegues", listaPegues);
        }

        //metodo para agregar registro de pegues
        // GET: Home/CreatePegue
        [HttpGet]
        public IActionResult CrearPegue()
        {
            ViewData["Pagos"] = new SelectList(_context.pagos.ToList(), "id", "descripcion"); // Asegúrate que "descripcion" sea el campo que quieres mostrar
            return View();
        }

        // POST: Home/CreatePegue
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPegue(pegues nuevoPegue)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nuevoPegue);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListaPegues");
            }

            ViewData["Pagos"] = new SelectList(_context.pagos.ToList(), "id", "monto", nuevoPegue.idPago);
            return View(nuevoPegue);
        }

    }
}
