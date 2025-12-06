using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoProgramacionDAL.Entidades
{
    public class Documento
    {
        [Key]
        public int DocumentoID { get; set; }

        [Required]
        public int SolicitudID { get; set; }

        [ForeignKey("SolicitudID")]
        public Solicitud Solicitud { get; set; }

        [Required]
        public string NombreArchivo { get; set; }

        [Required]
        public string RutaAlmacenamiento { get; set; }

        public DateTime FechaSubida { get; set; } = DateTime.Now;
    }
}