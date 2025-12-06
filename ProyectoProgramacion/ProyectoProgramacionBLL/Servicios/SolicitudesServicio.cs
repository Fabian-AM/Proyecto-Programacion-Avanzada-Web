using AutoMapper;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionDAL.Entidades;
using ProyectoProgramacionDAL.Repositorios;
using System;
using System.IO;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SolicitudesServicio(ISolicitudesRepositorio solicitudesRepositorio, IMapper mapper, IWebHostEnvironment webHostEnvironment)

        {
            _solicitudesRepositorio = solicitudesRepositorio;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<CustomResponse<SolicitudDto>> CrearSolicitudAsync(SolicitudDto solicitudDto, string usuarioId)
        {
            var respuesta = new CustomResponse<SolicitudDto>();

            if (solicitudDto.MontoCredito > 10000000)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se permite ingresar solicitud de crédito por un monto mayor a 10.000.000 colones";
                return respuesta;
            }

            var solicitudActiva = await _solicitudesRepositorio.ObtenerSolicitudActivaPorClienteAsync(solicitudDto.ClienteID);

            if (solicitudActiva != null)
            {

                string identificacion = solicitudActiva.Cliente.Identificacion.ToString();
                respuesta.EsError = true;
                respuesta.Mensaje = $"El usuario con identificación {identificacion} ya cuenta con la solicitud de crédito {solicitudActiva.SolicitudID}, por favor resolver la gestión antes de ingresar otra nueva";
                return respuesta;
            }

            var nuevaSolicitud = _mapper.Map<Solicitud>(solicitudDto);
            nuevaSolicitud.Estado = "Ingresado";
            nuevaSolicitud.FechaCreacion = DateTime.Now; 

            if (!await _solicitudesRepositorio.AgregarSolicitudAsync(nuevaSolicitud))
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pudo crear la solicitud en la base de datos";
                return respuesta;
            }
            if (solicitudDto.DocumentoAdjunto != null)
            {

                string carpetaDestino = Path.Combine(_webHostEnvironment.WebRootPath, "documentos");
                if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(solicitudDto.DocumentoAdjunto.FileName);
                string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await solicitudDto.DocumentoAdjunto.CopyToAsync(stream);
                }

                var nuevoDoc = new Documento
                {
                    SolicitudID = nuevaSolicitud.SolicitudID,
                    NombreArchivo = solicitudDto.DocumentoAdjunto.FileName, 
                    RutaAlmacenamiento = "documentos/" + nombreArchivo, 
                    FechaSubida = DateTime.Now
                };

                await _solicitudesRepositorio.AgregarDocumentoAsync(nuevoDoc);
            }

            var bitacora = new BitacoraMovimiento
            {
                SolicitudID = nuevaSolicitud.SolicitudID, 
                UsuarioID = usuarioId,
                Accion = "Crear",
                Comentario = "Se crea la gestión para el cliente",
                FechaMovimiento = DateTime.Now
            };
            await _solicitudesRepositorio.AgregarBitacoraAsync(bitacora);


            respuesta.Data = _mapper.Map<SolicitudDto>(nuevaSolicitud);
            return respuesta;
        }

        public async Task<CustomResponse<List<SolicitudDto>>> ObtenerTodasSolicitudesAsync(IList<string> rolesUsuario)
        {
            var respuesta = new CustomResponse<List<SolicitudDto>>();

            var lista = await _solicitudesRepositorio.ObtenerSolicitudesAsync(rolesUsuario);

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

        public async Task<CustomResponse<SolicitudDto>> ActualizarSolicitudAsync(SolicitudDto solicitudDto, string usuarioId)
        {
            var respuesta = new CustomResponse<SolicitudDto>();

            try
            {
                var solicitudExistente = await _solicitudesRepositorio.ObtenerSolicitudPorIdAsync(solicitudDto.SolicitudID);
                if (solicitudExistente == null) {  return respuesta; }


                string accion = solicitudDto.Estado ?? solicitudExistente.Estado;

                _mapper.Map(solicitudDto, solicitudExistente);

                if (string.IsNullOrEmpty(solicitudDto.Estado))
                {
                    solicitudExistente.Estado = accion; 
                }

                if (solicitudDto.DocumentoAdjunto != null)
                {
                    string carpetaDestino = Path.Combine(_webHostEnvironment.WebRootPath, "documentos");
                    if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

                    string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(solicitudDto.DocumentoAdjunto.FileName);
                    string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

                    using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                    {
                        await solicitudDto.DocumentoAdjunto.CopyToAsync(stream);
                    }

                    var nuevoDoc = new Documento
                    {
                        SolicitudID = solicitudExistente.SolicitudID,
                        NombreArchivo = solicitudDto.DocumentoAdjunto.FileName,
                        RutaAlmacenamiento = "documentos/" + nombreArchivo,
                        FechaSubida = DateTime.Now
                    };
                    await _solicitudesRepositorio.AgregarDocumentoAsync(nuevoDoc);
                }

                bool guardado = await _solicitudesRepositorio.ActualizarSolicitudAsync(solicitudExistente);
                if (!guardado) { return respuesta; }
                var bitacora = new BitacoraMovimiento
                {
                    SolicitudID = solicitudExistente.SolicitudID,
                    UsuarioID = usuarioId,
                    Accion = solicitudExistente.Estado, 
                    Comentario = solicitudDto.ComentariosIniciales ?? "Cambio de estado",
                    FechaMovimiento = DateTime.Now
                };
                await _solicitudesRepositorio.AgregarBitacoraAsync(bitacora);

                respuesta.Data = _mapper.Map<SolicitudDto>(solicitudExistente);
                respuesta.Mensaje = "Solicitud procesada correctamente.";
            }
            catch (Exception ex)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = $"Error interno: {ex.Message}";
            }
            return respuesta;
        }
        public async Task<CustomResponse<List<BitacoraDto>>> ObtenerHistorialSolicitudAsync(int id)
        {
            var respuesta = new CustomResponse<List<BitacoraDto>>();

            var historialEntidad = await _solicitudesRepositorio.ObtenerHistorialAsync(id);

            var historialDto = historialEntidad.Select(h => new BitacoraDto
            {
                Fecha = h.FechaMovimiento.ToString("dd/MM/yyyy HH:mm"), 
                Accion = h.Accion,
                Comentario = h.Comentario,
                NombreUsuario = h.Usuario != null ? $"{h.Usuario.Nombre} {h.Usuario.Apellido}" : "Usuario Desconocido",
                RoleUsuario = "N/A"
            }).ToList();

            respuesta.Data = historialDto;
            return respuesta;
        }

    }
}