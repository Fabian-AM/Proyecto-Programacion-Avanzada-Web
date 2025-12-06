using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos
{
    public class SolicitudDto
    {
        public int SolicitudID { get; set; }
        public int ClienteID { get; set; }
        public decimal MontoCredito { get; set; }
        public string Estado { get; set; }
        public string ComentariosIniciales { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string NombreCliente { get; set; }
        public IFormFile? DocumentoAdjunto { get; set; }
        public List<DocumentoDto> DocumentosExistentes { get; set; } = new List<DocumentoDto>();
    }
}