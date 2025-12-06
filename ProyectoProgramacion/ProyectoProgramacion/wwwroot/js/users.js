function openCreateModal() {
    $.get('/Users/Create', function (html) {
        $('#ModalContainer').html(html);
        $('#MainModal').modal('show');
    });
}

$(document).ready(function () {
    $('#usersTable').DataTable({
        paging: true,
        searching: true,
        ordering: true,
        info: true,
        lengthMenu: [5, 10, 25, 50]
    });
}); // ← ESTA LÍNEA ES LA QUE FALTABA
//----------------------------------------

function openEditModal(id) {
    $.get('/Users/Edit/' + id, function (html) {
        $('#ModalContainer').html(html);
        $('#MainModal').modal('show');
    });
}

function openRolesModal(id) {
    $.get('/Users/Roles/' + id, function (html) {
        $('#ModalContainer').html(html);
        $('#MainModal').modal('show');
    });
}

function openDetailsModal(id) {
    $.get('/Users/Details/' + id, function (html) {
        $('#ModalContainer').html(html);
        $('#MainModal').modal('show');
    });
}


function deleteUser(id) {
    Swal.fire({
        title: '¿Eliminar usuario?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Sí, eliminar'
    }).then(result => {
        if (result.isConfirmed) {
            $.post('/Users/Delete', { id }, function (res) {
                if (res.success) {
                    Swal.fire('Eliminado', res.message, 'success')
                        .then(() => location.reload());
                } else {
                    Swal.fire('Error', res.errors.join(','), 'error');
                }
            });
        }
    });
}
