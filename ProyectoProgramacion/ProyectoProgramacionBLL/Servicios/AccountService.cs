using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ProyectoProgramacionDAL.Entidades;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionBLL.Dtos.ViewModels;
using ProyectoProgramacionBLL.Inicializacion;


namespace ProyectoProgramacionBLL.Servicios
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterViewModel model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Identificacion = model.Identificacion,
                Nombre = model.Nombre,              
                Apellido = model.Apellido         
                                                    
            };

            // Lógica de Encriptación y Creación de Usuario
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, RoleInitializer.ClienteRole);
                await _signInManager.SignInAsync(user, isPersistent: false);
                return new AuthResponseDto { Succeeded = true, Message = "Registro exitoso." };
            }

            return new AuthResponseDto { Succeeded = false, Errors = result.Errors.Select(e => e.Description).ToList() };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginViewModel model)
        {
            //Lógica de Verificación de Credenciales
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return new AuthResponseDto { Succeeded = true, Message = "Inicio de sesión exitoso." };
            }

            //Lógica de Retorno de Error
            return new AuthResponseDto { Succeeded = false, Message = "Intento de inicio de sesión no válido. Credenciales incorrectas." };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
