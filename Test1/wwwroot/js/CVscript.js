

$(document).ready(function () {

    function addSlider() {
        var list = document.getElementById("skillsEdit").lastElementChild;
        var iden = "";
        var nyId = parseInt(list.id.substring(5, list.id.length))+2;

        //alert(list.id + " blir til " + nyId)
        var skill = "skill" + nyId;
        var bar = "skillbar" + nyId;
        var tekst = "sliderTekst" + nyId;
        var slider = "skillSlider" + nyId;
       // alert(skill + " " + bar + " " + tekst + " " + slider);
        var tag = "<div class='col-md-6' id='"+ skill +"'>" +
                         " <div class='form-group'>" +
                                   "<input placeholder='write a skill...' id='" + bar + "' />" +
                                          " <div class='input-group'>" +
                                                  "<input class='slider-general' id='" + slider + "' data-slider-id='ex1Slider' value='50'" +
                                                     " type='range' data-slider-min='0' data-slider-max='20' data-slider-step='1' data-slider-value='14' " +
                                                    "oninput='sliderFunc('" + tekst + "','" + slider + "')' />" +
                                                     "<p id='" + tekst + "'>50%</p>" +
                                          "</div>" +
                             "</div>" +
                   "</div>";

       // var tag = "<p>hello</p>";
       var element = document.getElementById("skillsEdit");
        //element.appendChild(tag);
        element.insertAdjacentHTML('beforeend', tag);

    }
    function sliderFunc(id, slider) {
       // alert(id + " " + slider);
        var x1 = document.getElementById(slider).value;
        x2 = document.getElementById(id);
        x2.innerHTML = x1 + '%';
    }
    function addEdit(id, buttonID, buttonIDRed) {
        document.getElementById(id).classList.add("editClass");
        document.getElementById(buttonID).style.display = "inline";
        document.getElementById(buttonIDRed).style.display = "inline";
        document.getElementById(submit).style.display = "inline";
    }
    function removeEdit(id, buttonID, buttonIDRed) {
        document.getElementById(id).classList.remove("editClass");
        document.getElementById(buttonID).style.display = "none";
        document.getElementById(buttonIDRed).style.display = "none";
        document.getElementById(submit).style.display = "none";
    }
    function onDelete(id, index, felt) {
        document.getElementById(id).style.display = "none";
        var json = {
            "felt": felt,
            "index": index,
        };
          $.ajax({
                    type: "POST",
                    url: '@Url.Action("DeleteCV", "ProfilSide")',
                    data: json,
                    dataType: "json",
                    success: function (result) {
                        alert("Dette er resultatet " + result.result);
                    }
                });
    }
    function onEdit(type, index, årDatDiv, årFra, årTil,tittel, bedrift, bio, submitKnapp) {
        document.getElementById(submitKnapp).style.display = "inline";
        var tittVerdi = document.getElementById(tittel).innerHTML;
        var bedVerdi = document.getElementById(bedrift).innerHTML;
        document.getElementById(tittel).outerHTML = "</br><input id=" +tittel+" value='" + tittVerdi+ "' class='mt-0 mb-1'/></br > ";
document.getElementById(bedrift).outerHTML = "<input id=" + bedrift + " value='" + bedVerdi + "' /></br>";
document.getElementById(årDatDiv).outerHTML = "<input id='" + (årFra + index) + "' value='" + årFra + "' /> - <input id='" + (årTil + index) + "' value='" + (årTil + index) + "' /></br>";
if (type == "ArbeidsErfaring") {
    var biVerdi = document.getElementById(bio).innerHTML;
    document.getElementById(bio).outerHTML = "<textarea id=" + bio + " class='text-muted mt-2' rows='3' cols='50'>" + biVerdi + "</textarea>";
}
    }
function saveEdit(bruker, felt, index, årFra, årTil, tittel, bedrift, bio) {
    // alert("first");
    var fra = document.getElementById(årFra);
    var til = document.getElementById(årTil);
    var bion = '';
    if (felt == "ArbeidsErfaring")
        bion = bio.value;
    //alert("hmm " + bion);
    var json = {
        "bruker": bruker,
        "felt": felt,
        "index": index,
        "årFra": fra.value,
        "årTil": til.value,
        "tittel": tittel.value,
        "bedrift": bedrift.value,
        "bio": bion
    };
    $.ajax({
        type: "POST",
        url: '@Url.Action("UpdateCV", "ProfilSide")',
        data: json,
        dataType: "json",
        success: function (result) {
            if (result.status == "ok")
                window.location.reload();
            //else POPUP ERROR
        }
    });
}



            document.getElementById("submitArbeid").addEventListener('click', function (event) {
                event.preventDefault();
                var felt = "Arbeidserfaring";
                var startÅr = $('#startYearA').val();
                var sluttÅr = $('#endYearA').val();
                var jobbTittel = $('#jobTitleA').val();
                var bedriftNavn = $('#companyNameA').val();
                var jobbBeskrivelse = $('#userbio').val();
                var jsonJobb = {
                    "felt": felt,
                    "par1": startÅr,
                    "par2": sluttÅr,
                    "par3": jobbTittel,
                    "par4": bedriftNavn,
                    "par5": jobbBeskrivelse
                };
                ajaxUpload(jsonJobb);
            });
            document.getElementById("submitSkole").addEventListener('click', function (event) {
            event.preventDefault();
                var felt = "Utdanning";
                var startÅr = $('#startYearU').val();
                var sluttÅr = $('#endYearU').val();
                var jobbTittel = $('#linje').val();
                var bedriftNavn = $('#skolename').val();

                var jsonJobb = {
            "felt": felt,
                    "par1": startÅr,
                    "par2": sluttÅr,
                    "par3": jobbTittel,
                    "par4": bedriftNavn,
                    "par5": ""
                };
                ajaxUpload(jsonJobb);
            });
            document.getElementById("submitSkill").addEventListener('click', function (event) {
            event.preventDefault();
                var felt = "Ferdigheter";
                var startÅr = $('#skillSlider').val() + "%";
                var sluttÅr = $('#skillbar').val();

                var jsonJobb = {
            "felt": felt,
                    "par1": sluttÅr,
                    "par2": startÅr,
                    "par3": "",
                    "par4": "",
                    "par5": ""
                };
                ajaxUpload(jsonJobb);
            });
            document.getElementById("submitSpråk").addEventListener('click', function (event) {
            event.preventDefault();
                var felt = "Språk";
                var startÅr = $('#språkbar').val();
                var sluttÅr = $('#språkSlider').val() + "%";
                var jsonJobb = {
            "felt": felt,
                    "par1": startÅr,
                    "par2": sluttÅr,
                    "par3": "",
                    "par4": "",
                    "par5": ""
                };
                ajaxUpload(jsonJobb);
            });
            function ajaxUpload(json) {
            $.ajax({
                type: "POST",
                url: '@Url.Action("LeggTilCV", "ProfilSide")',
                data: json,
                dataType: "json",
                success: function (result) {
                    alert("Dette er resultatet " + result.result);
                }
            });
            }


})

