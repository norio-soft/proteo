function HandleSelectAll(chk) {
    $(":checkbox[id$=chkDelete]").each(function(chkIndex) { if (this.checked != chk.checked) this.click(); });
}

function txtOrderId_OnTextChanged(sender) {
    var currentItem = $('#' + sender.id);
    if (currentItem != null) {
        var currentRow = $('#' + currentItem.parent().parent().attr("id"));
        if (currentRow != null);
        {
            var rowIsDirty = currentRow.find('input[id*=hidRowDirty]');
            rowIsDirty.val('true');
        }
    }
}