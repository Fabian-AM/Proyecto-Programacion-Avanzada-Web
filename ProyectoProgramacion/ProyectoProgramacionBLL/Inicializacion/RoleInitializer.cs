using Microsoft.AspNetCore.Identity;
using ProyectoProgramacionDAL.Entidades; 
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Inicializacion
{
    public static class RoleInitializer
    {
        public const string AdminRole = "Admin";
        public const string AnalistaRole = "Analista";
        public const string GestorRole = "Gestor";
        public const string ServicioClienteRole = "Servicio al Cliente";
        public const string ClienteRole = "Cliente"; // Rol por defecto

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            // 1. CREACIÓN DE ROLES NECESARIOS
            string[] roleNames = {
                AdminRole,
                AnalistaRole,
                GestorRole,
                ServicioClienteRole,
                ClienteRole
            };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            //CREACIÓN DEL USUARIO ADMINISTRADOR INICIAL
            var adminUser = await userManager.FindByEmailAsync("admin@sgc.com");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin@sgc.com",
                    Email = "admin@sgc.com",
                    EmailConfirmed = true,
                    Nombre = "Administrador",
                    Apellido = "SGC",
                    Identificacion = "000000000"
                };

                var result = await userManager.CreateAsync(adminUser, "AdminP@ss123");

                if (result.Succeeded)
                {
                    // Asignación de Rol Admin
                    await userManager.AddToRoleAsync(adminUser, AdminRole);
                }
            }
        }
    }
}
