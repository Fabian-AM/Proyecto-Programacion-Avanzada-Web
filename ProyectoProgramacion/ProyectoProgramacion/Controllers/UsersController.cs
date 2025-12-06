using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoProgramacionBLL.Dtos.ViewModels;
using ProyectoProgramacionBLL.Servicios;

namespace ProyectoProgramacion.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IAccountService _accountService;

        public UsersController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _accountService.GetAllUsersAsync();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var dto = await _accountService.GetUserByIdAsync(id);
            if (dto == null) return NotFound();

            var allRoles = new List<string> { "Admin", "Analista", "Gestor", "Servicio al Cliente", "Cliente" };
            var userRoles = await _accountService.GetUserRolesAsync(id);

            var vm = new UserEditViewModel
            {
                Id = dto.Id,
                Email = dto.Email,
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Identificacion = dto.Identificacion,
                Roles = allRoles.Select(r => new RoleCheckbox
                {
                    RoleName = r,
                    Selected = userRoles.Contains(r)
                }).ToList()
            };

            return PartialView("_Edit", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                });
            }

            var existingEmail = await _accountService.GetUserByEmailAsync(model.Email);
            if (existingEmail != null && existingEmail.Id != model.Id)
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "El correo ingresado ya está registrado con otro usuario." }
                });
            }

            if (!string.IsNullOrWhiteSpace(model.Identificacion))
            {
                var existingId = await _accountService.GetUserByIdentificacionAsync(model.Identificacion);
                if (existingId != null && existingId.Id != model.Id)
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] { "La identificación ingresada ya está registrada con otro usuario." }
                    });
                }
            }

            var result = await _accountService.UpdateUserAsync(model);

            if (!result.Succeeded)
            {
                return Json(new
                {
                    success = false,
                    errors = result.Errors
                });
            }

            if (model.SelectedRoles != null)
            {
                var rolesResult = await _accountService.SetUserRolesAsync(model.Id, model.SelectedRoles);

                if (!rolesResult.Succeeded)
                {
                    return Json(new
                    {
                        success = false,
                        errors = rolesResult.Errors
                    });
                }
            }

            return Json(new { success = true, message = "Usuario actualizado correctamente" });
        }

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("_Create", new UserCreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            }

            var emailExists = await _accountService.GetUserByEmailAsync(model.Email);
            if (emailExists != null)
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "El correo ya está registrado." }
                });
            }

            if (!string.IsNullOrWhiteSpace(model.Identificacion))
            {
                var idExists = await _accountService.GetUserByIdentificacionAsync(model.Identificacion);
                if (idExists != null)
                {
                    return Json(new
                    {
                        success = false,
                        errors = new[] { "La identificación ya está registrada." }
                    });
                }
            }

            if (model.Roles == null || !model.Roles.Any())
            {
                return Json(new
                {
                    success = false,
                    errors = new[] { "Debe seleccionar al menos un rol." }
                });
            }

            var result = await _accountService.CreateUserAsync(model);

            if (!result.Succeeded)
                return Json(new { success = false, errors = result.Errors });

            return Json(new { success = true, message = "Usuario creado exitosamente" });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _accountService.DeleteUserAsync(id);

            if (!result.Succeeded)
                return Json(new { success = false, errors = result.Errors });

            return Json(new { success = true, message = "Usuario eliminado correctamente" });
        }

        [HttpGet]
        public async Task<IActionResult> Roles(string id)
        {
            var user = await _accountService.GetUserByIdAsync(id);
            if (user == null) return NotFound();

            var rolesUsuario = await _accountService.GetUserRolesAsync(id);

            var rolesSistema = new List<string>
            {
                "Admin",
                "Analista",
                "Gestor",
                "Servicio al Cliente",
                "Cliente"
            };

            var vm = new UserRolesViewModel
            {
                UserId = id,
                Email = user.Email,
                Roles = rolesSistema
                    .Select(r => new RoleCheckbox
                    {
                        RoleName = r,
                        Selected = rolesUsuario.Contains(r)
                    })
                    .ToList()
            };

            return PartialView("_Roles", vm);
        }

        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            var dto = await _accountService.GetUserByIdAsync(id);
            if (dto == null) return NotFound();

            return PartialView("_Details", dto);
        }


        [HttpPost]
        public async Task<IActionResult> Roles(UserRolesViewModel model)
        {
            var rolesSeleccionados = model.Roles
                .Where(r => r.Selected)
                .Select(r => r.RoleName)
                .ToList();

            var result = await _accountService.SetUserRolesAsync(model.UserId, rolesSeleccionados);

            if (!result.Succeeded)
                return Json(new { success = false, errors = result.Errors });

            return Json(new { success = true, message = "Roles actualizados correctamente" });
        }
    }
}
