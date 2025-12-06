using AutoMapper;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionDAL.Entidades;

namespace ProyectoProgramacionBLL.Mapeos
{
    public class MapeoClases : Profile
    {
        public MapeoClases()
        {
            // 1. Clientes
            CreateMap<Cliente, ClienteDto>().ReverseMap();

            // 2. Documentos (ESTE ES EL QUE TE FALTA Y CAUSA EL ERROR)
            CreateMap<Documento, DocumentoDto>()
                .ForMember(dest => dest.FechaSubida, opt => opt.MapFrom(src => src.FechaSubida.ToString("dd/MM/yyyy HH:mm")))
                .ReverseMap();

            // 3. Solicitudes (Asegúrate que tenga la línea de DocumentosExistentes)
            CreateMap<Solicitud, SolicitudDto>()
                .ForMember(dest => dest.NombreCliente, opt => opt.MapFrom(src => src.Cliente.Nombre + " " + src.Cliente.Apellido))
                .ForMember(dest => dest.DocumentosExistentes, opt => opt.MapFrom(src => src.Documentos))
                .ReverseMap();
        }
    }
}