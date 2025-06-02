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

    //para filtar y paginacion
    public async Task<IActionResult> ListaPegues(string estado, string comunidad, int page = 1)
    {
        int pageSize = 10;

        var peguesQuery = _context.pegues
            .Include(p => p.pagos)
            .AsQueryable();

        if (!string.IsNullOrEmpty(estado))
        {
            bool estadoBool = estado == "true";
            peguesQuery = peguesQuery.Where(p => p.estado == estadoBool);
        }

        if (!string.IsNullOrEmpty(comunidad))
        {
            peguesQuery = peguesQuery.Where(p => p.comunidad == comunidad);
        }

        int totalRecords = await peguesQuery.CountAsync();

        var pegues = await peguesQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewData["CurrentPage"] = page;
        ViewData["PageSize"] = pageSize;
        ViewData["TotalRecords"] = totalRecords;
        ViewData["EstadoSeleccionado"] = estado;
        ViewData["ComunidadFiltro"] = comunidad;

        return View(pegues);
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
        // Ignoras validaci�n (�peligroso!)
        _context.Add(nuevoPegue);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Pegue creado exitosamente";
        return RedirectToAction("ListaPegues");
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
            Pegue = pegue // Asignamos el objeto pegue aqu�
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPago(pagos pago)
    {

        _context.pagos.Add(pago);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Pago registrado exitosamente";
        return RedirectToAction("ListaPegues");
    }

    //ver lista de pagos
    public async Task<IActionResult> ListaPagos()
    {
        var pagos = await _context.pagos
            .Include(p => p.Pegue)
            .ToListAsync();

        return View(pagos);
    }

    // GET: Editar Pegue
    [HttpGet]
    public async Task<IActionResult> EditarPegue(int id)
    {
        var pegue = await _context.pegues.FindAsync(id);
        if (pegue == null)
        {
            TempData["Error"] = "Pegue no encontrado";
            return RedirectToAction("ListaPegues");
        }

        return View(pegue);
    }

    // POST: Editar Pegue
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarPegue(pegues pegueEditado)
    {
        var pegueExistente = await _context.pegues.FindAsync(pegueEditado.PegueId);
        if (pegueExistente == null)
        {
            TempData["Error"] = "Pegue no encontrado";
            return RedirectToAction("ListaPegues");
        }

        pegueExistente.dueño = pegueEditado.dueño;
        pegueExistente.comunidad = pegueEditado.comunidad;
        pegueExistente.codigo = pegueEditado.codigo;
        pegueExistente.direccion = pegueEditado.direccion;
        pegueExistente.estado = pegueEditado.estado;

        await _context.SaveChangesAsync();
        TempData["Success"] = "Pegue editado exitosamente";
        return RedirectToAction("ListaPegues");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarPegue(int id)
    {
        var pegue = await _context.pegues
            .Include(p => p.pagos)
            .FirstOrDefaultAsync(p => p.PegueId == id);

        _context.pegues.Remove(pegue);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Pegue eliminado exitosamente";
        return RedirectToAction("ListaPegues");
    }





}