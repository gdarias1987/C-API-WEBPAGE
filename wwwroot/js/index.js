$(document).ready(() => {
    //alert("INDEX");
    loadDatabase();
    loadTablaPendiente();
    listener();
});

var listener = () => {
    $("#newCheckpoint").inputFilter(function (value) {
        return /^\d*$/.test(value);
    });

    $("#btnCrearCheckpoint").click((e) => {
        var idEvento = $("#newCheckpoint").val();

        if (idEvento != "") {
            $.ajax({
                "url": "api/checkpoint/" + idEvento,
                "type": "GET",
                "beforeSend": function () { showLoadingDiv(); },
                "complete": function (resp) {
                    hideLoadingDiv();
                    // Codigo 404 - No existe
                    if (resp.status == 404) {
                        $.ajax({
                            "url": "api/checkpoint/" + idEvento,
                            "type": "POST",
                            "beforeSend": function () { showLoadingDiv(); },
                            "complete": function (resp) {
                                $("#newCheckpoint").val("");
                                $("#tablaCheckpoint").DataTable().ajax.reload();
                            }
                        });
                    }
                    else {
                        initModalNotificar("El ID del evento ingresado ya existe");
                    }
                }
            });
        }
    });

    $("#enviarPendientes").click((e) => {
        if (listadoPendientes.length < 1) {
            initModalNotificar("No hay eventos pendientes");
        }
        else {
            $.ajax({
                "url": "api/checkpoint/",
                "type": "PUT",
                "contentType": dataTypeJson,
                "dataType": "json",
                "data": JSON.stringify(listadoPendientes),
                "beforeSend": function () { showLoadingDiv(); },
                "complete": function (resp) {
                    listadoPendientes = [];
                    loadTablaPendiente();
                    loadDatabase();

                }
            });
        }
    });

    $("#selectEstado").change((e) => {
        var seleccion = $("#selectEstado").val();

        $("#selectSubestado").find('option').remove().end();

        switch (seleccion) {
            case "Handling":
                $('#selectSubestado').append(`<option value="Null" selected>Null</option>`); 
                $('#selectSubestado').append(`<option value="Manufacturing">Manufacturing</option>`); 
                break;
            case "Ready To Ship":
                $('#selectSubestado').append(`<option value="Ready To Print" selected>Ready To Print</option>`);
                $('#selectSubestado').append(`<option value="Printed">Printed</option>`);
                break;
            case "Shipped":
                $('#selectSubestado').append(`<option value="Null" selected>Null</option>`);
                $('#selectSubestado').append(`<option value="Soon Deliver">Soon Deliver</option>`);
                $('#selectSubestado').append(`<option value="Waiting For Withdrawal">Waiting For Withdrawal</option>`);
                break;
            case "Delivered":
                $('#selectSubestado').append(`<option value="Null" selected>Null</option>`);
                break;
            case "Not Delivered":
                $('#selectSubestado').append(`<option value="Lost" selected>Lost</option>`);
                $('#selectSubestado').append(`<option value="Stolen">Stolen</option>`);
                break;
        }

    });

    $("#addPendiente").click((e) => {
        var data = tablaEventos.row($(tablaEventos.$('tr.selected'))).data();
        if (data == undefined) {
            initModalNotificar("Se debe seleccionar un ID de evento");
        }
        else {
            var estado = $("#selectEstado").val();
            var subestado = $("#selectSubestado").val();
            var pendiente = {
                "idEvento": data.idEvento,
                "estado": estado,
                "subestado": subestado
            };

            listadoPendientes.push(pendiente);
            loadTablaPendiente();
        }
    });
};

var tablaCheckpoint, tablaEventos, tablaPendientes;
var listadoCheckpoints,listadoPendientes = [];

var loadDatabase = () => {
    var tablaHeight = $(".databasePanel").height() - 80;

    tablaCheckpoint = $("#tablaCheckpoint").DataTable({
        "dom": "tp",
        "destroy": true,
        "paging": false,
        "scrollY": tablaHeight,
        "ajax": {
            "destroy": "true",
            "url": "api/checkpoint",
            "type": "GET",
            "dataSrc": function (data) {

                listadoCheckpoints = data;

                return data;
            },
            'beforeSend': function (request) {
                showLoadingDiv();
            },
            'complete': function (resp) {
                hideLoadingDiv();
                loadSecondTable();
            }
        },
        "language": idioma_espanol,
        "columns": [
            {
                "visible": true,
                "render": function (data, type, full, meta) {
                    return full.idEvento;
                }
            },
            {
                "render": function (data, type, full, meta) {
                    return full.estado;
                }
            },
            {
                "render": function (data, type, full, meta) {
                    return full.subestado == null ? "null" : full.subestado;
                }
            }
        ]
    });
};

var loadSecondTable = () => {
    
    var tablaHeight = $(".flexRow1").height() - 80;

    tablaEventos = $("#tablaEventos").DataTable({
        "dom": "tp",
        "destroy": true,
        "paging": false,
        "scrollY": tablaHeight,
        "data": listadoCheckpoints,
        "language": idioma_espanol,
        "columns": [
            {
                "visible": true,
                "render": function (data, type, full, meta) {
                    return full.idEvento;
                }
            }
        ]
    });

    $('#tablaEventos tbody').off("click");


    $('#tablaEventos tbody').on('click', 'tr', function () {
        if ($(this).hasClass('selected')) {
            $(this).removeClass('selected');
        }
        else {
            tablaEventos.$('tr.selected').removeClass('selected');
            $(this).addClass('selected');
        }
    });
};

var loadTablaPendiente = () => {
    var tablaHeight = $(".flexRow2").height() - 80 - $("#enviarPendientes").height();

    tablaPendientes = $("#tablaPendientes").DataTable({
        "dom": "tp",
        "destroy": true,
        "paging": false,
        "scrollY": tablaHeight,
        "data": listadoPendientes,
        "language": idioma_espanol,
        "columns": [
            {
                "visible": true,
                "render": function (data, type, full, meta) {
                    return full.idEvento;
                }
            },
            {
                "render": function (data, type, full, meta) {
                    return full.estado;
                }
            },
            {
                "render": function (data, type, full, meta) {
                    return full.subestado == null ? "null" : full.subestado;
                }
            }
        ]
    });

};



(function ($) {
    $.fn.inputFilter = function (inputFilter) {
        return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
            if (inputFilter(this.value)) {
                this.oldValue = this.value;
                this.oldSelectionStart = this.selectionStart;
                this.oldSelectionEnd = this.selectionEnd;
            } else if (this.hasOwnProperty("oldValue")) {
                this.value = this.oldValue;
                this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
            } else {
                this.value = "";
            }
        });
    };
}(jQuery));
