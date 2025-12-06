using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionBLL.Inicializacion;
using ProyectoProgramacionBLL.Mapeos;
using ProyectoProgramacionBLL.Servicios;
using ProyectoProgramacionDAL.Contexto;
using ProyectoProgramacionDAL.Entidades;
using ProyectoProgramacionDAL.Repositorios;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de MVC
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IClientesRepositorio, ClientesRepositorio>();
builder.Services.AddScoped<IClienteServicio, ClienteServicio>();
builder.Services.AddScoped<ISolicitudesRepositorio, SolicitudesRepositorio>();
builder.Services.AddScoped<ISolicitudesServicio, SolicitudesServicio>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MapeoClases));

// 2. Configuración de EF Core / SQLite / DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));

// 3. Configuración de ASP.NET Identity (Roles, Usuarios, Encriptación)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Requisitos de Contraseña
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>() // Conecta Identity con EF Core
.AddErrorDescriber<IdentityErrorDescriber>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 4. Servicios de BLL (Inyección de Dependencias)
builder.Services.AddScoped<IAccountService, AccountService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// Inicialización de roles (crea el usuario Admin y los roles)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

    await ProyectoProgramacionBLL.Inicializacion.RoleInitializer.SeedRolesAsync(roleManager, userManager);
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.MapControllerRoute(
    name: "Clientes",
    pattern: "{controller=Cliente}/{action=Index}/{id?}");

app.Run();
