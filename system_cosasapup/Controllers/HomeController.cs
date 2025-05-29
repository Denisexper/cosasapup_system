using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using system_cosasapup.Data;
using system_cosasapup.Models;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly AplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, AplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    // Mostrar los pegues y sus pagos
    public async Task<IActionResult> Index()
    {
        var listaPegues = await _context.pegues
            .Include(p => p.pagos) // Incluye los pagos relacionados
            .ToListAsync();

        return View(listaPegues);
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

    // Método para mostrar la lista de pegues
    public async Task<IActionResult> ListaPegues()
    {
        var listaPegues = await _context.pegues
            .Include(p => p.pagos) // Trae los pagos asociados
            .ToListAsync();

        return View("ListaPegues", listaPegues);
    }

    // Método para registrar un nuevo pegue
    // GET: Home/CreatePegue
    [HttpGet]
    public IActionResult CrearPegue()
    {
        // Pasando una lista de pagos que se utilizará en el formulario
        ViewData["Pagos"] = new SelectList(_context.pagos, "id", "monto");
        return View();
    }


    // POST: Home/CreatePegue
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPegue(pegues nuevoPegue)
    {
        if (ModelState.IsValid)
        {
            _context.Add(nuevoPegue);  // Agregar el nuevo pegue
            await _context.SaveChangesAsync();
            return RedirectToAction("ListaPegues"); // Redirigir a la lista de pegues
        }

        // Si el modelo no es válido, devolvemos la lista de pagos
        ViewData["Pagos"] = new SelectList(_context.pagos.ToList(), "id", "monto", nuevoPegue.PegueId);
        return View(nuevoPegue); // Retornar la vista para corregir cualquier error
    }

    // Método para registrar un pago (esto también lo puedes agregar como opción)
    // GET: Home/CreatePago
    [HttpGet]
    public IActionResult CrearPago()
    {
        // Obtener los pegues desde la base de datos
        var pegues = _context.pegues.ToList();  // Esto obtiene todos los pegues desde la base de datos

        // Pasar los pegues a la vista a través de ViewData
        ViewData["Pegues"] = pegues;

        return View();
    }

    // POST: Home/CreatePago
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPago(pagos nuevoPago)
    {
        ViewData["Pegues"] = _context.pegues.ToList();

        if (ModelState.IsValid)
        {
            _context.Add(nuevoPago);
            await _context.SaveChangesAsync();
            return RedirectToAction("ListaPegues");  // O redirigir a donde quieras
        }
        return View(nuevoPago);  // Retornar la vista si hay un error
    }
}
