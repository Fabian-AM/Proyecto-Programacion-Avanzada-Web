using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoProgramacionBLL.Dtos.ViewModels; 
using ProyectoProgramacionDAL.Entidades;
using ProyectoProgramacionBLL.Dtos;

namespace ProyectoProgramacionBLL.Servicios
{
    public interface IAccountService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterViewModel model);
        Task<AuthResponseDto> LoginAsync(LoginViewModel model);
        Task LogoutAsync();

        // ADMIN METHODS
        Task<UserEditDto> GetUserByEmailAsync(string email);
        Task<UserEditDto> GetUserByIdentificacionAsync(string identificacion);

        Task<List<UserListDto>> GetAllUsersAsync();
    Task<UserEditDto> GetUserByIdAsync(string id);
    Task<OperationResult> CreateUserAsync(UserCreateViewModel model);
    Task<OperationResult> UpdateUserAsync(UserEditViewModel model);
    Task<OperationResult> DeleteUserAsync(string id);
    Task<List<string>> GetUserRolesAsync(string userId);
    Task<OperationResult> SetUserRolesAsync(string userId, List<string> roles);
    }
}
