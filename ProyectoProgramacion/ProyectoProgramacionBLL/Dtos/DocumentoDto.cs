using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos
{
    public class DocumentoDto
    {
        public int DocumentoID { get; set; }
        public string NombreArchivo { get; set; }
        public string RutaAlmacenamiento { get; set; }
        public string FechaSubida { get; set; }
    }
}
