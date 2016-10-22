$("#start-profile").on("click", function () {
    $("#start-profile").hide();
    $("#stop-profile").show();
    $.get('api/Profile/Start/1', function () {
        $("#stop-profile").hide();
        $("#wait-profile").hide();
        $("#start-profile").show();
    });
});

$("#stop-profile").on("click", function () {
    $("#stop-profile").hide();
    $("#wait-profile").show();
    $.get('api/Profile/Stop', function () {
        $("#wait-profile").hide();
        $("#start-profile").show();
    });
});