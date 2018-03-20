/** add active class and stay opened when selected */
var url = window.location;

// for sidebar menu entirely but not cover treeview
$('ul.sidebar-menu a').filter(function () {
    return this.href == url;
}).parent().addClass('active');

// for treeview
$('ul.treeview-menu a').filter(function () {
    return this.href == url;
}).parentsUntil(".sidebar-menu > .treeview-menu").addClass('active');

$(document).ready(function () {
    $('[data-toggle="tooltip"]').tooltip();

    $(".cli").on('click', function () {

        /** add active class and stay opened when selected */
        var url = window.location.pathname;

        /*vk - capture and compare url and tag <a> attribute href to set
        background color on selected option menu*/
        $(this).find(".lintern").each(function () {

            //console.log($(this).attr("href"));

            if ($(this).attr("href") == url) {
                $(this).addClass('high');
            } else {
                $(this).removeClass('high');
            }
        });
    });
});