if (typeof jQuery != 'undefined') {
    $(document).ready(function () {

        if (sessionStorage) {

            getCookieSessionID();

            $('a').each(function () {
                var href = $(this).attr('href');
                var csidParam = 'csid=' + sessionStorage.sessionID;

                if (href) {
                    href = href.replace(/csid=xx/, csidParam);
                    $(this).attr('href', href);
                }
            });

        }

        trimCookies();

    });

}


function trimCookies() {

    if (document.cookie.length > 9000) { // 16K is default max request length in IIS, but above 10K IE stops reporting document.cookie.length correctly

        //loop round the cookies
        $.each(document.cookie.split(/; */), function () {
            var splitCookie = this.split('=');
            var cookieName = splitCookie[0];
            var cookieValue = splitCookie[1]

            // check for a traffic sheet cookie
            if (isTrafficSheetFilterCookie(cookieName, cookieValue)) {
                // found one (the oldest) so delete it then exit
                deleteCookie(cookieName);
                return false;
            }
        });


    }
}

function isTrafficSheetFilterCookie(name, value) {
    return (name.length == 6 && value == "TrafficSheetFilterJSON")
}


var deleteCookie = function(name) {
    document.cookie = name + '=; Path=/; expires=Thu, 01 Jan 1970 00:00:01 GMT;';
}

function getCookieSessionID() {

    // cookie sesion specified in query string takes precedence
    var qsParam = getParameterByName('csid');
    if (qsParam) sessionStorage.sessionID = qsParam;

    if (!sessionStorage.sessionID) {
        sessionStorage.sessionID = getRandomID(6);
    }
}


function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}


function getRandomID(length) {
    var text = "";
    var possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

    for (var i = 0; i < length; i++)
        text += possible.charAt(Math.floor(Math.random() * possible.length));

    return text;
}

function getCSID() {
    return (sessionStorage && sessionStorage.sessionID) ? '&csid=' + sessionStorage.sessionID : '';
}

function getCSIDSingle() {
    return (sessionStorage && sessionStorage.sessionID) ? '?csid=' + sessionStorage.sessionID : '';
}

function newCSID() {
    if (sessionStorage) {
        return '&csid=' + getRandomID(6);
    }
    else
        return "";
}

function newCSIDSingle() {
    if (sessionStorage) {
        return '?csid=' + getRandomID(6);
    }
    else
        return "";
}