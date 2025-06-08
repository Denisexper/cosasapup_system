using Microsoft.EntityFrameworkCore;
using system_cosasapup.Data;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// ? Solo una vez: configurar el DbContext con el nombre correcto del connection string
builder.Services.AddDbContext<AplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("defaultConnection"))); // Asegúrate que este nombre exista en appsettings.json

// ? Solo una vez: agregar MVC
builder.Services.AddControllersWithViews();

// ? Configurar sesiones
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ? Configurar Rotativa (debe ir después de app.Build())
RotativaConfiguration.Setup(app.Environment.WebRootPath, "Rotativa");

// ? Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // debe ir antes de Authorization
app.UseAuthorization();

// ? Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();
