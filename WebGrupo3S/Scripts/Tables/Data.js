$(document).ready(function () {
    var date = new Date();
    var val = date.getDate() + "-" + (date.getMonth() + 1) + "-" + date.getFullYear();

    $('#datos').DataTable({

        dom: 'Bfrtip',
      
        buttons: [
            {
                extend: 'pdfHtml5',
                title: 'Productos_' + val,
                text: 'PDF'
            },

            {
                extend: 'excelHtml5',
                title: 'Productos_' + val,
                text: 'Excel'
            }
        ],

        "language": {
            "search": "Filtrar"
        }
    });

});

    

