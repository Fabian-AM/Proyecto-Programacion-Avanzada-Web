using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos.ViewModels
{
    public class UserListDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Identificacion { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public List<string> Roles { get; set; }
    }
}
