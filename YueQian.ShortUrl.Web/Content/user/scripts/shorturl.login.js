$(function () {
    $("#loginform").validate({
        rules: {
            UserName: { required: true },
            Password: { required: true, minlength: 6 }
        },
        messages: {
            UserName: { required: "请填写用户名" },
            Password: { required: "请填写密码", minlength: "密码不少于6位" }
        },
        submitHandler: function (form) {
            $.post("/User/Login", $("#loginform").serialize(),
				function (data) {
				    if (data.result) {
				        window.location.href = data.url;
				    } else {
				        alert(data.msg);
				        $("#Password").val("");
				        return false;
				    }
				}, "Json");
        }
    });

});