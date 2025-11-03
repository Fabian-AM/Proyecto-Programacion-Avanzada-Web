using Microsoft.AspNetCore.Identity;
using ProyectoProgramacionBLL.Servicios;
using ProyectoProgramacionDAL.Entidades;
using ProyectoProgramacionDAL.Contexto;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionBLL.Inicializacion;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuración de MVC
builder.Services.AddControllersWithViews();

// 2. Configuración de EF Core / SQLite / DbContext
builder.Services.AddDbContext<AppDbContext>();

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

app.Run();
