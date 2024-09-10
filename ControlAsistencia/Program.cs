using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ControlAsistencia.Data; // Aseg�rate de importar el namespace correcto para ApplicationDbContext

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n de la base de datos con MySQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

// Configuraci�n de Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Agregar servicios MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar el pipeline de manejo de solicitudes
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
