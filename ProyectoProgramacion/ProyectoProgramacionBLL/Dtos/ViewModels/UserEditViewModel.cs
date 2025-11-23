using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Dtos.ViewModels
{
    public class UserEditViewModel : IValidatableObject
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe ser un email válido")]
        public string Email { get; set; }

        public string Identificacion { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El apellido es obligatorio")]
        public string Apellido { get; set; }
        public List<string> SelectedRoles { get; set; } = new List<string>();

        public List<RoleCheckbox> Roles { get; set; } = new List<RoleCheckbox>();

        // Validación personalizada
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SelectedRoles == null || !SelectedRoles.Any())
            {
                yield return new ValidationResult(
                    "Debe seleccionar al menos un rol",
                    new[] { nameof(SelectedRoles) });
            }
        }
    }

}
