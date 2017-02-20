jQuery.fn.dataTableExt.oSort['uk_date-asc'] = function (a, b) {

    var ukDatea = a.split(' ')[0].split('/');
    var ukDateb = b.split(' ')[0].split('/');

    var ukTimea = a.split(' ')[1].split(':');
    var ukTimeb = b.split(' ')[1].split(':');

    //Treat blank/non date formats as highest sort					
    if (isNaN(parseInt(ukDatea[0]))) {
        return 1;
    }

    if (isNaN(parseInt(ukDateb[0]))) {
        return -1;
    }

    var x = 0;
    var y = 0;
    if (isNaN(parseInt(ukTimea[0]))) {
        x = (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
    }
    else {
        x = (ukDatea[2] + ukDatea[1] + ukDatea[0] + ukTimea[0] + ukTimea[1]) * 1;
    }
    if (isNaN(parseInt(ukTimeb[0]))) {
        y = (ukDateb[2] + ukDateb[1] + ukDateb[0]) * 1;
    }
    else {
        y = (ukDateb[2] + ukDateb[1] + ukDateb[0] + ukTimeb[0] + ukTimeb[1]) * 1;
    }


    return ((x < y) ? -1 : ((x > y) ? 1 : 0));
};

jQuery.fn.dataTableExt.oSort['uk_date-desc'] = function (a, b) {
    var ukDatea = a.split(' ')[0].split('/');
    var ukDateb = b.split(' ')[0].split('/');

    var ukTimea = a.split(' ')[1].split(':');
    var ukTimeb = b.split(' ')[1].split(':');

    //Treat blank/non date formats as highest sort					
    if (isNaN(parseInt(ukDatea[0]))) {
        return -1;
    }

    if (isNaN(parseInt(ukDateb[0]))) {
        return 1;
    }

    var x = 0;
    var y = 0;
    if (isNaN(parseInt(ukTimea[0]))) {
        x = (ukDatea[2] + ukDatea[1] + ukDatea[0]) * 1;
    }
    else {
        x = (ukDatea[2] + ukDatea[1] + ukDatea[0] + ukTimea[0] + ukTimea[1]) * 1;
    }
    if (isNaN(parseInt(ukTimeb[0]))) {
        y = (ukDateb[2] + ukDateb[1] + ukDateb[0]) * 1;
    }
    else {
        y = (ukDateb[2] + ukDateb[1] + ukDateb[0] + ukTimeb[0] + ukTimeb[1]) * 1;
    }

    return ((x < y) ? 1 : ((x > y) ? -1 : 0));
};

jQuery.fn.dataTableExt.aTypes.push(
function (sData) {
    if (sData.match(/^(0[1-9]|[12][0-9]|3[01])\-(0[1-9]|1[012])\-(19|20|21)\d\d$/)) {
        return 'uk_date';
    }
    return null;
}
);
