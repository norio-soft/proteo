function filterOptionsDisplayToggle(show) {
    $('#overlayedClearFilterBox').toggle(show);
    $('#filterOptionsDiv').toggle(!show);
    $('#filterOptionsDivHide').toggle(show);
}

$(function () {
    $('#filterOptionsDiv').on('click', function () { filterOptionsDisplayToggle(true); });
    $('#filterOptionsDivHide').on('click', function () { filterOptionsDisplayToggle(false); });
});

function openJobDetails(jobID) {
    var url = '/job/job.aspx?wiz=true&jobId=' + jobID + getCSID();
    open(url, "JobDetails", "width=1200, height=900, scrollbars=yes, resizable=yes, status=1");
}
