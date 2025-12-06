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
                .Include(s => s.Documentos) 
                .FirstOrDefaultAsync(s => s.SolicitudID == id);
        }

        public async Task<List<Solicitud>> ObtenerSolicitudesAsync(IList<string> rolesUsuario)
        {
            var query = _context.Solicitudes
                .Include(s => s.Cliente)
                .OrderByDescending(s => s.FechaCreacion)
                .AsQueryable();

            if (rolesUsuario.Contains("Admin"))
            {
                return await query.ToListAsync();
            }

            var estadosVisibles = new List<string>();

            if (rolesUsuario.Contains("Analista"))
            {
                estadosVisibles.Add("Ingresado");
                estadosVisibles.Add("Devolución");
            }

            if (rolesUsuario.Contains("Gestor"))
            {
                estadosVisibles.Add("Enviado Aprobación");
            }

            query = query.Where(s => estadosVisibles.Contains(s.Estado));

            return await query.ToListAsync();
        }

        public async Task<bool> ClienteTieneSolicitudActivaAsync(int clienteId)
        {
            var estadosActivos = new[] { "Ingresado", "Devolución" };

            return await _context.Solicitudes
                .AnyAsync(s => s.ClienteID == clienteId && estadosActivos.Contains(s.Estado));
        }

        public async Task<bool> AgregarBitacoraAsync(BitacoraMovimiento movimiento)
        {
            try
            {
                await _context.BitacoraMovimientos.AddAsync(movimiento);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> AgregarDocumentoAsync(Documento documento)
        {
            try
            {
                await _context.Documentos.AddAsync(documento);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<List<BitacoraMovimiento>> ObtenerHistorialAsync(int solicitudId)
        {
            return await _context.BitacoraMovimientos
                .Include(b => b.Usuario) 
                .Where(b => b.SolicitudID == solicitudId)
                .OrderByDescending(b => b.FechaMovimiento) 
                .ToListAsync();
        }
        public async Task<Solicitud> ObtenerSolicitudActivaPorClienteAsync(int clienteId)
        {
            return await _context.Solicitudes
                .Include(s => s.Cliente) 
                .Where(s => s.ClienteID == clienteId &&
                            (s.Estado == "Ingresado" || s.Estado == "Devolución"))
                .FirstOrDefaultAsync();
        }
    }
}