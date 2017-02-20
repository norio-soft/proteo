// JScript File
var layoutType = new Array();
layoutType[0] = 1;

//TOOLTIP OFFSET
var xOffsetArray = new Array();
xOffsetArray[0] = 0;

var yOffsetArray = new Array();
yOffsetArray[0] = 0;

//TOOLTIP BOX DEFAULT WIDTH
//	standard values are 250, 276, or 300 depending on what layout you are using
var toolTipWidth = new Array();
toolTipWidth[0] = 276;

var timeOutID = -1;


//TOOLTIP STYLING
// 	Allows you to adjust the tooltip background color for the 
//	roll-over and roll-off states, the font used for the tooltip,
// 	the colors of the title and display URL links for the roll-over
//	and roll-off states and whether or not the links should be
// 	underlined at any point.
//	-------------- 
var tooltipBkgColor = "#EEEEEE";
var tooltipHighlightColor = "#FFFFE0";
var tooltipFont = "Verdana, Arial, Helvetica";
var tooltipTitleColorOff = "#0000DE";
var tooltipTitleColorOn = "#0000DE";
var tooltipURLColorOff = "#008000";
var tooltipURLColorOn = "#008000";
var tooltipTitleDecorationOff = "none";
var tooltipTitleDecorationOn = "underline";
var tooltipURLDecorationOff = "none";
var tooltipURLDecorationOn = "underline";
var xmlRequest = null;

//............................................................
// 	

//FIREFOX STYLE TWEAK
if(navigator.appName == "Netscape")
{
	document.write("<style>.orchestratorLink{padding-bottom: 1px;}</style>");
}

function changeStyle(objectID, propertyName, propertyValue)
{
	document.getElementById(objectID).style[propertyName] = propertyValue;
}

function changeProperty(objectID, propertyName, propertyValue)
{
	document.getElementById(objectID)[propertyName] = propertyValue;
}

function getStyleValue(objectID, propertyName)
{
	return document.getElementById(objectID).style[propertyName];
}

function getPropertyValue(objectID, propertyName)
{
	return document.getElementById(objectID)[propertyName];
}

// PROCEDURAL FUNCTIONS
var hideID = 0;
var lastToolNum = 0;
var tooltipXOffset = 10;
var tooltipYOffset = 1;

function displayStatus(string)
{
	window.status = string;
	return true;
}

function clearStatus()
{
	window.status = '';
	return true;
}

function getRealPos(ele,dir)
{
	(dir=="x") ? pos = ele.offsetLeft : pos = ele.offsetTop;
	tempEle = ele.offsetParent;
	while(tempEle != null)
	{
		pos += (dir=="x") ? tempEle.offsetLeft : tempEle.offsetTop;
		tempEle = tempEle.offsetParent;
	}
	return pos;
}

function getScrollY()
{
	if(window.pageYOffset != null) {
		return window.pageYOffset;
	} else {
		return document.body.scrollTop;
	}
}

function getScrollX()
{
	if(window.pageXOffset != null){
		return window.pageXOffset;
	} else {
		return document.body.scrollLeft;
	}
}

function adDelay()
{
    clearInterval(timeOutID);
	//close box
	changeStyle('tooltipBox', 'visibility', 'hidden');
	//clear ID
	clearInterval(hideID);
	//clear status message
	displayStatus(' ');
	//clear ad content to turn off possible flash audio
	changeProperty('tooltipBox', 'innerHTML', "");
}

function clearAdInterval()
{
	clearInterval(hideID);
}

function hideAd()
{
	clearInterval(hideID);
	//hideID = setInterval(adDelay, 250);
    adDelay();
	//THIN DOUBLE UNDERLINE
	//linkRefString = "link" + lastToolNum;
	//changeStyle(linkRefString, 'borderBottomWidth', '1px');
	
	//TURN OFF AUDIO
    //turnSoundOff();
    closeToolTip();
}



function highlightAd(tooltipRef, idString)
{
	//TURN ON AUDIO
	//turnSoundOn();
	
	var browserType = navigator.appName;
	switch(browserType){
		case('Netscape'):
			tooltipRef.style.MozOpacity = .9999999;
			break;
		case('Microsoft Internet Explorer'):
			document.getElementById(idString).style.filter = "alpha(opacity=100)";
			break;
		default:
			document.getElementById(idString).style.opacity = 1;
	}
	for(var x=1;x<7;x++){
		var tempID = "cZn" + x;
		document.getElementById(tempID).style.background = tooltipHighlightColor;
	}
}

function unHighlightAd(tooltipRef, idString){
	//TURN OFF AUDIO
	//turnSoundOff();
	
	var browserType = navigator.appName;
	switch(browserType){
		case('Netscape'):
			tooltipRef.style.MozOpacity = .9;
			break;
		case('Microsoft Internet Explorer'):
			document.getElementById(idString).style.filter = "alpha(opacity=90)";
			break;
		default:
			document.getElementById(idString).style.opacity = .9;
	}
	for(var x=1;x<7;x++){
		var tempID = "cZn" + x;
		document.getElementById(tempID).style.background = tooltipBkgColor;
	}
}

function _showOrchestratorToolTip()
{
    changeStyle('tooltipBox', 'visibility', 'visible');
}

function getDisplayStringAjax(data, textStatus, xmlHttpRequest) {
    var displayString = '';
    displayString += '<div id="toolTipText">' + data + '</div>'
    return displayString;
}

function showDriverToolTip(el, driverIdentityID)
{
    var url = "resource/driver/drivercontactpopup.aspx?identityId=" + driverIdentityID;
    var displayString = MakeToolTipRequestAsync(url, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Driver Information');
}

function ShowPointToolTip(el, pointId)
{
    var url = "~/point/getpointaddresshtml.aspx";
    url = url.replace("~", webserver);
    
	var pageUrl = url + "?pointId=" + pointId;
	var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Point Details');
}

function ShowPointWithNotesToolTip(el, instructionId) 
{
    var url = "~/point/getpointaddresshtml.aspx";
    url = url.replace("~", webserver);
	var pageUrl = url + "?iID=" + instructionId;

	var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Point Details');
}

function sPWINTT(el, pointId, instructionId) {
    ShowPointWithInstructionNotesToolTip(el, pointId, instructionId);
}

function ShowPointWithInstructionNotesToolTip(el, pointId, instructionId)
{
    var url = "~/point/getpointaddresshtml.aspx";
    url = url.replace("~", webserver);
	var pageUrl = url + "?pointId=" + pointId + "&iID=" + instructionId;

	var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Point Details');
}

function ShowContactInformationToolTip(el, identityId)
{
    var url = "~/resource/driver/drivercontactpopup.aspx?identityId= " + identityId;
    url = url.replace("~", webserver);
    var displayString = MakeToolTipRequestAsync(url, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Contact Information');
}

function ShowOnTrailerReturnsToolTip(el, trailerResourceId)
{
    var url = '~/GoodsRefused/getOnTrailerGoodshtml.aspx?resourceId=' + trailerResourceId;
    url = url.replace("~", webserver);

    var displayString = MakeToolTipRequestAsync(url, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'On Trailer Returns');
}

function sONTT(el, orderID) {
    ShowOrderNotesToolTip(el, orderID);
}

function ShowOrderNotesToolTip(el, orderID) {

    var url = "~/groupage/getOrderNotesHTML.aspx";
    url = url.replace("~", webserver);
	var pageUrl = url + "?orderID=" + orderID;

    var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Order Notes');
}

function ShowToolTipSuccess(data, textStatus, xmlHttpRequest, el, x, y, header) {
    try {
        $('body').mousemove(function() { closeToolTip(); });
        showToolTip(el, header, data, x, y);
    } catch (e) { }
}

function ShowOrderCollectionDeliveryNotes(el, orderID, isCollection) 
{
    var url = "~    ";
    url = url.replace("~", webserver);
    var pageUrl = url + "?orderID=" + orderID + "&isCollection=" + isCollection;

    var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Notes');
}

function ShowCollectionDeliveryNotesForInstructions(el, startInstructionID, endInstructionID) 
{
    var url = "~/groupage/getCollectionDeliveryNotesForInstructions.aspx";
    url = url.replace("~", webserver);
    var pageUrl = url + "?sID=" + startInstructionID + "&eID=" + endInstructionID;

    var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Notes');
}

function sODTT(el, identityID) {
    ShowOrganisationDetailsToolTip(el, identityID);
}

function ShowOrganisationDetailsToolTip(el, identityID)
{
    var url = "~/organisation/getContactDetails.aspx";
    url = url.replace("~", webserver);
	var pageUrl = url + "?identityId=" + identityID;

	var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Customer Details');
}

function ShowExtrasDetailsToolTip(el, instructionID)
{
    var url = "~/Traffic/getExtraDetails.aspx";
    url = url.replace("~", webserver);
	var pageUrl = url + "?InstructionID=" + instructionID;

	var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, 'Extra Details');
}

function showAllocationHistoryToolTip(el, orderID) {
    var pageUrl = webserver + "/Traffic/getExtraDetails.aspx?oid=" + orderID;
    MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX, window.event.clientY, "Allocation History");
}

function ShowCommunicationHistory(el, pointId) {
    var url = "~/Traffic/CommunicationHistory.aspx";
    url = url.replace("~", webserver);

    var pageUrl = url + "?run=" + pointId;
    var displayString = MakeToolTipRequestAsync(pageUrl, ShowToolTipSuccess, el, window.event.clientX - 400, window.event.clientY - 150 , 'Communication History');
}

var _xhr = null;

function MakeToolTipRequestAsync(url, success, ele, x, y, header) {

    if (_xhr != null) {
        _xhr.abort();
    }

    try {
            _xhr = $.ajax(
                {
                    async: true,
                    url: url,
                    context: document.body,
                    success: function(data, textStatus, xmlHttpRequest) { success(data, textStatus, XMLHttpRequest, ele, x, y, header); },
                    cache: true,
                    type: "POST",
                    contentType: "application/x-www-form-urlencoded",
                    timeout: 4000,
                    error: function(xmlHttpRequest, textStatus, errorThrown) { failure(xmlHttpRequest, textStatus, errorThrown); }
                }
        );
    }
    catch (e) { asyncRequestPending = false; }
}

function failure(xmlHttpRequest, textStatus, errorThrown) {
}

function captureMousePosition2(x, y) {
    xMousePos = 0; // Horizontal position of the mouse on the screen
    yMousePos = 0; // Vertical position of the mouse on the screen
    xMousePosMax = 0; // Width of the page
    yMousePosMax = 0; // Height of the page

    var divPointAddress = $('div[id=toolTip]')[0];

    xMousePos = x;
    yMousePos = y;
    xMousePosMax = document.body.clientWidth;
    yMousePosMax = document.body.clientHeight;

    divPointAddress.style.left = xMousePos + 25 + "px";
    divPointAddress.style.top = yMousePos - 50 + "px";
}

function showToolTip(el, statusVar, toolTipText, x, y) {
        var toolTip = $("#toolTip");
        var toolTipInner = $("#toolTipInner");
        var content = '<h1 id="toolTipTitle">' + statusVar + '</h1>' + toolTipText;
        toolTip.css({ "display": "block", "position": "fixed" });
        toolTipInner.empty();
        toolTipInner.append(content);
        captureMousePosition2(x, y);
}

function closeToolTip() {
    var toolTip = $("#toolTip");
    var toolTipInner = $("#toolTipInner");
    $(toolTipInner).replaceWith("<div id='toolTipInner'></div>");
    toolTip.css({ "display": "none" });
    $('body').unbind('mousemove');
}
	
document.write("<div id=\"tooltipBox\" onMouseOver=\"clearAdInterval();highlightAd(this,'orchTxtTbl');\" onMouseOut=\"hideAd();unHighlightAd(this,'orchTxtTbl');\" style=\"z-index:5000;position:absolute;cursor:pointer;-moz-opacity:.9;visibility:hidden;\"></div>");