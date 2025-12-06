using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoProgramacionDAL.Entidades
{
    public class Solicitud
    {
        [Key]
        public int SolicitudID { get; set; }

        [Required]
        public int ClienteID { get; set; }

        [ForeignKey("ClienteID")]
        public Cliente Cliente { get; set; }

        [Required]
        [Column(TypeName = "decimal(18, 2)")]
        public decimal MontoCredito { get; set; }

        [Required]
        public string Estado { get; set; } = "Ingresado"; // Ingresado, Devolución, Enviado Aprobación, Aprobado

        public string ComentariosIniciales { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Relaciones
        public ICollection<Documento> Documentos { get; set; }
        public ICollection<BitacoraMovimiento> Movimientos { get; set; }
    }
}