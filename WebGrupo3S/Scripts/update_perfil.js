$(document).ready(function () {
    "use strict";

    var msg = "<h4>Exito!</h4>" +
     "<p>* Datos Actualizados</p>";

    var msg_fail = "<h4>Alerta!</h4>" +
      "<p>* Ha ocurrido un problema</p>";

    var msg_data = "<h4>Alerta!</h4>" +
      "<p>*Los campos no pueden ir vacíos</p>";

    $("#btn-editar").click(function (event) {
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
        console.log(token);

        var perfil = {
            pf_empresa: $("#pf_empresa").val(),
            pf_codPerfil: $("#pf_codPerfil").val(),
            pf_timestamp: $("#pf_timestamp").val(),
            pf_nomPerfil: $("#nombre").val(),
            pf_descPerfil: $("#descripcion").val(),
            __RequestVerificationToken: token
        }
                
        $.ajax({
            url: './updatePerfil',
            type: "POST",
            dataType: 'json',
            data: perfil
        })
        .done(function (response) {
            var jsonObject = JSON.parse(response);

            if (jsonObject.status == 200) {
                
                document.getElementById('btn-editar').removeAttribute('disabled');
                alertify.closeLogOnClick(true).success(msg);
                $("#modal_create").modal("hide");
                return true;
            }

            if (jsonObject.status == 305) {
                document.getElementById('btn-editar').removeAttribute('disabled');
                alertify.closeLogOnClick(true).error("Ocurrió un problema!!");
                console.log(jsonObject.err);
                return false;
            }

            if (jsonObject.status == 400) {
                document.getElementById('btn-editar').removeAttribute('disabled');
                alertify.closeLogOnClick(true).error("Datos no válidos!");
                console.log(response);
                return false;
            }

        })
        .fail(function (response) {
            document.getElementById('btn-editar').removeAttribute('disabled');
            alertify.closeLogOnClick(true).error(msg_fail);
            console.log(response);
        });
    });
});