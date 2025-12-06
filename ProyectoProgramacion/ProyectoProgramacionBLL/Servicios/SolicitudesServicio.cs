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

            // Validaciones existentes...
            if (solicitudDto.MontoCredito > 10000000)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se permite ingresar solicitud de crédito por un monto mayor a 10.000.000 colones";
                return respuesta;
            }

            var solicitudActiva = await _solicitudesRepositorio.ObtenerSolicitudActivaPorClienteAsync(solicitudDto.ClienteID);

            if (solicitudActiva != null)
            {
                // Asegúrate de que tu entidad Cliente tenga la propiedad 'Identificacion'.
                // Si no la tiene, agrégala o usa solicitudActiva.Cliente.Id como fallback.
                string identificacion = solicitudActiva.Cliente.Identificacion.ToString();
                respuesta.EsError = true;
                respuesta.Mensaje = $"El usuario con identificación {identificacion} ya cuenta con la solicitud de crédito {solicitudActiva.SolicitudID}, por favor resolver la gestión antes de ingresar otra nueva";
                return respuesta;
            }

            var nuevaSolicitud = _mapper.Map<Solicitud>(solicitudDto);
            nuevaSolicitud.Estado = "Ingresado";
            nuevaSolicitud.FechaCreacion = DateTime.Now; // Asegurar fecha

            if (!await _solicitudesRepositorio.AgregarSolicitudAsync(nuevaSolicitud))
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pudo crear la solicitud en la base de datos";
                return respuesta;
            }
            if (solicitudDto.DocumentoAdjunto != null)
            {
                // 1. Definir ruta: wwwroot/documentos
                string carpetaDestino = Path.Combine(_webHostEnvironment.WebRootPath, "documentos");
                if (!Directory.Exists(carpetaDestino)) Directory.CreateDirectory(carpetaDestino);

                // 2. Generar nombre único (para no sobrescribir)
                string nombreArchivo = Guid.NewGuid().ToString() + Path.GetExtension(solicitudDto.DocumentoAdjunto.FileName);
                string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

                // 3. Guardar archivo en disco
                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await solicitudDto.DocumentoAdjunto.CopyToAsync(stream);
                }

                // 4. Guardar registro en BD
                var nuevoDoc = new Documento
                {
                    SolicitudID = nuevaSolicitud.SolicitudID,
                    NombreArchivo = solicitudDto.DocumentoAdjunto.FileName, // Nombre original
                    RutaAlmacenamiento = "documentos/" + nombreArchivo, // Ruta relativa para usar en web
                    FechaSubida = DateTime.Now
                };

                await _solicitudesRepositorio.AgregarDocumentoAsync(nuevoDoc);
            }
            // --- BITÁCORA: Registrar creación ---
            var bitacora = new BitacoraMovimiento
            {
                SolicitudID = nuevaSolicitud.SolicitudID, // EF Core ya generó el ID
                UsuarioID = usuarioId,
                Accion = "Crear",
                Comentario = "Se crea la gestión para el cliente",
                FechaMovimiento = DateTime.Now
            };
            await _solicitudesRepositorio.AgregarBitacoraAsync(bitacora);
            // ------------------------------------

            respuesta.Data = _mapper.Map<SolicitudDto>(nuevaSolicitud);
            return respuesta;
        }

        public async Task<CustomResponse<List<SolicitudDto>>> ObtenerTodasSolicitudesAsync(IList<string> rolesUsuario)
        {
            var respuesta = new CustomResponse<List<SolicitudDto>>();

            // Pasamos los roles al repositorio
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
                if (solicitudExistente == null) { /* ... error ... */ return respuesta; }

                // 1. Detectar cambio de estado y asignar valores nuevos
                // NOTA: Si el botón envía "Estado", solicitudDto.Estado traerá el NUEVO valor.
                string accion = solicitudDto.Estado ?? solicitudExistente.Estado;

                // Mapeamos los cambios (incluyendo el nuevo Estado si vino del botón)
                _mapper.Map(solicitudDto, solicitudExistente);

                // Si el estado venía null (ej: guardado simple), mantenemos el anterior
                if (string.IsNullOrEmpty(solicitudDto.Estado))
                {
                    solicitudExistente.Estado = accion; // Restaurar o mantener
                }

                // 2. --- NUEVA LÓGICA DE ARCHIVOS (Igual que en Create) ---
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

                // 3. Guardar Solicitud
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

            // Consultar al repositorio
            var historialEntidad = await _solicitudesRepositorio.ObtenerHistorialAsync(id);

            // Mapeo manual (o puedes usar AutoMapper si lo prefieres)
            var historialDto = historialEntidad.Select(h => new BitacoraDto
            {
                Fecha = h.FechaMovimiento.ToString("dd/MM/yyyy HH:mm"), // Formato amigable
                Accion = h.Accion,
                Comentario = h.Comentario,
                // Manejo seguro de nulos por si se borró el usuario
                NombreUsuario = h.Usuario != null ? $"{h.Usuario.Nombre} {h.Usuario.Apellido}" : "Usuario Desconocido",
                // Opcional: Podrías buscar el rol si lo necesitas, por ahora mandamos vacío o hardcodeado
                RoleUsuario = "N/A"
            }).ToList();

            respuesta.Data = historialDto;
            return respuesta;
        }

    }
}