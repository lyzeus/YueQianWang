$(document).ready(function () {

    //Sidebar Accordion Menu:

    $("#main-nav li ul").hide(); // Hide all sub menus
    $("#main-nav li a.current").parent().find("ul").slideToggle("slow"); // Slide down the current menu item's sub menu

    $("#main-nav li a.nav-top-item").click( // When a top menu item is clicked...
        function () {
            $(this).parent().siblings().find("ul").slideUp("normal"); // Slide up all sub menus except the one clicked
            $(this).next().slideToggle("normal"); // Slide down the clicked sub menu
            return false;
        }
    );

    $("#main-nav li a.no-submenu").click( // When a menu item with no sub menu is clicked...
        function () {
            //window.location.href = (this.href); // Just open the link instead of a sub menu
            var url = this.getAttribute("href");
            if (url != undefined && url.length > 0 && url != "#" && url.substring(0, 1) != "#") {
                $("#main-nav a").removeClass("current");
                $(this).addClass("current");
                $("#contentFrame").attr("src", url);
            }
            return false;
        }
    );

    // Sidebar Accordion Menu Hover Effect:

    $("#main-nav li .nav-top-item").hover(
        function () {
            $(this).stop().animate({ paddingRight: "25px" }, 200);
        },
        function () {
            $(this).stop().animate({ paddingRight: "15px" });
        }
    );

    $("#main-nav ul a").click(function () {
        var url = this.getAttribute("href");
        if (url != undefined && url.length > 0
            && url != "#" && url.substring(0, 1) != "#") {
            $("#main-nav a").removeClass("current");
            $(this).addClass("current");
            $(this).parent().parent().parent().find(".nav-top-item").addClass("current");
            $("#contentFrame").attr("src", url);
            $("#cTitle").text(this.innerText);
        }
        return false;
    });
    $(".shortcut-button").click(function () {
        var url = this.getAttribute("href");
        if (url != undefined && url.length > 0
            && url != "#" && url.substring(0, 1) != "#") {
            $("#main-nav a").removeClass("current");
            $(this).addClass("current");
            $(this).parent().parent().parent().find(".nav-top-item").addClass("current");
            $("#contentFrame").attr("src", url);
            var nav_a = $("#main-nav a[href='" + url + "']");
            if (nav_a.length > 0) {
                var nav_a_parent = nav_a.eq(0).parent().parent().parent().find(".nav-top-item");
                nav_a_parent.get(0).click();
                nav_a.get(0).click();
            }
            $("#cTitle").text(this.innerText);
        }
        return false;
    });


    $("#usersign").click(function () {
        $.post("/User/Sign?cache=" + Math.random(), function (data) {
            easyDialog.open({
                container: {
                    header: data.result ? '签到成功' : '签到失败',
                    content: data.msg,
                    noFn: true,
                    noText: '确定'
                },
                drag: false
            });

            //签到成功后删除签到按钮
            $("#usersign").parent().remove();

        }, "Json");
    });


    ////Minimize Content Box

    //	$(".content-box-header h3").css({ "cursor":"s-resize" }); // Give the h3 in Content Box Header a different cursor
    //	$(".closed-box .content-box-content").hide(); // Hide the content of the header if it has the class "closed"
    //	$(".closed-box .content-box-tabs").hide(); // Hide the tabs in the header if it has the class "closed"

    //	$(".content-box-header h3").click( // When the h3 is clicked...
    //		function () {
    //		  $(this).parent().next().toggle(); // Toggle the Content Box
    //		  $(this).parent().parent().toggleClass("closed-box"); // Toggle the class "closed-box" on the content box
    //		  $(this).parent().find(".content-box-tabs").toggle(); // Toggle the tabs
    //		}
    //	);

    //// Content box tabs:

    //	$('.content-box .content-box-content div.tab-content').hide(); // Hide the content divs
    //	$('ul.content-box-tabs li a.default-tab').addClass('current'); // Add the class "current" to the default tab
    //	$('.content-box-content div.default-tab').show(); // Show the div with class "default-tab"

    //	$('.content-box ul.content-box-tabs li a').click( // When a tab is clicked...
    //		function() { 
    //			$(this).parent().siblings().find("a").removeClass('current'); // Remove "current" class from all tabs
    //			$(this).addClass('current'); // Add class "current" to clicked tab
    //			var currentTab = $(this).attr('href'); // Set variable "currentTab" to the value of href of clicked tab
    //			$(currentTab).siblings().hide(); // Hide all content divs
    //			$(currentTab).show(); // Show the content div with the id equal to the id of clicked tab
    //			return false; 
    //		}
    //	);

    ////Close button:

    //	$(".close").click(
    //		function () {
    //			$(this).parent().fadeTo(400, 0, function () { // Links with the class "close" will close parent
    //				$(this).slideUp(400);
    //			});
    //			return false;
    //		}
    //	);

    //// Alternating table rows:

    //	$('tbody tr:even').addClass("alt-row"); // Add class "alt-row" to even table rows

    //// Check all checkboxes when the one in a table head is checked:

    //	$('.check-all').click(
    //		function(){
    //			$(this).parent().parent().parent().parent().find("input[type='checkbox']").attr('checked', $(this).is(':checked'));   
    //		}
    //	);

    //// Initialise Facebox Modal window:

    //	$('a[rel*=modal]').facebox(); // Applies modal window to any link with attribute rel="modal"

    //// Initialise jQuery WYSIWYG:

    //	$(".wysiwyg").wysiwyg(); // Applies WYSIWYG editor to any textarea with the class "wysiwyg"

    //检测用户是否已经认证
    $.get("/Validate/CheckVerification?cache=" + Math.random(), function (data) {
        if (!data) {
            easyDialog.open({
                container: {
                    header: '个人信息认证',
                    content: '您还没有认证个人信息,是否现在认证?',
                    noFn: true,
                    noText: '下次认证',
                    yesFn: function () {
                        openUrl('/User/MyVerification');
                    },
                    yesText: '现在认证',
                },
                drag: false
            });
        }
        return false;
    }, "JSON");


});


function openUrl(url) {
    if (url != undefined && url.length > 0
        && url != "#" && url.substring(0, 1) != "#") {
        var nav_a = $("#main-nav a[href='" + url + "']");
        if (nav_a.length > 0) {
            var nav_a_parent = nav_a.eq(0).parent().parent().parent().find(".nav-top-item");
            nav_a_parent.get(0).click();
            nav_a.get(0).click();
            $("#cTitle").text(nav_a.get(0).innerText);
        }
    }
    return false;
}

function openAddUrl(url) {
    if (url != undefined && url.length > 0
        && url != "#" && url.substring(0, 1) != "#") {
        var nav_a = $("#main-nav a[href='/User/Create']");
        if (nav_a.length > 0) {
            var nav_a_parent = nav_a.eq(0).parent().parent().parent().find(".nav-top-item");
            nav_a_parent.get(0).click();
            nav_a.get(0).click();
            $("#contentFrame").attr("src", url);
            $("#cTitle").text(nav_a.get(0).innerText);
        }
    }
    return false;
}