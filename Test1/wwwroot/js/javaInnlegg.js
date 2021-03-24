// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
window.onload = function () {
    var small = { height: "50%", width: "50%"};
    var large = { height: "100%", width: "100%" };
    var count = 1;

    $("#innlegg").css(small).on('click', function () {
    $(this).animate((count == 1) ? large : small);
        count = 1 - count;
  });

    var htmlElements = "";
    for (var i = 0; i < 5; i++) {
        htmlElements += '<div class="box"> <p> Hei </p> </div>';
    }
    var container = document.getElementById("alleKommentarFelt");
    container.innerHTML = htmlElements;
}    
/*
window.onload = function () {
    var htmlElements = "";
    for (var i = 0; i < 5; i++) {
        htmlElements += '<div class="box"> <p> Hei </p> </div>';
    }
    var container = document.getElementById("alleKommentarFelt");
    container.innerHTML = htmlElements;
}*/


function Kommentar(nyId) {
    var url = '@Html.Raw(Url.Action("NyttKommentar", "Innlegg" , new { tekst = "_tekst_", innleggId = "_innleggId_" }))';
    var tekst = $('#kommentarFelt').val();

    var params = url.replace('_tekst_', tekst).replace('_innleggId_', nyId);
    window.location.href = params;
}

function spillAv(sang, toggle) {
    var audio = document.getElementsByClassName('lydspor'),
        i = audio.length;
    var playpause = document.getElementsByClassName("play_button");
    var lyd = document.getElementById(sang);
    var loggle = document.getElementById(toggle);

    if (lyd.paused) {
        while (i--) {
            audio[i].pause();
            playpause[i].src = "https://www.clipartmax.com/png/middle/134-1347519_youtube-png-play-pictures-png-images-play-button-png.png"
        };
        loggle.src = "https://png.pngtree.com/png-vector/20190120/ourmid/pngtree-pause-vector-icon-png-image_470715.jpg"
        lyd.play();
    }
    else {
        loggle.src = "https://www.clipartmax.com/png/middle/134-1347519_youtube-png-play-pictures-png-images-play-button-png.png"
        lyd.pause();
    }
};

