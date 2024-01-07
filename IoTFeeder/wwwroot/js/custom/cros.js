$(document).ready(function () {
    $("#grid table").addClass("table-responsive").addClass("tablestyle");
});
var id = $("#Id").val() != undefined ? $("#Id").val() : 0;
var _NotConfirmed = "Not Confirmed";
var _Confirmed = "Confirmed";
var _PasswordNotMatch = "Password and confirmed password do not match.";
function funCofirmPassword() { document.getElementById("Password").value.length > 0 && document.getElementById("ConfirmPassword").value.length > 0 && document.getElementById("Password").value != document.getElementById("ConfirmPassword").value ? (document.getElementById("spanConfirm").innerHTML = _PasswordNotMatch, document.getElementById("spanConfirm").style.color = "red") : document.getElementById("Password").value.length > 0 && document.getElementById("ConfirmPassword").value.length == 0 ? (document.getElementById("spanConfirm").innerHTML = _NotConfirmed, document.getElementById("spanConfirm").style.color = "red") : (document.getElementById("spanConfirm").innerHTML = _Confirmed, document.getElementById("spanConfirm").style.color = "green") }
jQuery(document).ready(function () { $("#Password").keyup(function () { $("#passComment").html(passwordStrength($("#Password").val(), $("#FirstName").val())); document.getElementById("passComment").style.color = $("#passComment").html() == "&nbsp;&nbsp;Too short" || $("#passComment").html() == "&nbsp;&nbsp;Average" ? "red" : "green" }) }); $(document).ready(function () { document.getElementById("FirstName")/*.focus()*/ }); $(function () { $("#Password").change(function () { return document.getElementById("Password").value.length > 0 ? (document.getElementById("spanConfirm").innerHTML = _NotConfirmed, document.getElementById("spanConfirm").style.color = "red") : document.getElementById("spanConfirm").innerHTML = "", !1 }) })
$("#btnSubmit").click(function () {
    if ($("#frmClient").valid() && !isEmailDup && !isUserDup) {
        if (document.getElementById("Password").value != document.getElementById("ConfirmPassword").value) {
            return false;
        }
        if (isEmailDup || isUserDup) {
            if ($(".validation-summary-errors ul li#user-name-error").length < 1 && isUserDup) {
                var $li = $("<li id='user-name-error'/>").html(userDupMessage);
                $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
                $(".validation-summary-errors ul").append($li);
            }
            if ($(".validation-summary-errors ul li#email-name-error").length < 1 && isEmailDup) {
                var $li = $("<li id='email-name-error'/>").html(emailDupMessage);
                $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
                $(".validation-summary-errors ul").append($li);
            }
            return false;
        }
    }
    else {
        if ($(".validation-summary-errors ul li#user-name-error").length < 1 && isUserDup) {
            var $li = $("<li id='user-name-error'/>").html(userDupMessage);
            $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
            $(".validation-summary-errors ul").append($li);
        }
        if ($(".validation-summary-errors ul li#email-name-error").length < 1 && isEmailDup) {
            var $li = $("<li id='email-name-error'/>").html(emailDupMessage);
            $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
            $(".validation-summary-errors ul").append($li);
        }
        //e.preventdefault()
        return false;
    }
})

var isUserDup = false;
var isEmailDup = false;
var userDupMessage = "";
var emailDupMessage = "";

function IsUserNameExists() {
    if ($("#FirstName").val().trim().length > 0) {
        var n = SiteUrl + "CROs/ValidateDuplicateCROs",
            t = { username: $("#FirstName").val().trim(), Id: id };
        $.post(n, t, function (n) {
            $(".validation-summary-errors ul li#user-name-error").remove();
            if (n.Status) {
                isUserDup = true;
                userDupMessage = n.Message;
                if ($(".validation-summary-errors ul li#user-name-error").length < 1) {
                    var $li = $("<li id='user-name-error'/>").html(n.Message);
                    $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
                    $(".validation-summary-errors ul").append($li);
                }
            }
            else {
                isUserDup = false;
                if ($(".validation-summary-errors ul li:visible").length < 1) {
                    $(".validation-summary-errors").addClass("validation-summary-valid").removeClass("validation-summary-errors");
                }
            }
        }, "json")
    }
}

function IsUserEmailExists() {
    if ($("#FirstName").val().trim().length > 0) {
        var n = SiteUrl + "CROs/ValidateDuplicateEmail",
            t = { email: $("#Email").val().trim(), Id: id };
        $.post(n, t, function (n) {
            $(".validation-summary-errors ul li#email-name-error").remove();
            if (n.Status) {
                isEmailDup = true;
                emailDupMessage = n.Message;
                if ($(".validation-summary-errors ul li#email-name-error").length < 1) {
                    var $li = $("<li id='email-name-error'/>").html(n.Message);
                    $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
                    $(".validation-summary-errors ul").append($li);
                }
            }
            else {
                isEmailDup = false;
                if ($(".validation-summary-errors ul li:visible").length < 1) {
                    $(".validation-summary-errors").addClass("validation-summary-valid").removeClass("validation-summary-errors");
                }
            }
        }, "json")
    }
}

$(function () {
    //$("#Username").focusout(function () {
    //    IsUserNameExists()
    //})

    //$("#Email").focusout(function () {
    //    IsUserEmailExists()
    //})
    $("#PhoneNumber").mask("(999) 999 9999");

    $("form input[data-val-remote-url]").on({
        focus: function () {
            $(this).closest('form').validate().settings.onkeyup = false;
        },
        blur: function () {
            $(this).closest('form').validate().settings.onkeyup = $.validator.defaults.onkeyup;
        }
    });
})
function Search() {
    $('#grid').data('kendoGrid').dataSource.page(1);
    $('#grid').data('kendoGrid').dataSource.read();
    $('#grid').data('kendoGrid').refresh();
}

function FilterData() {
    return {
        searchValue: $("#txtSearch").val()
    };
}