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
    public class BitacoraMovimiento
    {
        [Key]
        public int MovimientoID { get; set; }

        [Required]
        public int SolicitudID { get; set; }

        [ForeignKey("SolicitudID")]
        public Solicitud Solicitud { get; set; }

        // Relación con el Usuario que hizo la acción (IdentityUser usa string como ID)
        [Required]
        public string UsuarioID { get; set; }

        [ForeignKey("UsuarioID")]
        public ApplicationUser Usuario { get; set; }

        [Required]
        public string Accion { get; set; } // Ej: "Crear", "Aprobar", "Devolver"

        public string Comentario { get; set; }

        public DateTime FechaMovimiento { get; set; } = DateTime.Now;
    }
}