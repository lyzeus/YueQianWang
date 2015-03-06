$(function () {

    $("#vcode").click(function () {
        var src = $(this).attr("src") + "?cache=" + Math.random();
        $(this).attr("src", src);
    });
    $("#registerform").validate({
        rules: {
            InviteCode: { required: true },
            UserName: {
                required: true,
                remote: {
                    type: "POST",
                    async: false,
                    url: "/Validate/CheckUserName",
                    data: {
                        UserName: function () {
                            return $("#UserName").val()
                        }
                    }
                }
            },
            Password: { required: true, minlength: 6 },
            ConfirmPassword: { equalTo: "#Password" },
            Email: { required: true, email: true },
            VerifyCode: { required: true }
        },
        messages: {
            InviteCode: { required: "请先获取邀请码" },
            UserName: { required: "请填写用户名", remote: "用户名已存在" },
            Password: { required: "请填写密码", minlength: "密码不少于6位" },
            ConfirmPassword: { required: "请填写密码", minlength: "密码不少于6位", equalTo: "两次输入的密码不一致" },
            Email: { required: "请填写Email", email: "邮箱格式不正确" },
            VerifyCode: { required: "请填写验证码" }
        },
        submitHandler: function (form) {
            $.post("/User/Register", $("#registerform").serialize(),
				function (data) {
				    if (data.result) {
				        window.location.href = data.url;
				    }
				    else {
				        alert(data.msg);
				        document.getElementById("vcode").click();
				        return false;
				    }
				}, "Json");
        }
    });

});