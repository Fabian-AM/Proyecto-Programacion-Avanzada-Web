using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionDAL.Contexto;
using ProyectoProgramacionDAL.Entidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoProgramacionDAL.Repositorios
{
    public class ClientesRepositorio : IClientesRepositorio
    {
        private readonly AppDbContext _context;

        public ClientesRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> ObtenerClienteAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente> ObtenerClientePorIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task<bool> AgregarClienteAsync(Cliente cliente)
        {
            // Entity Framework maneja el ID autoincremental, no es necesario calcularlo manualmente
            try
            {
                _context.Clientes.Add(cliente);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarClienteAsync(Cliente cliente)
        {
            var clienteExistente = await _context.Clientes.FindAsync(cliente.Id);
            if (clienteExistente == null)
            {
                return false;
            }

            // Actualizamos las propiedades
            clienteExistente.Nombre = cliente.Nombre;
            clienteExistente.Apellido = cliente.Apellido;
            clienteExistente.Identificacion = cliente.Identificacion;
            clienteExistente.Edad = cliente.Edad;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EliminarClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                return false;
            }

            try
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}