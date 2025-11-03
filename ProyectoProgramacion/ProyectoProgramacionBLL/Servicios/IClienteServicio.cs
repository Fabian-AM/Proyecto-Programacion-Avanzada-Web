using ProyectoProgramacionBLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Servicios
{
    public interface IClienteServicio
    {
        Task<CustomResponse<ClienteDto>> ObtenerClientePorIdAsync(int id);
        Task<CustomResponse<List<ClienteDto>>> ObtenerClientesAsync();
        Task<CustomResponse<ClienteDto>> AgregarClienteAsync(ClienteDto clienteDto);
        Task<CustomResponse<ClienteDto>> ActualizarClienteAsync(ClienteDto clienteDto);
        Task<CustomResponse<ClienteDto>> EliminarClienteAsync(int id);
    }
}
