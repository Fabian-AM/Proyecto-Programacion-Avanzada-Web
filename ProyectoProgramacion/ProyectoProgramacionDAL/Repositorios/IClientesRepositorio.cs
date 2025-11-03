using ProyectoProgramacionDAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionDAL.Repositorios
{
    public interface IClientesRepositorio
    {
        Task<List<Cliente>> ObtenerClienteAsync();
        Task<Cliente> ObtenerClientePorIdAsync(int id);
        Task<bool> AgregarClienteAsync(Cliente cliente);
        Task<bool> ActualizarClienteAsync(Cliente cliente);
        Task<bool> EliminarClienteAsync(int id);
    }
}
