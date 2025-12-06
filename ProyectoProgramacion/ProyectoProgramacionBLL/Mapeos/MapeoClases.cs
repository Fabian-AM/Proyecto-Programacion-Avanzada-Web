using AutoMapper;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionDAL.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Mapeos
{
    public class MapeoClases : Profile
    {
        public MapeoClases()
        {
            CreateMap<Cliente, ClienteDto>().ReverseMap();          
            CreateMap<Solicitud, SolicitudDto>().ReverseMap();
        }
    }
}