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
    }
}
