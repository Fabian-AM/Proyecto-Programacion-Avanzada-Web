using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionDAL.Contexto;
using ProyectoProgramacionDAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionDAL.Repositorios
{
    public class SolicitudesRepositorio : ISolicitudesRepositorio
    {
        private readonly AppDbContext _context;

        public SolicitudesRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AgregarSolicitudAsync(Solicitud solicitud)
        {
            try
            {
                await _context.Solicitudes.AddAsync(solicitud);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> ActualizarSolicitudAsync(Solicitud solicitud)
        {
            try
            {
                _context.Solicitudes.Update(solicitud);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Solicitud> ObtenerSolicitudPorIdAsync(int id)
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente) 
                .FirstOrDefaultAsync(s => s.SolicitudID == id);
        }

        public async Task<List<Solicitud>> ObtenerSolicitudesAsync()
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente)
                .OrderByDescending(s => s.FechaCreacion)
                .ToListAsync();
        }

        public async Task<bool> ClienteTieneSolicitudActivaAsync(int clienteId)
        {
            var estadosActivos = new[] { "Ingresado", "Devolución" };

            return await _context.Solicitudes
                .AnyAsync(s => s.ClienteID == clienteId && estadosActivos.Contains(s.Estado));
        }
    }
}