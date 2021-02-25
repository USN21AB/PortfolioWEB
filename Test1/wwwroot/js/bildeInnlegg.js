// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.onload = function () {
    var small = { width: "25%", height: "25%" };
    var large = { width: "50%", height: "50%" };
    var count = 1;

    $("#innlegg").css(small).on('click', function () {
        $(this).animate((count == 1) ? large : small);
        count = 1 - count;
    });
}