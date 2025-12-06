using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionBLL.Servicios;
using ProyectoProgramacionDAL.Entidades;
using System.Threading.Tasks;

namespace ProyectoProgramacion.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ISolicitudesServicio _solicitudesServicio;
        private readonly IClienteServicio _clienteServicio; // Necesario para listar clientes en el Create
        private readonly UserManager<ApplicationUser> _userManager;

        public SolicitudesController(ISolicitudesServicio solicitudesServicio,IClienteServicio clienteServicio,UserManager<ApplicationUser> userManager) // 1. Agrega este parámetro
        {
            _solicitudesServicio = solicitudesServicio;
            _clienteServicio = clienteServicio;
            _userManager = userManager;
        }

        // GET: Solicitudes
        public async Task<IActionResult> Index()
        {
            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return Challenge();
            var roles = await _userManager.GetRolesAsync(usuario);
            var respuesta = await _solicitudesServicio.ObtenerTodasSolicitudesAsync(roles);

            if (respuesta.EsError)
            {
                ViewBag.Error = respuesta.Mensaje;
                return View(new List<SolicitudDto>());
            }

            return View(respuesta.Data);
        }

        // GET: Solicitudes/Create
        [Authorize(Roles = "Servicio al Cliente,Administrador")]
        public async Task<IActionResult> Create()
        {
            await CargarListaClientes();
            return View();
        }

        // POST: Solicitudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Servicio al Cliente,Administrador")]
        public async Task<IActionResult> Create(SolicitudDto solicitudDto)
        {
            ModelState.Remove("Estado");
            ModelState.Remove("NombreCliente");
            solicitudDto.Estado = "Pendiente";

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Datos del formulario inválidos." });
            }

            var usuario = await _userManager.GetUserAsync(User);
            if (usuario == null) return Challenge();
            var respuesta = await _solicitudesServicio.CrearSolicitudAsync(solicitudDto, usuario.Id);

            if (respuesta.EsError)
            {
                return Json(new { success = false, message = respuesta.Mensaje });
            }

            return Json(new { success = true, message = "La solicitud se ha creado correctamente." });
        }

        // Método auxiliar para llenar el Dropdown de Clientes
        private async Task CargarListaClientes()
        {
            // Asumo que tu servicio de clientes tiene un método para obtener todos
            // Si tu IClienteServicio devuelve un CustomResponse, la lógica sería así:
            var respuesta = await _clienteServicio.ObtenerClientesAsync();

            // Ajusta "Id" y "Nombre" según como se llamen las propiedades en tu ClienteDto
            // Nota: concatenamos Nombre y Apellido para que se vea mejor en el dropdown
            if (respuesta != null) // Ajustar según estructura de respuesta de ClienteServicio
            {
                // Creamos una lista anónima para el select list
                var clientesSelect = respuesta.Data.Select(c => new {
                    Id = c.Id,
                    NombreCompleto = $"{c.Nombre} {c.Apellido}" // Asegúrate de tener Apellido en el DTO si lo usas
                });

                ViewBag.ClienteID = new SelectList(clientesSelect, "Id", "NombreCompleto");
            }
        }
        // 1. GET: Para abrir la pantalla de edición
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var respuesta = await _solicitudesServicio.ObtenerSolicitudPorIdAsync(id);

            if (respuesta.EsError || respuesta.Data == null)
            {
                return NotFound();
            }

            await CargarListaClientes();

            // CAMBIO CRÍTICO: Usar PartialView en lugar de View
            // Esto devuelve solo el HTML del formulario, sin el Layout completo.
            return PartialView(respuesta.Data);
        }

        // 2. POST: Para guardar los cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SolicitudDto solicitudDto)
        {
            ModelState.Remove("NombreCliente");

            if (ModelState.IsValid)
            {
                // OBTENER ID DEL USUARIO ACTUAL
                var usuario = await _userManager.GetUserAsync(User);
                if (usuario == null) return Challenge();

                // Pasamos el ID al servicio
                var respuesta = await _solicitudesServicio.ActualizarSolicitudAsync(solicitudDto, usuario.Id);

                if (!respuesta.EsError)
                {
                    TempData["MensajeExito"] = "La solicitud ha sido actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", respuesta.Mensaje);
            }
            await CargarListaClientes();
            return View(solicitudDto);
        }
        [HttpGet]
        public async Task<IActionResult> Historial(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var respuesta = await _solicitudesServicio.ObtenerHistorialSolicitudAsync(id);

            if (respuesta.EsError)
            {
                return BadRequest(respuesta.Mensaje);
            }

            // Retornamos una VISTA PARCIAL, no un JSON ni una Vista completa
            return PartialView("_Historial", respuesta.Data);
        }
    }
}