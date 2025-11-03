(() => {
    const Clientes = {
        tabla: null,

        init() {
            this.inicializarTabla();
            this.registrarEventos();
        },

        inicializarTabla() {
            this.tabla = $('#tablaClientes').DataTable({
                ajax: {
                    url: '/Cliente/ObtenerClientes',
                    type: 'GET',
                    dataSrc: function (json) {
                        return json && json.data ? json.data : [];
                    }
                },
                columns: [
                    { data: 'id', title: 'ID' },
                    { data: 'nombre', title: 'Nombre' },
                    { data: 'apellido', title: 'Apellido' },
                    { data: 'identificacion', title: 'Identificación' },
                    { data: 'edad', title: 'Edad' },
                    {
                        data: null,
                        title: 'Acciones',
                        orderable: false,
                        render: function (data, type, row) {
                            return `
                <button class="btn btn-sm btn-primary editar" data-id="${row.id}">Editar</button>
                <button class="btn btn-sm btn-danger eliminar" data-id="${row.id}">Eliminar</button>
              `;
                        }
                    }
                ],
                responsive: true,
                processing: true,
                pageLength: 10
            });
        },

        registrarEventos() {
            $('#tablaClientes').on('click', '.editar', function () {
                const id = $(this).data('id');
                Clientes.CargarDatosCliente(id);
            });

            $('#tablaClientes').on('click', '.eliminar', function () {
                const id = $(this).data('id');
                Clientes.EliminarCliente(id);
            });

            
            $('#btnGuardarCambios').on('click', function () {
                Clientes.GuardarCliente();
            });

         
            $('#btnEditarCambios').on('click', function () {
                Clientes.EditarCliente();
            });
        },

        GuardarCliente() {
            const form = $('#formCrearCliente');
             if (!form.valid()) return;  

            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: form.serialize(),
                success: function (response) {
                    if (!response.esError) {
                        $('#modalCrearCliente').modal('hide');
                        Clientes.tabla.ajax.reload();
                        form[0].reset();
                        Swal.fire({ title: 'Éxito', text: response.mensaje || 'Cliente creado', icon: 'success' });
                    } else {
                        Swal.fire({ title: 'Error', text: response.mensaje || 'No se pudo crear', icon: 'error' });
                    }
                },
                error: function () {
                    Swal.fire({ title: 'Error', text: 'Error en la solicitud al servidor', icon: 'error' });
                }
            });
        },


        CargarDatosCliente(id) {
            $.get(`/Cliente/ObtenerClientePorId/${id}`, function (response) {
                if (!response.esError && response.data) {
                    const c = response.data;
                    $('#ClienteId').val(c.id);
                    $('#Nombre').val(c.nombre);
                    $('#Apellido').val(c.apellido);
                    $('#Identificacion').val(c.identificacion);
                    $('#Edad').val(c.edad);
                    $('#modalEditarCliente').modal('show');
                } else {
                    Swal.fire({ title: 'Error', text: response.mensaje || 'No se pudo cargar', icon: 'error' });
                }
            }).fail(() => {
                Swal.fire({ title: 'Error', text: 'No se pudo conectar', icon: 'error' });
            });
        },

        EditarCliente() {
            const form = $('#formEditarCliente');
             if (!form.valid()) return;   

            $.ajax({
                url: form.attr('action'),
                type: 'POST',
                data: form.serialize(),
                success: function (response) {
                    if (!response.esError) {
                        $('#modalEditarCliente').modal('hide');
                        Clientes.tabla.ajax.reload();
                        Swal.fire({ title: 'Éxito', text: response.mensaje || 'Cliente actualizado', icon: 'success' });
                    } else {
                        Swal.fire({ title: 'Error', text: response.mensaje || 'No se pudo actualizar', icon: 'error' });
                    }
                },
                error: function () {
                    Swal.fire({ title: 'Error', text: 'Error en la solicitud al servidor', icon: 'error' });
                }
            });
        },


        EliminarCliente(id) {
            Swal.fire({
                title: "¿Estás seguro?",
                text: "No podrás revertir esta acción",
                icon: "warning",
                showCancelButton: true,
                confirmButtonText: "Sí, borrar"
            }).then((result) => {
                if (result.isConfirmed) {
                    $.ajax({
                        url: '/Cliente/EliminarCliente',
                        type: 'POST',
                        data: { id: id },
                        success: function (response) {
                            if (!response.esError) {
                                Clientes.tabla.ajax.reload();
                                Swal.fire({ title: 'Éxito', text: response.mensaje || 'Cliente eliminado', icon: 'success' });
                            } else {
                                Swal.fire({ title: 'Error', text: response.mensaje || 'No se pudo eliminar', icon: 'error' });
                            }
                        },
                        error: function () {
                            Swal.fire({ title: 'Error', text: 'Error en la solicitud al servidor', icon: 'error' });
                        }
                    });
                }
            });
        }
    };

    $(document).ready(() => Clientes.init());
})();
