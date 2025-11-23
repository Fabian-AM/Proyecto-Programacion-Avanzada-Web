using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionBLL.Dtos.ViewModels;
using ProyectoProgramacionBLL.Inicializacion;
using ProyectoProgramacionDAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ProyectoProgramacionBLL.Servicios
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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

        public async Task<List<UserListDto>> GetAllUsersAsync()
        {
            var users = _userManager.Users.ToList();
            var list = new List<UserListDto>();
            foreach (var u in users)
            {
                var roles = await _userManager.GetRolesAsync(u);
                list.Add(new UserListDto
                {
                    Id = u.Id,
                    Email = u.Email,
                    Identificacion = u.Identificacion,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Roles = roles.ToList()
                });
            }
            return list;
        }

        public async Task<UserEditDto> GetUserByIdAsync(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return null;
            var roles = await _userManager.GetRolesAsync(u);
            return new UserEditDto
            {
                Id = u.Id,
                Email = u.Email,
                Identificacion = u.Identificacion,
                Nombre = u.Nombre,
                Apellido = u.Apellido,
                Roles = roles.ToList()
            };
        }

        public async Task<OperationResult> CreateUserAsync(UserCreateViewModel model)
        {
            var resultObj = new OperationResult();
            var exists = await _userManager.FindByEmailAsync(model.Email);
            if (exists != null)
            {
                resultObj.Succeeded = false;
                resultObj.Errors.Add("El correo ya está registrado.");
                return resultObj;
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Identificacion = model.Identificacion,
                Nombre = model.Nombre,
                Apellido = model.Apellido
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                resultObj.Succeeded = false;
                resultObj.Errors.AddRange(result.Errors.Select(e => e.Description));
                return resultObj;
            }

            if (model.Roles != null && model.Roles.Any())
            {
                await _userManager.AddToRolesAsync(user, model.Roles);
            }

            resultObj.Succeeded = true;
            return resultObj;
        }

        public async Task<OperationResult> UpdateUserAsync(UserEditViewModel model)
        {
            var resultObj = new OperationResult();
            var u = await _userManager.FindByIdAsync(model.Id);
            if (u == null) { resultObj.Succeeded = false; resultObj.Errors.Add("Usuario no encontrado."); return resultObj; }

            u.Email = model.Email;
            u.UserName = model.Email;
            u.Identificacion = model.Identificacion;
            u.Nombre = model.Nombre;
            u.Apellido = model.Apellido;

            var result = await _userManager.UpdateAsync(u);
            if (!result.Succeeded) { resultObj.Succeeded = false; resultObj.Errors.AddRange(result.Errors.Select(e => e.Description)); return resultObj; }

            resultObj.Succeeded = true;
            return resultObj;
        }

        public async Task<OperationResult> DeleteUserAsync(string id)
        {
            var resultObj = new OperationResult();
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) { resultObj.Succeeded = false; resultObj.Errors.Add("Usuario no encontrado."); return resultObj; }

            var result = await _userManager.DeleteAsync(u);
            resultObj.Succeeded = result.Succeeded;
            if (!result.Succeeded) resultObj.Errors.AddRange(result.Errors.Select(e => e.Description));
            return resultObj;
        }

        public async Task<List<string>> GetUserRolesAsync(string userId)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) return new List<string>();
            var roles = await _userManager.GetRolesAsync(u);
            return roles.ToList();
        }

        public async Task<OperationResult> SetUserRolesAsync(string userId, List<string> roles)
        {
            var resultObj = new OperationResult();
            var u = await _userManager.FindByIdAsync(userId);
            if (u == null) { resultObj.Succeeded = false; resultObj.Errors.Add("Usuario no encontrado."); return resultObj; }

            var currentRoles = await _userManager.GetRolesAsync(u);
            var removeResult = await _userManager.RemoveFromRolesAsync(u, currentRoles.Except(roles));
            if (!removeResult.Succeeded) { resultObj.Succeeded = false; resultObj.Errors.AddRange(removeResult.Errors.Select(e => e.Description)); return resultObj; }

            var addResult = await _userManager.AddToRolesAsync(u, roles.Except(currentRoles));
            if (!addResult.Succeeded) { resultObj.Succeeded = false; resultObj.Errors.AddRange(addResult.Errors.Select(e => e.Description)); return resultObj; }

            resultObj.Succeeded = true;
            return resultObj;
        }

        public async Task<UserEditDto> GetUserByEmailAsync(string email)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return null;

            return new UserEditDto
            {
                Id = user.Id,
                Email = user.Email,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Identificacion = user.Identificacion
            };
        }

        public async Task<UserEditDto> GetUserByIdentificacionAsync(string identificacion)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Identificacion == identificacion);

            if (user == null) return null;

            return new UserEditDto
            {
                Id = user.Id,
                Email = user.Email,
                Nombre = user.Nombre,
                Apellido = user.Apellido,
                Identificacion = user.Identificacion
            };
        }

    }
}
