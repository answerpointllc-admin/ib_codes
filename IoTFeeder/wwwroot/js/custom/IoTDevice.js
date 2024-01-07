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

