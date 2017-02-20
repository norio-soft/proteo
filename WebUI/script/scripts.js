var Nav4 = ((navigator.appName == "Netscape") && (parseInt(navigator.appVersion) == 4));
var webserver = "";

function showHideElement(idOfElementToHide, imageIdToChange, imgPath1, imgPath2) {

    if ($($get(idOfElementToHide)) != null) {
        // Show or hide content
        if ($($get(idOfElementToHide)).css('display') == "none") {
            $($get(idOfElementToHide)).css('display', '');
        } else {

            $($get(idOfElementToHide)).css('display', 'none');
        }

        // Swap the image displayed
        var img = $get(imageIdToChange);

        if (img != null) {
            if (img.src.indexOf(imgPath1) > -1) {
                img.src = "/" + imgPath2;
            }
            else {
                img.src = "/" + imgPath1;
            }
        }
    }
}

// One object tracks the current modal dialog opened from this window.
var dialogWin = new Object()

function doValidate(f){
	f.BIN_button.value=ProcessingText;
	f.BIN_button.disabled=true;
		if (cnt==0)f.submit();
	cnt++;
}

function getCookie(c_name) {
    if (document.cookie.length > 0) {
        c_start = document.cookie.indexOf(c_name + "=");
        if (c_start != -1) {
            c_start = c_start + c_name.length + 1;
            c_end = document.cookie.indexOf(";", c_start);
            if (c_end == -1) c_end = document.cookie.length;
            return unescape(document.cookie.substring(c_start, c_end));
        }
    }
}

function setCookie(c_name, value, expiredays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + expiredays);
    document.cookie = c_name + "=" + escape(value) +
            ((expiredays == null) ? "" : ";expires=" + exdate.toGMTString());

}

function openResizableDialogWithScrollbars(url, width, height)
{
	url = url.replace("~", webserver);
	
	// Keep name unique so Navigator doesn't overwrite an existing dialog.
	var name = (new Date()).getSeconds().toString();
	
	// Assemble window attributes and try to center the dialog.
	if (Nav4)
	{
		// Center on the main window.
		var left = window.screenX + 
		((window.outerWidth - width) / 2)
		var top = window.screenY + 
		((window.outerHeight - height) / 2)
		var attr = "screenX=" + left + 
		",screenY=" + top + ",resizable=yes,scrollbars=yes,width=" + 
		width + ",height=" + height
	}
	else
	{
		// The best we can do is center in screen.
		var left = (screen.width - width) / 2
		var top = (screen.height - height) / 2
		var attr = "left=" + left + ",top=" + 
		top + ",resizable=yes,scrollbars=yes,width=" + width + 
		",height=" + height
	}
	
	// Generate the dialog and make sure it has focus.
	var obj = window.open(url, name, attr)
	obj.focus()
}

function openResizableDialogWithoutScrollbars(url, width, height) {
    url = url.replace("~", webserver);

    // Keep name unique so Navigator doesn't overwrite an existing dialog.
    var name = (new Date()).getSeconds().toString();

    // Assemble window attributes and try to center the dialog.
    if (Nav4) {
        // Center on the main window.
        var left = window.screenX +
		((window.outerWidth - width) / 2)
        var top = window.screenY +
		((window.outerHeight - height) / 2)
        var attr = "screenX=" + left +
		",screenY=" + top + ",resizable=yes,scrollbars=no,width=" +
		width + ",height=" + height
    }
    else {
        // The best we can do is center in screen.
        var left = (screen.width - width) / 2
        var top = (screen.height - height) / 2
        var attr = "left=" + left + ",top=" +
		top + ",resizable=yes,scrollbars=no,width=" + width +
		",height=" + height
    }

    // Generate the dialog and make sure it has focus.
    var obj = window.open(url, name, attr)
    obj.focus()
}

function openDialogWithScrollbars(url, width, height)
{
	url = url.replace("~", webserver);
	
	// Keep name unique so Navigator doesn't overwrite an existing dialog.
	var name = (new Date()).getSeconds().toString();
	
	// Assemble window attributes and try to center the dialog.
	if (Nav4)
	{
		// Center on the main window.
		var left = window.screenX + 
		((window.outerWidth - width) / 2)
		var top = window.screenY + 
		((window.outerHeight - height) / 2)
		var attr = "screenX=" + left + 
		",screenY=" + top + ",resizable=y,scrollbars=yes,width=" + 
		width + ",height=" + height
	}
	else
	{
		// The best we can do is center in screen.
		var left = (screen.width - width) / 2
		var top = (screen.height - height) / 2
		var attr = "left=" + left + ",top=" + 
		top + ",resizable=no,scrollbars=yes,width=" + width + 
		",height=" + height
	}
	
	// Generate the dialog and make sure it has focus.
	var obj = window.open(url, name, attr)
	obj.focus()
}

function openDialog(url, width, height)
{
	url = url.replace("~", webserver);
	
	if (!dialogWin.win || (dialogWin.win && dialogWin.win.closed))
	{
		// Initialize properties of the modal dialog object.
		dialogWin.returnFunc = returnUrlFromPopUp;
		dialogWin.url = url
		dialogWin.width = width
		dialogWin.height = height
		
		// Keep name unique so Navigator doesn't overwrite an existing dialog.
		dialogWin.name = (new Date()).getSeconds().toString()
		
		// Assemble window attributes and try to center the dialog.
		if (Nav4)
		{
			// Center on the main window.
			dialogWin.left = window.screenX + 
			((window.outerWidth - dialogWin.width) / 2)
			dialogWin.top = window.screenY + 
			((window.outerHeight - dialogWin.height) / 2)
			var attr = "screenX=" + dialogWin.left + 
			",screenY=" + dialogWin.top + ",resizable=no,width=" + 
			dialogWin.width + ",height=" + dialogWin.height
		}
		else
		{
			// The best we can do is center in screen.
			dialogWin.left = (screen.width - dialogWin.width) / 2
			dialogWin.top = (screen.height - dialogWin.height) / 2
			var attr = "left=" + dialogWin.left + ",top=" + 
			dialogWin.top + ",resizable=no,width=" + dialogWin.width + 
			",height=" + dialogWin.height
		}
		
		// Generate the dialog and make sure it has focus.
		dialogWin.win=window.open(dialogWin.url, dialogWin.name, attr)
		dialogWin.win.focus()
	}
	else
	{
		dialogWin.win.focus()
		dialogWin.win.location = url;
	}
}

function RowHelper(cell, defaultValue)
{
    var disabledCheckboxHolders = cell.getElementsByTagName("span");
    if (disabledCheckboxHolders.length == 1)
    {
        if (disabledCheckboxHolders[0].getAttribute("disabled"))
        {
            // this checkbox is disabled.
            disabledCheckboxHolders[0].getElementsByTagName("input")[0].checked = defaultValue;
            return false;
        }
    }
    
    return true;
}

function RowSelectingHelper(cell)
{
    return RowHelper(cell, false);
}

function RowDeselectingHelper(cell)
{
    return RowHelper(cell, true);
}

function ShortCutKeyCapture(e) 
{
    if (event.keyCode)    
        keycode=event.keyCode;   
    else  
        keycode=event.which;   
        
    if (keycode==113 && (event.charCode ? event.charCode : 0) == 0)   
    {
        openDialogWithScrollbars('~/Organisation/clientContacts.aspx?isCalledFromHotKey=true', '600', '275');
        event.returnValue = false;
    }
}

//Used in conjunction with the RadWindowManager.
function OpenWindow(name, url)
{
    var oManager = GetRadWindowManager();
    oManager.Open(url, null);
}

function GetRadWindow()
{
    var oWindow = null;
    try {
        if (window.radWindow)
            oWindow = window.radWindow;
        else if (window.frameElement.radWindow)
            oWindow = window.frameElement.radWindow;
    }
    catch (err) { }

    return oWindow;
}

//Will reload the parent window on the child window closing.
function CloseWindow()
{
    var oWindow = GetRadWindow();
    oWindow.BrowserWindow.location.reload();
    oWindow.Close();
}

 // Array Remove - By John Resig (MIT Licensed)
   Array.prototype.remove = function(from, to) {
     var rest = this.slice((to || from) + 1 || this.length);
     this.length = from < 0 ? this.length + from : from;
     return this.push.apply(this, rest);
 };

// findComponentBySelector() is like asp.net's $find() but will match a jQuery-style selector string.
// The "context" parameter is optional and, if supplied, can be either a jQuery object or a DOM element.
function findComponentBySelector(selector, context) {
    var element = $(selector, context).get(0);
    return element ? $find(element.id) : null;
}
