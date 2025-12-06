using ProyectoProgramacionBLL.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Servicios
{
    public interface ISolicitudesServicio
    {
        Task<CustomResponse<SolicitudDto>> CrearSolicitudAsync(SolicitudDto solicitudDto);
        Task<CustomResponse<List<SolicitudDto>>> ObtenerTodasSolicitudesAsync();
        Task<CustomResponse<SolicitudDto>> ObtenerSolicitudPorIdAsync(int id);
        Task<CustomResponse<SolicitudDto>> ActualizarSolicitudAsync(SolicitudDto solicitudDto);
    }
}
