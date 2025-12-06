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
          
        }
    }
}

