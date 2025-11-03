using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos
{
    public class CustomResponse<T>
    {
        public bool EsError { get; set; }

        public string Mensaje { get; set; }

        public T Data { get; set; }

        public CustomResponse()
        {
            EsError = false;
            Mensaje = "Acción realizada correctamente";
        }
    }
}
