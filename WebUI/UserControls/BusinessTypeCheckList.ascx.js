$(document).ready(function () {
    var storeBusinessTypes = function () {
        var selectedIDs = $.map($('.chkBusinessType:checked'), function (el) { return el.id.split('_')[1]; });
        $('input:hidden[id$=hidBusinessTypeIDs]').val(selectedIDs.join(','));
    };

    var restoreBusinessTypes = function () {
        $('.chkBusinessType').prop('checked', false);
        var selectedIDs = getSelectedBusinessTypeIDs();
        $.each(selectedIDs, function (idx, btID) {
            $('#chkBusinessType_' + btID).prop('checked', true);
        });
        $('#chkBusinessTypeAll').prop('checked', allBusinessTypesSelected());
    };

    var allBusinessTypesSelected = function () {
        return $('.chkBusinessType:not(:checked)').length == 0;
    }

    restoreBusinessTypes();

    $('#chkBusinessTypeAll').click(function () {
        $('.chkBusinessType').prop('checked', this.checked);
        storeBusinessTypes();
    });

    $('.chkBusinessType').click(function () {
        $('#chkBusinessTypeAll').prop('checked', allBusinessTypesSelected());
        storeBusinessTypes();
    });
});

function getSelectedBusinessTypeIDs() {
    var retVal = $('input:hidden[id$=hidBusinessTypeIDs]').val().trim().split(',');
    retVal = $.grep(retVal, function (i) { return i.length > 0; });
    return retVal;
}
