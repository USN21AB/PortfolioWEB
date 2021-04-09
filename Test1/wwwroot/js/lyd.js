/*window.onload = initial();

    function initial() {
        var audio = document.getElementsByClassName('lydspor'),
            i = audio.length;
        while (i--) {
            var lyd = audio[i].id;
            var luld = document.getElementById(lyd);
            luld.volume = 0.5;
        }
    };
    */

function spillAv(sang, toggle) {
    var audio = document.getElementsByClassName('lydspor'),
        i = audio.length;
    var playpause = document.getElementsByClassName("play_button");
    var lyd = document.getElementById(sang);
    var loggle = document.getElementById(toggle);
    var spiller = document.getElementById("test").style;



    if (lyd.paused) {
        while (i--) {
            audio[i].pause();
            playpause[i].src = "/resources/icons8-play-96.png"
        };
        spiller.transition = "all .5s ease";
        spiller.bottom = "0";
        loggle.src = "/resources/icons8-pause-96.png"
        lyd.play();
    }
    else {
        spiller.transition = "all .5s ease";
        spiller.bottom = "-70px";
        loggle.src = "/resources/icons8-play-96.png"
        lyd.pause();
    }


};

function volum(verdi) {


    var audio = document.getElementsByClassName('lydspor'),
        i = audio.length;


    while (i--) {
        var lyd = audio[i].id;
        var luld = document.getElementById(lyd);
        luld.volume = verdi / 10;
    };

}

function visSang(sang, artist) {
    var sang = document.getElementById(sang).textContent;
    var artist = document.getElementById(artist).textContent;
    var sangfelt = document.getElementById("sangfelt");
    var artistfelt = document.getElementById("artistfelt");

    sangfelt.innerHTML = sang + " - ";
    artistfelt.innerHTML = artist;
}



function timer(lydId) {
    var lyd = document.getElementById(lydId);
    lyd = document.getElementById("seekbar").value = (lyd.currentTime * 100) / lyd.duration;

    /*var time = lyd.currentTime;
    var minutes = Math.floor(time / 60);
    var seconds = Math.floor(time);

    minutes = parseFloat(minutes);
    seconds = parseFloat(seconds);*/

    var tid = lyd.currentTime;
    var lengde = lyd.duration;



    //document.getElementById('tracktime').innerHTML = Math.floor(lyd.currentTime) + ' / ' + Math.floor(lyd.duration);
    // + " / " +

}


function stopAll() {
    var lyd = document.getElementsByClassName('lydspor');
    var spiller = document.getElementById("test").style;
    var playpause = document.getElementsByClassName("play_button");
    i = lyd.length;
    

    while (i--) {

        lyd[i].pause();
        playpause[i].src = "/resources/icons8-play-96.png"
    }

    spiller.transition = "all .5s ease";
    spiller.bottom = "-70px";
};

var info = document.getElementsByClassName("awesome-info");
var image = document.getElementsByClassName("single-awesome-portfolio");




/*$(function () {
    $('.awesome-portfolio-content').mixItUp({
        animation: {
            effects: 'rotateZ',
            duration: 1000,
        }
    });

    $('.blog-column-content').mixItUp({
        animation: {
            effects: 'scale',
            duration: 1000,
        }
    });

    $('.portfolio-column-content').mixItUp({
        animation: {
            effects: 'fade rotateY(-180deg)',
            duration: 1000,
        }
    });
});
*/

/*$(function timer2(sanger) {
    var lol = "#" + sanger;
    $(lol).on('timeupdate', function () {
        $('#seekbar').attr("value", (this.currentTime * 100) / this.duration);
    });
})*/

/*$(document).ready(function () {
    var hash = window.location.hash;

    if (hash != "")
        $('#v-pills-tab a[href="' + hash + '"]').tab('show');
    else
        $('#v-pills-tab a:second').tab('show');
});
*/