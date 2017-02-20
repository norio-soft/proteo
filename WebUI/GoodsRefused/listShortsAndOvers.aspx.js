function HandleSelectAll(chk) {
    $(":checkbox[id$=chkSelectShort]").each(function (chkIndex) { if (this.checked != chk.checked) this.click(); });
}

