using Microsoft.AspNetCore.Mvc;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionBLL.Servicios;

namespace Caso1.Controllers
{
    public class ClienteController : Controller
    {
        private readonly ILogger<ClienteController> _logger;
        private readonly IClienteServicio _clientesServicio;

        public ClienteController(ILogger<ClienteController> logger, IClienteServicio clientesServicio)
        {
            _logger = logger;
            _clientesServicio = clientesServicio;
        }

        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "Listado de Clientes";
            return View();
        }

        public async Task<IActionResult> ObtenerClientes()
        {
            var respuesta = await _clientesServicio.ObtenerClientesAsync();

            if (respuesta.EsError || respuesta.Data == null)
                return Json(new { data = new List<ClienteDto>() });

            return Json(new { data = respuesta.Data });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCliente(ClienteDto clienteDto)
        {
            if (!ModelState.IsValid)
                return Json(new CustomResponse<ClienteDto> { EsError = true, Mensaje = "Error de validación" });

            var response = await _clientesServicio.AgregarClienteAsync(clienteDto);
            return Json(response);
        }

        public async Task<IActionResult> ObtenerClientePorId(int id)
        {
            var respuesta = await _clientesServicio.ObtenerClientePorIdAsync(id);
            return Json(respuesta);
        }

   
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCliente(ClienteDto clienteDto)
        {
            if (!ModelState.IsValid)
                return Json(new CustomResponse<ClienteDto> { EsError = true, Mensaje = "Error de validación" });

            var respuesta = await _clientesServicio.ActualizarClienteAsync(clienteDto);
            return Json(respuesta);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCliente(int id)
        {
            var respuesta = await _clientesServicio.EliminarClienteAsync(id);
            return Json(respuesta);
        }
    }
}
