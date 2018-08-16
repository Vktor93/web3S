$(document).ready(function () {

    var msg = "<h4>Exito!</h4>" +
      "<p>* Datos Guardados</p>";

    var msg_fail = "<h4>Alerta!</h4>" +
      "<p>* Ha ocurrido un problema</p>";

    var msg_data = "<h4>Alerta!</h4>" +
      "<p>* Llenar los campos</p>";

    $("#btn-crear").click(function () {

        if ($("#nombre").val() == "" || $("#descripcion").val() == "") {
            alertify.closeLogOnClick(true).error(msg_data);
            return false;
        }

        $(this).attr('disabled', true);

        var perfil = new Object();
        perfil.pf_nomPerfil = $("#nombre").val();
        perfil.pf_descPerfil = $("#descripcion").val();

        $.ajax({
            url: './SavePerfil',
            type: "POST",
            dataType: 'json',
            data: perfil
        })
        .done(function (response) {
            jsonObject = JSON.parse(response);

            if ( jsonObject.status == 200 ) {
                $("#nombre").val("");
                $("#descripcion").val("");
                document.getElementById('btn-crear').removeAttribute('disabled');
                alertify.closeLogOnClick(true).success(msg);
                return true;
            }

            if ( jsonObject.status == 305 ) {
                document.getElementById('btn-crear').removeAttribute('disabled');
                alertify.closeLogOnClick(true).error("Ocurrió un problema!!");
                console.log(jsonObject.err);
                return false;
            }

            if (jsonObject.status == 400) {
                document.getElementById('btn-crear').removeAttribute('disabled');
                alertify.closeLogOnClick(true).error("Datos no válidos!");
                console.log(response);
                return false;
            }

        })
        .fail(function (response) {
            document.getElementById('btn-crear').removeAttribute('disabled');
            alertify.closeLogOnClick(true).error(msg_fail);
            console.log(response);
        });
    });

});