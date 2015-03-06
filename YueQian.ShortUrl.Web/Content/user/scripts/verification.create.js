$(function () {
    $("#data-form").validate({
        errorElement: "span",
        errorClass: "invalid",
        rules: {
            Account: { required: true },
            RealName: { required: true },
            Amount: { required: true, digits: true, min: 10, max: 99999 },
            MobilePhone: { required: true }
        },
        messages: {
            Account: { required: "请输入帐号" },
            RealName: { required: "请输入账户名" },
            Amount: { required: "请输入提现金额", digits: "提现金额必须是整数", min: "提现金额不能小于10", max: "提现金额不能大于99999" },
            MobilePhone: { required: "手机号码必填" }
        },
        submitHandler: function (form) {
            $.post("/User/Verification", $("#data-form").serialize(),
				function (data) {
				    alert(data.msg);
				    if (data.result) {
				        window.location.href = data.url;
				    }
				    return false;
				}, "Json");
            return false;

        }
    });

    if ($("#PaymentId").val() == 2) {
        $("#pbank").show();
        $("#AccountBank").rules("add", {
            required: true,
            messages: {
                required: "请输入开户行"
            }
        });
    }


});


function paymentChange(select) {
    if (select.value == 2) {
        $("#pbank").show();
        $("#AccountBank").rules("add", {
            required: true,
            messages: {
                required: "请输入开户行"
            }
        });
    }
    else {
        $("#pbank").hide();
        $("#AccountBank").val("").rules("remove");
    }
}

function gotoList() {
    parent.openUrl("/User/List");
}