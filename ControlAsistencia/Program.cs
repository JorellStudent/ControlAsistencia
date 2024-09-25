using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de la base de datos con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Configuraci�n de Identity con opciones personalizadas
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;

    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuraci�n de cookies y acceso
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Ruta de la p�gina de login
    options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta en caso de acceso denegado
});

// Agregar servicios de MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar Rotativa con la ruta correcta de los ejecutables wkhtmltopdf
// Verifica que el camino apunte a la carpeta correcta
RotativaConfiguration.Setup(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"));


// Configuraci�n del pipeline de manejo de solicitudes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores y rutas
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
