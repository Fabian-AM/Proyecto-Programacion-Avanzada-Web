using ProyectoProgramacionDAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionDAL.Repositorios
{
    public interface ISolicitudesRepositorio
    {
        Task<bool> AgregarSolicitudAsync(Solicitud solicitud);
        Task<bool> ActualizarSolicitudAsync(Solicitud solicitud);
        Task<Solicitud> ObtenerSolicitudPorIdAsync(int id);
        Task<List<Solicitud>> ObtenerSolicitudesAsync();
        Task<bool> ClienteTieneSolicitudActivaAsync(int clienteId);
    }
}