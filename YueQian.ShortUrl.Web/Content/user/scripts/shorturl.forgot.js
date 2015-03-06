$(function () {
    $("#loginform").validate({
        rules: {
            Email: { required: true }
        },
        messages: {
            Email: { required: "必须填写邮箱" }
        },
        submitHandler: function (form) {
            $.post("/User/Forgot", $("#loginform").serialize(),
				function (data) {
				    if (data.result) {
				        
				    } else {
				        alert(data.msg);
				        return false;
				    }
				}, "Json");
        }
    });

});