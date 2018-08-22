$(document).ready(function () {
    "use strict";

    var msg = "<h4>Exito!</h4>" +
     "<p>* Se dió de baja el Perfil.</p>";

    var msg_fail = "<h4>Alerta!</h4>" +
      "<p>* Ha ocurrido un problema,</p>" +
        "<p>* ver salida en consola.</p>";

    var msg_data = "<h4>Alerta!</h4>" +
      "<p>*Los campos no pueden ir vacíos.</p>";

    $("#btn-baja").click(function (event) {
        event.preventDefault;

        if ($("#nombre").val() == "" || $("#descripcion").val() == "") {
            alertify.closeLogOnClick(true).error(msg_data);
            return false;
        }

        $(this).attr('disabled', true);
        /*
        var perfil = new Object();
        perfil.pf_empresa = $("#pf_empresa").val();
        perfil.pf_codPerfil = $("#pf_codPerfil").val();
        perfil.pf_timestamp = $("#pf_timestamp").val();
        perfil.pf_nomPerfil = $("#nombre").val();
        perfil.pf_descPerfil = $("#descripcion").val();*/



        var token = $('[name=__RequestVerificationToken]').val();
        //console.log(token);
                      
        var perfil = {
            id: parseInt($("#pf_codPerfil").val()),
            __RequestVerificationToken: token
        }
        

        $.ajax({
            url: './deletePerfil',
            type: "POST",            
            dataType: 'json',
            data: perfil
        })
        .done(function (response) {
            var jsonObject = JSON.parse(response);

            if (jsonObject.status == 200) {

                document.getElementById('btn-baja').removeAttribute('disabled');
                alertify.closeLogOnClick(true).success(msg);
                $("#modal_create").modal("hide");
                return true;
            }

            if (jsonObject.status == 305) {
                document.getElementById('btn-baja').removeAttribute('disabled');
                alertify.closeLogOnClick(true).error(jsonObject.message);
                console.log(jsonObject.err);
                return false;
            }

            if (jsonObject.status == 400) {
                document.getElementById('btn-baja').removeAttribute('disabled');
                alertify.closeLogOnClick(true).error("Datos no válidos!");
                console.log(response);
                return false;
            }

        })
        .fail(function (response) {
            document.getElementById('btn-baja').removeAttribute('disabled');
            alertify.closeLogOnClick(true).error(msg_fail);
            console.log(response);
        });
    });
});