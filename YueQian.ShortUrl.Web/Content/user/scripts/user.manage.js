$(function () {
    $("#manageForm").validate({
        errorElement: "span",
        errorClass: "invalid",
        rules: {
            OldPassword: { required: true },
            NewPassword: { required: true, minlength: 6 },
            ConfirmPassword: { required: true, equalTo: "#NewPassword" }
        },
        messages: {
            OldPassword: { required: "请输入原始密码" },
            NewPassword: { required: "请输入新密码", minlength: "密码不少于6位" },
            ConfirmPassword: { required: "再一次输入新密码", equalTo: "两次输入的密码不一致" }
        },
        submitHandler: function (form) {
            $.post("/User/Manage", $("#manageForm").serialize(),
				function (data) {
				    alert(data.msg);
				    if (data.result) {
				        window.location.href = data.url;
				    }
				    return false;
				}, "Json");
        }
    });

});