using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data;

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de la base de datos con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Configuraci�n de Identity con opciones personalizadas
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configuraciones de contrase�as
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;

    // Configuraciones de bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Configuraci�n de usuario
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configuraci�n de cookies y acceso
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login"; // Ruta a la p�gina de login
    options.AccessDeniedPath = "/Account/AccessDenied"; // Ruta cuando se deniega el acceso
});

// Agregar servicios de MVC y Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Configurar el pipeline de manejo de solicitudes
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Configuraci�n adicional
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores y rutas para Razor Pages
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Si est�s usando Razor Pages en Identity

app.Run();
