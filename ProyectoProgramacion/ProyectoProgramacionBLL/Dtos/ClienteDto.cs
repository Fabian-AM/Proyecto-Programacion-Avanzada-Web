using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos
{
    public class ClienteDto
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nombre es obligatorio.")] 
        public string Nombre { get; set; }
        [Required(ErrorMessage = "Apellido es obligatorio.")]
        public string Apellido { get; set; }

        [Required(ErrorMessage = "La identificacion es obligatoria.")]
        public int? Identificacion { get; set; }

        [Required(ErrorMessage = "Edad es obligatoria.")]
        public int? Edad { get; set; }
    }
}
