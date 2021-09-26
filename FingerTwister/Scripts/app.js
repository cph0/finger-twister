$(".game").on("touchstart", function (e) {
    var element = e.target;
    e.preventDefault();
    if (!started || element.classList.contains("btn-primary") || element.classList.contains("btn-danger") || element.classList.contains("btn-warning"))
        press(element.id);
});
$(".game").on("touchend", function (e) {
    var element = e.target;
    if (!started || element.classList.contains("btn-success"))
        release(element.id, e.originalEvent.touches.length);
});
$("#difficult").on("touchstart", function (e) {
    var element = e.target;
    if (element.innerHTML == "Currently Hard") {
        element.innerHTML = "Currently Easy";
        difficult = false;
    }
    else {
        element.innerHTML = "Currently Hard";
        difficult = true;
    }
});
var level = 1;
var lastID;
var difficult = false;
var started = false;
function press(id) {
    $("#" + id).removeClass("btn-primary");
    $("#" + id).removeClass("btn-danger");
    $("#" + id).removeClass("btn-warning");
    $("#" + id).addClass("btn-success");
    $.ajax({
        method: "POST",
        url: "/Home/GetData",
        data: {
            id: id
        },
        success: function (result) {
            if (result.finish) {
                $(".game").removeClass("btn-primary");
                $(".game").removeClass("btn-danger");
                $(".game").removeClass("btn-warning");
                $(".game").removeClass("btn-success");
                $(".game").addClass("btn-default");
                level = result.level;
                $("#level").html(result.level);
            }
            else {
                $("#" + result.nextID).removeClass("btn-default");
                if (result.player == 1)
                    $("#" + result.nextID).addClass("btn-primary");
                else if (result.player == 2)
                    $("#" + result.nextID).addClass("btn-danger");
                else if (result.player == 3)
                    $("#" + result.nextID).addClass("btn-warning");
                lastID = result.lastID;
                $("#scores").html(result.scores);
            }
            started = result.started;
        }
    });
}
function release(id, touch) {
    if (touch < level - 1 || (lastID != id && difficult)) {
        $("#" + id).removeClass("btn-success");
        $("#" + id).addClass("btn-default");
        $.ajax({
            method: "POST",
            url: "/Home/SendData",
            data: {
                id: id
            },
            success: function (result) {
            }
        });
    }
}
//# sourceMappingURL=app.js.map