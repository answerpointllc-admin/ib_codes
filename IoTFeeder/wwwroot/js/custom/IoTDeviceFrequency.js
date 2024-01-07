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

$(function () {
    $('input[name="FrequencyType"]').on('click', function () {
        if ($(this).val() == 'false') {

            $('#fix').removeClass('d-none');
            $('#range').addClass('d-none');
            //$('#maxValue').addClass('d-none');
        }
        else {
            $('#range').removeClass('d-none');
            $('#fix').addClass('d-none');
            //$('#maxValue').removeClass('d-none');
        }
    });
});