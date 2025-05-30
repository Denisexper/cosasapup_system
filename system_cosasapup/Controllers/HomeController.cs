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

    public async Task<IActionResult> Index()
    {
        var listaPegues = await _context.pegues
            .Include(p => p.pagos)
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

    public async Task<IActionResult> ListaPegues()
    {
        var listaPegues = await _context.pegues
            .Include(p => p.pagos)
            .ToListAsync();
        return View(listaPegues);
    }

    [HttpGet]
    public IActionResult CrearPegue()
    {
        ViewBag.Pagos = new SelectList(_context.pagos, "PagoId", "monto");
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPegue(pegues nuevoPegue)
    {
        if (ModelState.IsValid)
        {
            _context.Add(nuevoPegue);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Pegue creado exitosamente";
            return RedirectToAction("ListaPegues");
        }

        ViewBag.Pagos = new SelectList(_context.pagos, "PagoId", "monto", nuevoPegue.PegueId);
        return View(nuevoPegue);
    }

    [HttpGet]
    public IActionResult CrearPago(int pegueId)
    {
        var pegue = _context.pegues.Find(pegueId); // Eliminamos Include innecesario
        if (pegue == null)
        {
            TempData["Error"] = "Pegue no encontrado";
            return RedirectToAction("ListaPegues");
        }

        // Pasamos el pegue directamente al modelo
        var model = new pagos
        {
            PegueId = pegueId,
            fechaPago = DateTime.Today,
            Pegue = pegue // Asignamos el objeto pegue aquí
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPago(pagos pago)
    {
        if (!ModelState.IsValid)
        {
            // Recargar datos del pegue si hay error
            pago.Pegue = await _context.pegues.FindAsync(pago.PegueId);
            return View(pago);
        }

        _context.pagos.Add(pago);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Pago registrado exitosamente";
        return RedirectToAction("ListaPegues");
    }
}