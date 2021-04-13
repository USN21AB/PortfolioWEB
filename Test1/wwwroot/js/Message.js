
function fn() {
    alert("Inni message");
    $.ajax({
        type: "GET",
        url: '@Url.Action("SendMelding", "Home")',
    });
}