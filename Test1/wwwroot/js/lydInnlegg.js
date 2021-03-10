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