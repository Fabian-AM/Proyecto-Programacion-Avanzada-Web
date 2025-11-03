using AutoMapper;
using ProyectoProgramacionBLL.Dtos;
using ProyectoProgramacionDAL.Entidades;
using ProyectoProgramacionDAL.Repositorios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoProgramacionBLL.Servicios
{
    public class ClienteServicio : IClienteServicio
    {
        private readonly IClientesRepositorio _clientesRepositorio;
        private readonly IMapper _mapper;
        public ClienteServicio(IClientesRepositorio clientesRepositorio, IMapper mapper)
        {
            _clientesRepositorio = clientesRepositorio;
            _mapper = mapper;
        }

        public async Task<CustomResponse<ClienteDto>> ActualizarClienteAsync(ClienteDto clienteDto)
        {
            var respuesta = new CustomResponse<ClienteDto>();

            
            if (clienteDto.Edad < 18)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pueden registrar clientes menores de edad.";
                return respuesta;
            }

            var clientesExistentes = await _clientesRepositorio.ObtenerClienteAsync();
            if (clientesExistentes.Any(c => c.Identificacion == clienteDto.Identificacion && c.Id != clienteDto.Id))
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "Ya existe otro cliente con esa identificación.";
                return respuesta;
            }

            var cliente = _mapper.Map<Cliente>(clienteDto);
            if (!await _clientesRepositorio.ActualizarClienteAsync(cliente))
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pudo actualizar el cliente.";
                return respuesta;
            }

            respuesta.EsError = false;
            respuesta.Mensaje = "Cliente actualizado correctamente.";
            return respuesta;
        }

        public async Task<CustomResponse<ClienteDto>> EliminarClienteAsync(int id)
        {
            var respuesta = new CustomResponse<ClienteDto>();

            if (!await _clientesRepositorio.EliminarClienteAsync(id))
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pudo eliminar el usuario";
                return respuesta;
            }

            return respuesta;

        }

        public async Task<CustomResponse<ClienteDto>> AgregarClienteAsync(ClienteDto clienteDto)
        {
            var respuesta = new CustomResponse<ClienteDto>();

           
            if (clienteDto.Edad < 18)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pueden registrar clientes menores de edad.";
                return respuesta;
            }

            
            var clientesExistentes = await _clientesRepositorio.ObtenerClienteAsync();
            if (clientesExistentes.Any(c => c.Identificacion == clienteDto.Identificacion))
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "Ya existe un cliente con esa identificación.";
                return respuesta;
            }

            var agregado = await _clientesRepositorio.AgregarClienteAsync(_mapper.Map<Cliente>(clienteDto));
            if (!agregado)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "No se pudo agregar el cliente.";
                return respuesta;
            }

            respuesta.EsError = false;
            respuesta.Mensaje = "Cliente agregado correctamente.";
            respuesta.Data = clienteDto;

            return respuesta;
        }


        public async Task<CustomResponse<ClienteDto>> ObtenerClientePorIdAsync(int id)
        {
            var respuesta = new CustomResponse<ClienteDto>();

            var cliente = await _clientesRepositorio.ObtenerClientePorIdAsync(id);

            var validaciones = validar(cliente);
            if (validaciones.EsError)
            {
                return validaciones;
            }

            respuesta.Data = _mapper.Map<ClienteDto>(cliente);
            return respuesta;

        }
        public async Task<CustomResponse<List<ClienteDto>>> ObtenerClientesAsync()
        {
            var respuesta = new CustomResponse<List<ClienteDto>>();
            var Clientes = await _clientesRepositorio.ObtenerClienteAsync();
            var clienteDto = _mapper.Map<List<ClienteDto>>(Clientes);
            respuesta.Data = clienteDto;
            return respuesta;
        }
        private CustomResponse<ClienteDto> validar(Cliente cliente)
        {
            var respuesta = new CustomResponse<ClienteDto>();
            if (cliente == null)
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "Usuario no encontrado";
                return respuesta;
                
            }
            if (cliente.Edad < 18) //Falla de negocio
            {
                respuesta.EsError = true;
                respuesta.Mensaje = "Usuario menor de edad";
                return respuesta;
                
            }

            return respuesta;
        }
    }
}
