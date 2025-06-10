using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using system_cosasapup.Data;
using system_cosasapup.Models;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

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
        // Validación de sesión para el Index
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

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

    // para filtar y paginacion
    public async Task<IActionResult> ListaPegues(string estado, string comunidad, int page = 1)
    {
        // Validación de sesión para ListaPegues
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

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

    // GET: Crear Pegue
    [HttpGet]
    public IActionResult CrearPegue()
    {
        // Validación de sesión: Si no hay usuario logeado, redirige a la página de login
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        ViewBag.Pagos = new SelectList(_context.pagos, "PagoId", "monto");
        return View();
    }

    // POST: Crear Pegue
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPegue(pegues nuevoPegue)
    {
        // Validación de sesión: Si no hay usuario logeado, redirige a la página de login
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        _context.Add(nuevoPegue);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Pegue creado exitosamente";
        return RedirectToAction("ListaPegues");
    }

    // GET: Crear Pago
    [HttpGet]
    public IActionResult CrearPago(int pegueId)
    {
        // Validación de sesión
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var pegue = _context.pegues.Find(pegueId);
        if (pegue == null)
        {
            TempData["Error"] = "Pegue no encontrado";
            return RedirectToAction("ListaPegues");
        }

        var model = new pagos
        {
            PegueId = pegueId,
            fechaPago = DateTime.Today,
            Pegue = pegue
        };

        return View(model);
    }

    // POST: Crear Pago
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPago(pagos pago)
    {
        // Validación de sesión
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        _context.pagos.Add(pago);
        await _context.SaveChangesAsync();
        TempData["Success"] = "Pago registrado exitosamente";
        return RedirectToAction("ListaPegues");
    }

    // Ver lista de pagos
    public async Task<IActionResult> ListaPagos(int page = 1, int pageSize = 10)
    {
        // Validación de sesión
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var totalRecords = await _context.pagos.CountAsync();
        var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

        var pagos = await _context.pagos
            .Include(p => p.Pegue)
            .OrderByDescending(p => p.fechaPago)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.PageSize = pageSize;

        return View(pagos);
    }

    // GET: Editar Pegue
    [HttpGet]
    public async Task<IActionResult> EditarPegue(int id)
    {
        // Validación de sesión: Si no hay usuario logeado, redirige a la página de login
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

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
        // Validación de sesión: Si no hay usuario logeado, redirige a la página de login
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

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

    // GET: ConfirmarEliminarPegue
    [HttpGet]
    public async Task<IActionResult> ConfirmarEliminar(int id)
    {
        // Validación de sesión
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var pegue = await _context.pegues
            .Include(p => p.pagos) // Incluimos los pagos por si quieres mostrarlos en la confirmación
            .FirstOrDefaultAsync(p => p.PegueId == id);

        if (pegue == null)
        {
            TempData["Error"] = "Pegue no encontrado para eliminar.";
            return RedirectToAction("ListaPegues");
        }

        return View(pegue);
    }

    // POST: Eliminar Pegue (ahora se llamará desde la vista de confirmación)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarPegue(int id) // Recibimos el ID directamente del formulario de confirmación
    {
        // Validación de sesión
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var pegue = await _context.pegues
            .Include(p => p.pagos)
            .FirstOrDefaultAsync(p => p.PegueId == id);

        if (pegue == null)
        {
            TempData["Error"] = "Pegue no encontrado."; // Ya no existe, quizás fue eliminado por otro lado
            return RedirectToAction("ListaPegues");
        }

        // Eliminar pagos asociados primero si hay una relación restrictiva (cascade delete)
        if (pegue.pagos != null && pegue.pagos.Any())
        {
            _context.pagos.RemoveRange(pegue.pagos);
        }

        _context.pegues.Remove(pegue);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Pegue eliminado exitosamente.";
        return RedirectToAction("ListaPegues");
    }

    // Reportes de pegues
    public async Task<IActionResult> ReportePegues()
    {
        // Validación de sesión
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var pegues = await _context.pegues
            .Include(p => p.pagos)
            .ToListAsync();

        return new ViewAsPdf("ReportesPegues", pegues)
        {
            FileName = "ReportePegues.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        };
    }

    // Reportes de pagos
    public async Task<IActionResult> ReportePagos()
    {
        // Validación de sesión
        if (HttpContext.Session.GetInt32("UsuarioId") == null)
        {
            return RedirectToAction("Login", "Auth");
        }

        var pagos = await _context.pagos
            .Include(p => p.Pegue)
            .OrderByDescending(p => p.fechaPago)
            .ToListAsync();

        return new ViewAsPdf("ReportesPagos", pagos)
        {
            FileName = "ReportePagos.pdf",
            PageSize = Rotativa.AspNetCore.Options.Size.A4,
            PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        };
    }
}
