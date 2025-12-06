using AutoMapper;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionDAL.Entidades;
using ProyectoProgramacionDAL.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Servicios
{
    public class SolicitudesServicio : ISolicitudesServicio
    {
        private readonly ISolicitudesRepositorio _solicitudesRepositorio;

        private readonly IMapper _mapper;

        public SolicitudesServicio(ISolicitudesRepositorio solicitudesRepositorio, IMapper mapper)

        {
            _solicitudesRepositorio = solicitudesRepositorio;
            _mapper = mapper;

        }

        public async Task<CustomResponse<SolicitudDto>> CrearSolicitudAsync(SolicitudDto solicitudDto)
        {
            var respuesta = new CustomResponse<SolicitudDto>();

            if (solicitudDto.MontoCredito > 10000000)
            {
                respuesta.EsError = true;
               respuesta.Mensaje = "No se permite ingresar solicitud de crédito por un monto mayor a 10.000.000 colones";
                return respuesta;

            }

            // 2. REGLA DE NEGOCIO (PDF Pág 3): No permitir si ya tiene una en "Ingresado" o "Devolución"
            bool tieneActiva = await _solicitudesRepositorio.ClienteTieneSolicitudActivaAsync(solicitudDto.ClienteID);
            if (tieneActiva)
            {
                respuesta.EsError = true;
                // El mensaje exacto solicitado en el PDF
                respuesta.Mensaje = $"El usuario ya cuenta con una solicitud de crédito en proceso, por favor resolver la gestión antes de ingresar otra nueva";
                return respuesta;
            }

            var nuevaSolicitud = _mapper.Map<Solicitud>(solicitudDto);
            nuevaSolicitud.Estado = "Ingresado";
            if (!await _solicitudesRepositorio.AgregarSolicitudAsync(nuevaSolicitud))
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pudo crear la solicitud en la base de datos";
                return respuesta;
            }
            respuesta.Data = _mapper.Map<SolicitudDto>(nuevaSolicitud);
            return respuesta;
        }

        public async Task<CustomResponse<List<SolicitudDto>>> ObtenerTodasSolicitudesAsync()
        {
            var respuesta = new CustomResponse<List<SolicitudDto>>();
            var lista = await _solicitudesRepositorio.ObtenerSolicitudesAsync();
            respuesta.Data = _mapper.Map<List<SolicitudDto>>(lista);
            return respuesta;
        }

        public async Task<CustomResponse<SolicitudDto>> ObtenerSolicitudPorIdAsync(int id)
        {
            var respuesta = new CustomResponse<SolicitudDto>();
            var solicitud = await _solicitudesRepositorio.ObtenerSolicitudPorIdAsync(id);

            if (solicitud == null)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "Solicitud no encontrada";
                return respuesta;
            }

            respuesta.Data = _mapper.Map<SolicitudDto>(solicitud);
            return respuesta;
        }

        public async Task<CustomResponse<SolicitudDto>> ActualizarSolicitudAsync(SolicitudDto solicitudDto)
        {
            var respuesta = new CustomResponse<SolicitudDto>();

            try
            {
                var solicitudExistente = await _solicitudesRepositorio.ObtenerSolicitudPorIdAsync(solicitudDto.SolicitudID);

                if (solicitudExistente == null)
                {
                    respuesta.EsError = true;
                    respuesta.Mensaje = "La solicitud que intentas editar no existe.";
                    return respuesta;
                }

                _mapper.Map(solicitudDto, solicitudExistente);

                bool guardado = await _solicitudesRepositorio.ActualizarSolicitudAsync(solicitudExistente);

                if (!guardado)
                {
                    respuesta.EsError = true;
                    respuesta.Mensaje = "No se pudieron guardar los cambios en la base de datos.";
                    return respuesta;
                }

                respuesta.Data = _mapper.Map<SolicitudDto>(solicitudExistente);
                respuesta.Mensaje = "Solicitud actualizada correctamente.";
            }
            catch (Exception ex)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = $"Error interno: {ex.Message}";
            }
            return respuesta;
        }
    }
}