$(document).ready(() => {
    var heightTotal = $(document).height();
    var heightNavbar = $("#navBarID").height();
    $(".mainBody").height(heightTotal - heightNavbar - 4);

    listenerBtn();
});

var dataTypeJson = 'application/json; charset=utf-8';
var diagNotificacion;

var idioma_espanol = {
    "sProcessing": "Procesando...",
    "sLengthMenu": "Mostrar _MENU_ registros",
    "sZeroRecords": "No se encontraron resultados",
    "sEmptyTable": "Ningún dato disponible en esta tabla",
    "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
    "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
    "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
    "sInfoPostFix": "",
    "sSearch": "Buscar:",
    "sUrl": "",
    "sInfoThousands": ",",
    "sLoadingRecords": "Cargando...",
    "oPaginate": {
        "sFirst": "Primero",
        "sLast": "Ãšltimo",
        "sNext": "Siguiente",
        "sPrevious": "Anterior"
    },
    "oAria": {
        "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
        "sSortDescending": ": Activar para ordenar la columna de manera descendente"
    }
}


var showLoadingDiv = function () {
    $("#loading-div-background").show();
}

var hideLoadingDiv = function () {
    $("#loading-div-background").hide();
}

var initModalNotificar = (text) => {

    diagNotificacion = $("#dialog-notificacion").dialog({
        show: { effect: "fade", speed: '7000' },
        resizable: false,
        height: "200",
        width: "60%",
        modal: true,
        create: function (event, ui) {
            $("#response").text(text);
        },
        open: function (event, ui) {

            $('.ui-widget-overlay').css({
                opacity: '.9',
                background: 'black'
            });

        },
        close: function (event, ui) {
            $(this).dialog('destroy');
        },
        hide: { effect: "fade", speed: '7000' }
    });
};

var listenerBtn = () => {
    $("#closeNotificacion").click((e) => {
        diagNotificacion.dialog("close");
    });
};