using ProyectoProgramacionDAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionDAL.Repositorios
{
    public class ClientesRepositorio : IClientesRepositorio
    {
        private  List<Cliente> clientes = new List<Cliente>()
        {
            new Cliente { Id = 1, Nombre = "Juan", Apellido = "Pérez",Identificacion = 143241231, Edad = 30 },
            new Cliente { Id = 2, Nombre = "María", Apellido = "Gómez",Identificacion = 231348745, Edad = 25 },
            new Cliente { Id = 3, Nombre = "Carlos", Apellido = "López",Identificacion = 332335821, Edad = 28 }
        };

        public async Task<bool> ActualizarClienteAsync(Cliente cliente)
        {
            var clienteExistente = clientes.FirstOrDefault(u => u.Id == cliente.Id);
            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Apellido = cliente.Apellido;
            clienteExistente.Identificacion = cliente.Identificacion;
            clienteExistente.Edad = cliente.Edad;

            return true;
        }

        public async Task<bool> AgregarClienteAsync(Cliente cliente)
        {
            cliente.Id = clientes.Any() ? clientes.Max(u => u.Id) + 1 : 1;
            clientes.Add(cliente);
            return true;
        }

        public async Task<bool> EliminarClienteAsync(int id)
        {
            var cliente = clientes.FirstOrDefault(u => u.Id == id);
            if (cliente != null)
            {
                clientes.Remove(cliente);
                return true;
            }
            return false;
        }

        public async Task<Cliente> ObtenerClientePorIdAsync(int id)
        {
            //SP //API // ETC
            var cliente = clientes.FirstOrDefault(u => u.Id == id);
            return cliente;
        }

        public async Task<List<Cliente>> ObtenerClienteAsync()
        {
            return clientes;
        }
    }
}
