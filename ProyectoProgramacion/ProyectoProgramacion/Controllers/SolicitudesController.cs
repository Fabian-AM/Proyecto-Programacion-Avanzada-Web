using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionBLL.Servicios;
using System.Threading.Tasks;

namespace ProyectoProgramacion.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly ISolicitudesServicio _solicitudesServicio;
        private readonly IClienteServicio _clienteServicio; // Necesario para listar clientes en el Create

        public SolicitudesController(ISolicitudesServicio solicitudesServicio, IClienteServicio clienteServicio)
        {
            _solicitudesServicio = solicitudesServicio;
            _clienteServicio = clienteServicio;
        }

        // GET: Solicitudes
        public async Task<IActionResult> Index()
        {
            var respuesta = await _solicitudesServicio.ObtenerTodasSolicitudesAsync();

            if (respuesta.EsError)
            {
                // Si hay error, podrías mandar un mensaje a la vista o loguearlo
                ViewBag.Error = respuesta.Mensaje;
                return View(new List<SolicitudDto>());
            }

            return View(respuesta.Data);
        }

        // GET: Solicitudes/Create
        public async Task<IActionResult> Create()
        {
            await CargarListaClientes();
            return View();
        }

        // POST: Solicitudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SolicitudDto solicitudDto)
        {
            // 1. ELIMINAMOS LAS VALIDACIONES QUE NO CORRESPONDEN AL USUARIO
            // Esto le dice al sistema: "No te preocupes si estos campos vienen vacíos"
            ModelState.Remove("Estado");
            ModelState.Remove("NombreCliente");

            // 2. ASIGNAMOS VALORES POR DEFECTO
            // Asignamos el estado inicial (ej: "Pendiente", "Nueva", o el Id 1 según tu base de datos)
            solicitudDto.Estado = "Pendiente";

            // (Opcional) Si tu base de datos requiere un nombre a fuerza, podrías poner uno temporal,
            // aunque lo ideal es que el servicio lo busque por el ID.
            solicitudDto.NombreCliente = "TBD";

            // 3. AHORA SÍ VERIFICAMOS
            if (ModelState.IsValid)
            {
                var respuesta = await _solicitudesServicio.CrearSolicitudAsync(solicitudDto);

                if (!respuesta.EsError)
                {
                    TempData["MensajeExito"] = "La solicitud se ha creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError("", respuesta.Mensaje);
            }

            // 4. SI FALLA, RECARGAMOS LA LISTA (Muy importante para que no truene la vista)
            await CargarListaClientes();
            return View(solicitudDto);
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
            // Buscamos la solicitud por ID
            var respuesta = await _solicitudesServicio.ObtenerSolicitudPorIdAsync(id);

            if (respuesta.EsError || respuesta.Data == null)
            {
                return NotFound();
            }

            // IMPORTANTE: Cargamos la lista de clientes para el Dropdown
            await CargarListaClientes();

            // Mandamos los datos a la vista para que los campos salgan llenos
            return View(respuesta.Data);
        }

        // 2. POST: Para guardar los cambios
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SolicitudDto solicitudDto)
        {
            // Limpiamos validaciones que no aplican (igual que en Create)
            ModelState.Remove("NombreCliente");

            // Si el estado no viene del form, puedes quitarlo también o asignarlo:
            // ModelState.Remove("Estado"); 

            if (ModelState.IsValid)
            {
                var respuesta = await _solicitudesServicio.ActualizarSolicitudAsync(solicitudDto);

                if (!respuesta.EsError)
                {
                    // Usamos TempData para la alerta bonita que configuramos antes
                    TempData["MensajeExito"] = "La solicitud ha sido actualizada correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                // Si falló el servicio, mostramos el error
                ModelState.AddModelError("", respuesta.Mensaje);
            }

            // Si algo salió mal, recargamos la lista y devolvemos la vista
            await CargarListaClientes();
            return View(solicitudDto);
        }
    }
}