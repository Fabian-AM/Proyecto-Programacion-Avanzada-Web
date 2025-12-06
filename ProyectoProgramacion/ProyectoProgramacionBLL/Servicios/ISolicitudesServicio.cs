using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionDAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Servicios
{
    public interface ISolicitudesServicio
    {
        Task<CustomResponse<SolicitudDto>> CrearSolicitudAsync(SolicitudDto solicitudDto, string usuarioId);
        Task<CustomResponse<List<SolicitudDto>>> ObtenerTodasSolicitudesAsync(IList<string> rolesUsuario);
        Task<CustomResponse<SolicitudDto>> ObtenerSolicitudPorIdAsync(int id);
        Task<CustomResponse<SolicitudDto>> ActualizarSolicitudAsync(SolicitudDto solicitudDto, string usuarioId);       
        Task<CustomResponse<List<BitacoraDto>>> ObtenerHistorialSolicitudAsync(int id);
    }
}
