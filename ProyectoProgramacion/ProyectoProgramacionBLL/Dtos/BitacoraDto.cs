using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos
{
    public class BitacoraDto
    {
        public string Fecha { get; set; }
        public string Accion { get; set; }
        public string Comentario { get; set; }
        public string NombreUsuario { get; set; }
        public string RoleUsuario { get; set; }
    }
}