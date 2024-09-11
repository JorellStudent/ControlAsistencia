using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data; // Asegúrate de importar el namespace correcto para ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Configuración de la base de datos con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Configuración de Identity con opciones personalizadas
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configuraciones de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;

    // Configuraciones de bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // Configuración de usuario
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

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

// Configuración adicional
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Agregar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapear controladores y rutas para Razor Pages (opcional si usas Razor Pages en Identity)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Si estás usando Razor Pages en Identity

app.Run();
