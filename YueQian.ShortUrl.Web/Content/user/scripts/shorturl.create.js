$(function () {
    jQuery.validator.addMethod("isUrl", function (value, element) {
        var webUrl = /((http|https):\/\/)?[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?/;
        return this.optional(element) || webUrl.test(value);
    }, "网址格式错误");
    $("#shorturlform").validate({
        errorElement: "span",
        errorClass: "invalid",
        rules: {
            Url: { required: true, isUrl: true }
        },
        messages: {
            Url: { required: "请输入原始地址", isUrl: "您输入网址格式错误" }
        },
        submitHandler: function (form) {
            $.post("/User/Edit", $("#shorturlform").serialize(),
				function (data) {
				    if (data.result) {
				        window.location.href = data.url;
				    }
				    else {
				        alert(data.msg);
				        return false;
				    }
				}, "Json");
        }
    });

});

function gotoList() {
    parent.openUrl("/User/List");
}