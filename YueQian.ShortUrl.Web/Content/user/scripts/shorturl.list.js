$(function () {
    $(".del").click(function () {
        if (confirm('确实要删除短址"' + $(this).attr("data-title") + '"吗?')) {
            $.post(this.getAttribute("href"), function (data) {
                alert(data.msg);
                if (data.result)
                    window.location.href = data.url;
                return false;
            }, "JSON");
        }
        return false;
    });

    ZeroClipboard.config({
        moviePath: '/Scripts/ZeroClipboard/ZeroClipboard.swf',
        swfPath: "/Scripts/ZeroClipboard/ZeroClipboard.swf"
    });

    $(".copy").each(function () {
        var _this = $(this);
        var client = new ZeroClipboard(_this);
        client.on("load", function (readyEvent) {
            client.on("complete", function (client, args) {
                _this.css("color", "#0f0");
                _this.text("已复制");
            });
        });
    });
});