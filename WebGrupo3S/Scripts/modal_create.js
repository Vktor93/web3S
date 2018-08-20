$(document).ready(function () {

    $("#btn-mostrar").click(function () {
    
        $.ajax({
            url: './perfilView',
            type: "POST",
            dataType: 'json',            
        })
        .done(function (response) {
            $(".modal").html(response.PartialView);
            //console.log(response.PartialView);            
        })
        .fail(function (response) {
            console.log(response);
            return false;
        });
        
        $("#modal_create").modal();
    });    

});

//FUNCION PARA CERRAR MODAL
function closeModal() {
    $("#modal_create").modal("hide");
    $("#nombre").val("");
    $("#descripcion").val("");
}


//FUNCION PARA LEVANTAR VISTA DE EDITAR PERFIL
function viewUpdate(link) {
                  
    var row = link.parentNode.parentNode;
    var id = parseInt(row.getElementsByTagName("td")[0].innerHTML);
    //var id = parseInt(link.closest("tr").find("td")[0].innerHTML);
    console.log(id);

    //var perfil = new Object();

    //perfil.pf_nomPerfil = $("#nombre").val();
    //perfil.pf_descPerfil = $("#descripcion").val();

    //console.log(perfil)

    $.ajax({
        url: './updateView',
        type: "POST",
        data: { id: id} ,
        dataType: 'json',
    })
    .done(function (response) {
        $(".modal").html(response.PartialView);
        //console.log(response.PartialView);            
    })
    .fail(function (response) {
        console.log(response);
    });

    $("#modal_create").modal();
}