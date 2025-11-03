using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ProyectoProgramacionDAL.Entidades
{
    public class ApplicationUser : IdentityUser
    {
        public string Identificacion { get; set; } 
        public string Nombre { get; set; } 
        public string Apellido { get; set; }

    }
}
