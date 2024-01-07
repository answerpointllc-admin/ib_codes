$(document).ready(function () {
    $("#grid table").addClass("table-responsive").addClass("tablestyle");
});

function Search() {
    $('#grid').data('kendoGrid').dataSource.page(1);
    $('#grid').data('kendoGrid').dataSource.read();
}

function FilterData() {
    return {
        searchValue: $("#txtSearch").val()
    };
}

function appendDiv() {
    var divHtml = $("#hdnTimeFields").clone();
    var numItems = $('.addfields').length;
    if (numItems <= 6) {
        $(".addPropertyList").append(divHtml.html());

    }
    else {
        $("#btnPlus").prop('disabled', true);
    }
}
function removeDiv(e) {
    $(e).parents(".addfields").remove();
    var numItems = $('.addfields').length;
    if (numItems <= 6) {
        $("#btnPlus").prop('disabled', false);
    }
}
function divHide(e) {
    var data = $(e).parents(".addfields");
    data.find("#MinValue").attr('placeholder', 'Min Value');
    if (data.find("#DataTypeId").val() == 3) {
        data.find("#MinValue").attr('placeholder', 'Max Length');
        data.find("#MinValue").removeClass("d-none");
        data.find("#MaxValue").addClass("d-none");
    }
    else if (data.find("#DataTypeId").val() == 4) {
        data.find("#MinValue").addClass("d-none");
        data.find("#MaxValue").addClass("d-none");
    }
    else if (data.find("#DataTypeId").val() == 5) {
        data.find("#MinValue").addClass("d-none");
        data.find("#MaxValue").addClass("d-none");
    }
    else {
        data.find("#MinValue").removeClass("d-none");
        data.find("#MaxValue").removeClass("d-none");
    }
}
$("#blogform").on("submit", function () {
    var languageList = new Array();
    if ($(".addPropertyList .addfields").length > 0) {

        $(".addPropertyList .addfields").each(function () {
            var currentRow = $(this);
            var answers = currentRow.find("#PropertyName").val();
            var dd = currentRow.find("#DataTypeId").val();
            //var rightanswer2 = currentRow.find("#MaxLength").val();
            var rightanswer = parseFloat(currentRow.find("#MaxValue").val());
            var rightanswer1 = parseFloat(currentRow.find("#MinValue").val());

            var languageitem = {};
            if (answers != undefined) {
                languageitem.PropertyName = answers;
                languageitem.DataTypeId = dd;
                languageitem.MinValue = rightanswer1;
                languageitem.MaxValue = rightanswer;
                languageitem.MaxLength = 0; // rightanswer2;
                languageList.push(languageitem);
            }
        });
        if (languageList.length > 0) {
            $("#blogform #StrProperty").val(JSON.stringify(languageList))
            return true;
        }
        else {
            var $li = $("<li id='lang' class='error'/>").html(pleaseenteratleastonelanguage);
            $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
            if ($("#frmUser").valid()) {
                $('.validation-summary-errors li').remove()
            }
            if ($(".validation-summary-errors ul").find('#lang').length <= 0) {
                $(".validation-summary-errors ul").append($li);
            }
            $('html, body').animate({ scrollTop: 0 }, 500);
            //DisplayErrorMsg([pleaseenteratleastonelanguage], "#display-message");
            return false;
        }

    }
    else {
        var $li = $("<li id='lang' class='error'/>").html(pleaseenteratleastonelanguage);
        $(".validation-summary-valid").addClass("validation-summary-errors").removeClass("validation-summary-valid");// 
        if ($("#frmUser").valid()) {
            $('.validation-summary-errors li').remove()
        }
        if ($(".validation-summary-errors ul").find('#lang').length <= 0) {
            $(".validation-summary-errors ul").append($li);
        }
        $('html, body').animate({ scrollTop: 0 }, 500);
        //DisplayErrorMsg([pleaseenteratleastonelanguage], "#display-message");
        return false;
    }

});

function statusChange() {
    showConfirmation("Updating data over here will drop the current object and a new object will be created.", "Yes").then(function (result) {
        if (result.value) {
            $("form").submit();
            return true;
        }
        else {
            location.replace("/IoTDeviceProperty/Index");
        }
    });
}
