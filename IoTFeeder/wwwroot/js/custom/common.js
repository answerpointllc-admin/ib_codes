function CheckLocationPermission() {
    navigator.permissions.query({ name: 'geolocation' }).then(data => {
        if (data.state == 'granted') { return true; }
        else { return false; }
    });
}

function getLocation() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    } else {
        alert("Geolocation is not supported by this browser.");
    }
}

function showPosition(position) {
    SetLocalStorage("current_latitude", position.coords.latitude);
    SetLocalStorage("current_longitude", position.coords.longitude);
}

function getAddress(lat, lng) {
    var latlng = new google.maps.LatLng(lat, lng);
    var geocoder = new google.maps.Geocoder();
    geocoder.geocode({ 'latLng': latlng }, function (results, status) {
        if (status === google.maps.GeocoderStatus.OK) {
            var address = results[0].formatted_address;
            SetLocalStorage("search_location", address);
            $(".c-search-filters #Address").val(address);
        }
    });
}

var showOverlay = true;
var isAllChecked = false;
var isUnChecked = false;
var pageNumber = 0;

$(document).ready(function () {
    setTimeout(function () {
        $(".close-navbar").trigger("click");
    }, 10);

    $(document).ajaxStart(function () {
        $(".k-loading-image").hide();
        if (showOverlay)
            $("#divWait").css({ "display": "block" });
    });
    $(document).ajaxStop(function () {
        $("#divWait").css({ "display": "none" });
        $(".k-loading-image").show();
    });
    if ($("#phone").length > 0) {
        $("#phone").mask("(999) 999-9999?");
    }
    if ($("#faxnumber").length > 0) {
        $("#faxnumber").mask("(999) 999-9999?");
    }
    $('.addempform').hide();
    $('.addempbtn').click(function (e) {

        $('.addempform').slideDown('slow', function () {
            reposition();
        });
        $('.addempbtn').removeClass('selected');
        $this = $(this);

        divID = $this.attr('title');
        $('#' + divID).slideDown('slow');

    });
    $('.closeform').click(function (e) {
        $(this).parents('.addempform').slideUp('slow', function () {
            reposition();
        });
    });
    $('#add').click(function () {
        return !$('#select1 option:selected').remove().appendTo('#select2');
    });
    $('#remove').click(function () {
        return !$('#select2 option:selected').remove().appendTo('#select1');
    });
    $('form').submit(function () {
        $('#select2 option').each(function (i) {
            $(this).attr("selected", "selected");
        });
    });
    $('.modal').on('show.bs.modal', function () {
        reposition($(this));
    });
    $('.modal-child').on('show.bs.modal', function () {
        var modalParent = $(this).attr('data-modal-parent');
        $(modalParent).css('opacity', 0);

    });
    $('.modal-child').on('hidden.bs.modal', function () {
        var modalParent = $(this).attr('data-modal-parent');
        $(modalParent).css('opacity', 1);

    });
});
function SetLocalStorage(name, value) {
    localStorage.setItem(name, value);
}

function initializeAllCheckBox() { var n = $("#allCheckBox").is(":checked"); $(".singleCheckBox").prop("checked", n ? !0 : !1); $(".singleCheckBox").closest("tr").addClass(n ? "selected-row" : "not-selected-row"); $(".singleCheckBox").closest("tr").removeClass(n ? "not-selected-row" : "selected-row") }
function initializeSingleCheckBox(n) { var t = $(n).is(":checked"); $(n).closest("tr").addClass(t ? "selected-row" : "not-selected-row"); $(n).closest("tr").removeClass(t ? "not-selected-row" : "selected-row"); t && $(".singleCheckBox").length == $(".selected-row").length ? $("#allCheckBox").prop("checked", !0) : $("#allCheckBox").prop("checked", !1) }
//function Check_CheckBox_Count() { var n = $("input[name=chkDelete]"), t = n.filter(":checked").length; return t > 0 ? confirm("Are you sure? Do you want to delete record(s)?") : (alert("Please select at least one checkbox to delete."), !1) }
function validateEmail(n) { return n.match(/^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/) ? !0 : !1 }
function ValidateNumber(n) { var i = window.event || n || event, t = i.which || i.keyCode; return t > 31 && (t < 48 || t > 57) && (t < 96 || t > 105) ? !1 : !0 }
function isNumberKey(n, t, i, r) { var o = GetCursorLocation(n), u = t.which ? t.which : t.keyCode, e, s, f; if (u > 31 && (u < 48 || u > 57) && (u < 96 || u > 105) && u != 46 && u != 190 && u != 110 && !(u == 8 || u == 16 || u == 32 || u == 35 || u == 36 || u == 37 || u == 39 || u == 46)) return !1; if (e = i, s = r, Num = n.value, f = Num.indexOf("."), f != -1) { if (u != 9 && u != 8 && Num.length - f > s && (o > f || f > e - 1) || f > e - 1 && o < f && u != 8 && u != 9) return !1 } else if (u != 9 && u != 8 && Num.length > e - 1 && u != 46) return !1; return !0 }
function isNumberOnlyKey(n, t) { console.log("isNumberOnlyKey"); var u = GetCursorLocation(n), i = t.which ? t.which : t.keyCode, r = t.keyCode; return i > 95 && i < 106 ? !0 : i > 31 && (i < 46 || i > 57) && i != 47 && r != 9 && r != 37 && r != 38 && r != 39 && r != 40 ? !1 : i == 47 ? !1 : !0 }
function GetCursorLocation(n) { var t, i, u, r = -1; return typeof n.selectionStart == "number" ? r = n.selectionStart : document.selection && n.createTextRange && (t = document.selection, t && (u = t.createRange(), i = n.createTextRange(), i.setEndPoint("EndToStart", u), r = i.text.length)), r }
function populateDropdown(n, t, i) { n.html(""); n.append($("<option><\/option>").val("").html(i)); $.each(t, function (t, i) { n.append($("<option><\/option>").val(i.Value).html(i.Text)) }); $(n).removeAttr("disabled") }
function populateDropdownWithoutLabel(n, t) { $(n).children("option:not(:first)").remove(); $.each(t, function (t, i) { n.append($("<option><\/option>").val(i.Value).html(i.Text)) }); $(n).removeAttr("disabled") }
function validateFromDateToDate(n, t) { var i = n.split("/"), u = parseInt(i[0]), f = parseInt(i[1]), e = parseInt(i[2]), r = t.split("/"), o = parseInt(r[0]), s = parseInt(r[1]), h = parseInt(r[2]), c = new Date(u + "/" + f + "/" + e), l = new Date(o + "/" + s + "/" + h); return c > l ? !1 : !0 }
function getQueryString(n) { for (urlStr = window.location.search.substring(1), sv = urlStr.split("&"), i = 0; i < sv.length; i++) if (ft = sv[i].split("="), ft[0] == n) return ft[1] }
//function DisableCutCopyPaste() { $("input[type='password']").bind("cut copy paste", function (n) { n.preventDefault() }) } var dateFomat = "mm/dd/yyyy", MaskingFormatPhone = "(000) 000-0000", MaskingFormatZip = "00000", MaskingFormatLicense = "000000000", MaskingFormatFifteenDigit = "000000000000000", MaskingFormatNineDigit = "000000000", _ValidFileExtention = "You can only upload files like .xls, .xlsx.", _RequiredField = "##FieldName## is required.", _StartFileProcess = "File process starting...", _ValidationCompleted = "Validation process has been completed and starting to process file...", _RecordCount = "Validating records ##MsgRecordCount## out of ##MsgTotalRecord##", _ErrorValidating = "Some error occurred while validating file...", _FileProcessCompleted = "File process has been completed....", _FileProcessing = "Processing records ##MsgRecordCount## out of ##MsgTotalRecord##", _ErrorProcessing = "Some error occurred while processing file...", _AjaxSuccess = "1", _AjaxFailure = "0", ww, adjustMenu; $.validator.setDefaults({ ignore: [] }); $(document).on("change", ".btn-file :file", function () { var n = $(this), t = n.get(0).files ? n.get(0).files.length : 1, i = n.val().replace(/\\/g, "/").replace(/.*\//, ""); n.trigger("fileselect", [t, i]) }); $(document).ready(function () { $(".btn-file :file").on("fileselect", function (n, t, i) { var u = $(this).parents(".input-group").find(":text"), r = t > 1 ? t + " files selected" : i; u.length ? u.val(r) : r && alert(r) }); DisableCutCopyPaste() }); ww = $(window).width(); $(window).load(function () { $("#main-menu li a").each(function () { $(this).next().length > 0 && $(this).addClass("parent") }); $(".toggleMenu").click(function (n) { n.preventDefault(); $(this).toggleClass("active"); $("#main-menu").toggle() }); adjustMenu() }); $(window).bind("resize orientationchange", function () { ww = $(window).width(); adjustMenu() }); adjustMenu = function () { ww < 1023 ? ($(".toggleMenu").css("display", "inline-block"), $(".toggleMenu").hasClass("active") ? $("#main-menu").show() : $("#main-menu").hide(), $("#main-menu li").unbind("mouseenter mouseleave"), $("#main-menu li a.parent").unbind("click").bind("click", function (n) { n.preventDefault(); $(this).parent("li").toggleClass("hover") })) : ww >= 768 && ($(".toggleMenu").css("display", "none"), $("#main-menu").show(), $("#main-menu li").removeClass("hover"), $("#main-menu li a").unbind("click"), $("#main-menu li").unbind("mouseenter mouseleave").bind("mouseenter mouseleave", function () { $("#main-menu li").hoverIntent({ over: function () { $(this).removeClass("hover").addClass("hover") }, out: function () { $(this).removeClass("hover") } }) })) }; $("#main-menu").bind("show.smapi", function (n, t) { $(t).dataSM("parent-a").children("span.sub-arrow").text("-") }); $("#main-menu").bind("hide.smapi", function (n, t) { $(t).dataSM("parent-a").children("span.sub-arrow").text("+") })
function reposition(mod) {
    var modal = mod ? mod : $('.modal:visible');
    var dialog = modal.find('.modal-dialog');
    modal.css('display', 'block');
    dialog.css("margin-top", Math.max(0, ($(window).height() - dialog.height()) / 2));
}
$(window).on('resize', function () {
    $('.modal:visible').each(reposition);
});
$('body').on('click', function (e) {
    $('[rel="popover"]').each(function () {
        if (!$(this).is(e.target) && $(this).has(e.target).length === 0 && $('.popover').has(e.target).length === 0) {
            $(this).popover('hide');
        }
    });
});
/*-----Page Active-----*/
function PageActive(pageid) {
    $('#' + pageid).addClass("active");
}
function onGridDataBound(n) { if (!n.sender.dataSource.view().length) { var t = n.sender.thead.find("th:visible").length, i = '<tr><td colspan="' + t + '" align="center">No records found.<\/td><\/tr>'; n.sender.tbody.parent().width(n.sender.thead.width()).end().html(i) } }

function showLoader() {
    $("#divWait").css({ "display": "block" });
}

function hideLoader() {
    $("#divWait").css({ "display": "none" });
}

function GetLocalStorage(name) {
    var searchResult = localStorage.getItem(name);
    if (searchResult !== null) { return searchResult.split(",,,,"); }
}

//var successMsgHtml = '<div class="alert alert-success" role="alert"><span id = "msg-caption" ></span></div >';
var successMsgHtml = '<div class="green-box" role="alert"><span id = "msg-caption" ></span></div >';
var errorMsgHtml = '<div class="alert alert-danger" role="alert"><span id = "msg-caption" ></span></div >';

function DisplayErrorMsg(msg, errorDivId) {
    var message = "<ul class='p-0 mb-0 list-unstyled'>";
    $.each(msg, function (i) {
        message += "<li> " + msg[i] + " </li>";
    });
    message += "</ul>";
    if (message !== "" && message !== undefined) {
        $(errorDivId).removeClass("displaynone");
        var specialMatch = message.match(/[^[\]]+(?=])/g);
        if (specialMatch) {
            var html = $(errorMsgHtml);
            html.find("#msg-caption").html(specialMatch[0].replace('"', '').replace('"', ''));
            $(errorDivId).html(html);
        }
        else {
            var _html = $(errorMsgHtml);
            _html.find("#msg-caption").html(message);
            $(errorDivId).html(_html);
        }
    }
}

function DisplaySuccessMsg(msg, divId) {
    var message = "<ul class='p-0 mb-0 list-unstyled'>";
    //$.each(msg, function (i) {
    message += "<li> " + msg + " </li>";
    //});
    message += "</ul>";
    if (message !== "" && message !== undefined) {
        $(divId).removeClass("displaynone");
        var specialMatch = message.match(/[^[\]]+(?=])/g);
        if (specialMatch) {
            var html = $(successMsgHtml);
            html.find("#msg-caption").html(specialMatch[0].replace('"', '').replace('"', ''));
            $(divId).html(html);
        }
        else {
            var _html = $(successMsgHtml);
            _html.find("#msg-caption").html(message);
            $(divId).html(_html);
        }
    }
}

function ShowImage(path, file) {
    if (file != undefined && file != null && file != "") {
        $("#divImg #img").html("");
        $("#divImg #img").append("<img class='img-fluid' src='" + path + file + "' alt='img' />");
        $("#divImg").modal('show');
    }
}

function SearchExt(str, strArray) {
    for (var j = 0; j < strArray.length; j++) {
        if (strArray[j].match(str)) { return true; }
    }
    return false;
}
function DateTimeFilter(element) {
    $(element).kendoDatePicker({
        format: "MM/dd/yyyy"
    });
};

function OnlyNumber(e) {
    var keyCode = e.which ? e.which : e.keyCode;

    //if (!(keyCode >= 48 && keyCode <= 57)) {
    //    return false;
    //}
    if (keyCode > 31 && (keyCode < 48 || keyCode > 57)) {
        return false;
    }
}


function PhoneNumberVal(e) {
    var keyCode = e.which ? e.which : e.keyCode;

    if (!(keyCode >= 48 && keyCode <= 57) && keyCode != 40 && keyCode != 41 && keyCode != 45) {
        return false;
    }
}

function IsNOTUndefinedNullEmpty(value) {
    if (value == undefined || value == null || value == '') {
        return false;
    }
    return true;
}
function IsNOTUndefinedOrNullOrEmptyOrZero(value) {
    if (value == undefined || value == null || value == '' || value == '0' || value == 0) {
        return false;
    }
    return true;
}

//function IsDecimalKey(e) {
//    var evt = window.event;
//    var charCode = (evt.which) ? evt.which : evt.keyCode
//    if (charCode > 31 && (charCode < 48 || charCode > 57) && charCode != 46)
//        return false;
//    else {
//        var $txtBox = $(e);
//        var len = $txtBox.val().length;
//        var index = $txtBox.val().indexOf('.');
//        debugger
//        if (index >= 0 && charCode == 46) {
//            return false;
//        }
//    }
//    return true;
//}

function IsDecimalKey(evt) {
    var charCode = (evt.which) ? evt.which : evt.keyCode;
    if (charCode != 46 && charCode > 31
        && (charCode < 48 || charCode > 57))
        return false;
    return true;
}

function CheckDecimalKey(evt) {
    var ex = /^[0-9]+\.?[0-9]*$/;
    if (ex.test(evt.value) == false) {
        return false;
    }
}

function OnlyAlphabets(event) {
    //if ((event.charCode > 64 && event.charCode < 91) || (event.charCode > 96 && event.charCode < 123)) {
    //    return true;
    //} else {
    //    return false;
    //}
    var regex = /^[a-zA-Z ]*$/;
    if (regex.test(event.value) == false) {
        return false;
    }
}

function toastMsg(msg, isSuccess = true, title = undefined, timeOut = 5000) {
    if (title == undefined) { title = isSuccess ? "Success!" : "Error!"; }
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": true,
        "progressBar": true,
        "positionClass": "toast-bottom-right",
        "preventDuplicates": true,
        "onclick": null,
        "showDuration": 300,
        "hideDuration": 100,
        "timeOut": timeOut,
        "extendedTimeOut": 1000,
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    }
    if (isSuccess)
        toastr.success(msg, title);
    else
        toastr.error(msg, title);
}
function removeURLParameter(url, parameter) {
    var urlparts = url.split('?');
    if (urlparts.length >= 2) {
        var prefix = encodeURIComponent(parameter) + '=';
        var pars = urlparts[1].split(/[&;]/g);
        for (var i = pars.length; i-- > 0;) {
            if (pars[i].lastIndexOf(prefix, 0) !== -1) {
                pars.splice(i, 1);
            }
        }

        url = urlparts[0] + (pars.length == 0 ? '' : '?') + pars.join('&');
        return url;
    }
    else {
        return url;
    }
}

async function showConfirmation(message, buttonText) {
    return Swal.fire(
        {
            title: "Are you sure?",
            text: message,
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: buttonText
        });
}


function Check_CheckBox_Count(e) {
    var n = $("input[name=chkDelete]");
    var t = n.filter(":checked").length;
    if (t > 0) {
        var recordsCount = t == 1 ? "record" : (t > 1 ? "record(s)" : '');
        showConfirmation("Are you sure? Do you want to delete " + recordsCount + "?", 'Yes').then((result) => {
            if (result.value) {
                $(e).parents("form").submit();
                return true;
            }
            else {
                return false;
            }
        });
    }
    else {
        toastMsg("Please select at least one checkbox to delete.", false);
        return false;
    }
}

function initializeAllCheckBoxById(id) { var n = $(id).is(":checked"); $(".singleCheckBox").prop("checked", n ? !0 : !1); $(".singleCheckBox").closest("tr").addClass(n ? "selected-row" : "not-selected-row"); $(".singleCheckBox").closest("tr").removeClass(n ? "not-selected-row" : "selected-row") }

function IsAlphaNumeric(e) {
    var regex = /^[a-z 0-9]+$/i;
    if (regex.test(e.key) == false) {
        return false;
    }
    //var keyCode = e.keyCode == 0 ? e.charCode : e.keyCode;
    //var ret = ((keyCode >= 48 && keyCode <= 57) || (keyCode >= 65 && keyCode <= 90) || (keyCode >= 97 && keyCode <= 122));
    //return ret;
}
function initializeAllCheckBoxById(id) { var n = $(id).is(":checked"); $(".singleCheckBox").prop("checked", n ? !0 : !1); $(".singleCheckBox").closest("tr").addClass(n ? "selected-row" : "not-selected-row"); $(".singleCheckBox").closest("tr").removeClass(n ? "not-selected-row" : "selected-row") }

$(".toggle-password").click(function () {
    $(this).toggleClass("fa-eye fa-eye-slash");
    var input = $($(this).attr("toggle"));
    if (input.attr("type") == "password") {
        input.attr("type", "text");
    } else {
        input.attr("type", "password");
    }
});

function scheduler_dataBound(e) {
    $(".k-scheduler .k-scheduler-navigation .k-nav-today a").text($("#hdnTodayText").val())
    $(".k-scheduler .k-scheduler-times .k-scheduler-times-all-day").html($("#hdnAlldayText").val());
    $(".k-scheduler .k-scheduler-footer").remove();
}