// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/*
window.onload = function () {
    var htmlElements = "";
    for (var i = 0; i < 5; i++) {
        htmlElements += '<div class="box"> <p> Hei </p> </div>';
    }
    var container = document.getElementById("alleKommentarFelt");
    container.innerHTML = htmlElements;
}*/
window.onload = function () {
    var small = { height: "50%", width: "50%" };
    var large = { height: "100%", width: "100%" };
    var count = 1;

    $("#innlegg").css(small).on('click', function () {
        $(this).animate((count == 1) ? large : small);
        count = 1 - count;
    });
}
