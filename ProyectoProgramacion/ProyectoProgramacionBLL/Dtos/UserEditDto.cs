using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos
{
    public class UserEditDto
    {
        public string Id { get; set; } = string.Empty;

        public string Identificacion { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Apellido { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string RolActual { get; set; } = string.Empty;

        public string NuevoRol { get; set; } = string.Empty;

        public List<string> Roles { get; set; } = new List<string>();
    }
}
