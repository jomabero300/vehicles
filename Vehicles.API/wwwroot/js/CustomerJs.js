let datatable;

$(document).ready(function () {
    loadDataTable();
    let id = document.getElementById("customerID");
    if (id.value > 0) {
        $('#myModal').modal('show');
    }
});

function limpiar() {
    $('#customerID').val(0);
    $('#firstName').val('');
    $('#lastName').val('');
    $('#address').val('');
    $('#phoneNumber').val('');
    $('#estate').val('true');

}

function loadDataTable() {
    datatable = $('#tblData').DataTable({
        "ajax": {
            "url":"/Customers/ObtenerTodos"
        },
        "columns": [
            { "data": "firstName", "width": "24%" },
            { "data": "lastName", "width": "24%" },
            { "data": "address", "width": "22%" },
            { "data": "phoneNumber", "width": "10%" },
            {
                "data": "estate",
                "render": function (data) {
                    if (data == true) {
                        return "Activo";
                    } else {
                        return "Inactivo";
                    }
                },
                "width": "10%",
            },
            {
                "data": "customerID",
                "render": function (data) {
                    return `
                            <div>
                            <a href="/Customers/Index/${data}" class="btn btn-sm btn-success text-white" style="cursor:pointer;"><i class="bi bi-pencil-fill" style="color: White;"></i></a> |
                            <a onclick=Delete("/Customers/Delete/${data}") class="btn btn-sm btn-danger text-white" style="cursor:pointer;"><i class="bi bi-trash2-fill"></i></a>
                            </div>
                            `;
                }, "width":"10%"
            }
        ]
    });
}

function Delete(url) {

    swal({
        title: "¿Esta realmente seguro de eliminar este clienete?",
        text: "Este registro no se podrá recuperar",
        icon: "warning",
        showCancelButton: true,
        closeOnConfirm: false,
        confirmButtonText: "Si, borra esto!",
        confirmButtonCollor:"#ec6c62",
        buttons: true
    }).then((borrar) => {
        if (borrar) {
            $.ajax({
                type: "DELETE",
                url: url,
                success: function (data) {
                    if (data.success) {
                        alert(data.message);
                        datatable.ajax.reload();
                    } else {
                        alert(data.message);
                    }
                }
            });
        }
    });
}