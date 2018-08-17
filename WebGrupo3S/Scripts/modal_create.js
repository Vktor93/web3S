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
        });
        
        $("#modal_create").modal();
    });


    $("#datos .update").click(function () {
        var id = $(this).closest("tr").find("td").eq(0).html();
        console.log(id);


        $.ajax()
    });

});

function closeModal() {
    $("#modal_create").modal("hide");
}

function editProfile() {
    $("#btn-editar").click(function () {

    });
}