// Set Netscape up to run the "captureMousePosition" function whenever
	// the mouse is moved. For Internet Explorer and Netscape 6, you can capture
	// the movement a little easier.
	//if (document.layers) { // Netscape
	//	document.captureEvents(Event.MOUSEMOVE);
	//	document.onmousemove = captureMousePosition;
	//} else if (document.all) { // Internet Explorer
	//	document.onmousemove = captureMousePosition;
	//} else if (document.getElementById) { // Netcsape 6
	//	document.onmousemove = captureMousePosition;
	//}
	// Global variables
	xMousePos = 0; // Horizontal position of the mouse on the screen
	yMousePos = 0; // Vertical position of the mouse on the screen
	xMousePosMax = 0; // Width of the page
	yMousePosMax = 0; // Height of the page
	var divPointAddress = document.getElementById('divPointAddress');
	if (divPointAddress != null)
		divPointAddress.style.display = 'none';

	function captureMousePosition(e) {
		if (document.layers) {
			// When the page scrolls in Netscape, the event's mouse position
			// reflects the absolute position on the screen. innerHight/Width
			// is the position from the top/left of the screen that the user is
			// looking at. pageX/YOffset is the amount that the user has 
			// scrolled into the page. So the values will be in relation to
			// each other as the total offsets into the page, no matter if
			// the user has scrolled or not.
			xMousePos = e.pageX;
			yMousePos = e.pageY;
			xMousePosMax = window.innerWidth+window.pageXOffset;
			yMousePosMax = window.innerHeight+window.pageYOffset;
		} else if (document.all) {
			// When the page scrolls in IE, the event's mouse position 
			// reflects the position from the top/left of the screen the 
			// user is looking at. scrollLeft/Top is the amount the user
			// has scrolled into the page. clientWidth/Height is the height/
			// width of the current page the user is looking at. So, to be
			// consistent with Netscape (above), add the scroll offsets to
			// both so we end up with an absolute value on the page, no 
			// matter if the user has scrolled or not.
			xMousePos = window.event.x+document.body.scrollLeft;
			yMousePos = window.event.y+document.body.scrollTop;
			xMousePosMax = document.body.clientWidth+document.body.scrollLeft;
			yMousePosMax = document.body.clientHeight+document.body.scrollTop;
		} else if (document.getElementById) {
			// Netscape 6 behaves the same as Netscape 4 in this regard 
			xMousePos = e.pageX;
			yMousePos = e.pageY;
			xMousePosMax = window.innerWidth+window.pageXOffset;
			yMousePosMax = window.innerHeight+window.pageYOffset;
		}

		if (divPointAddress != null && divPointAddress.style.display == '')
		{
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
	}

	function ShowPoint(url, pointId) 
	{
	    url = url.replace("~", webserver);
	    
		var txtPointAddress = document.getElementById('txtPointAddress');
		var pageUrl = url + "?pointId=" + pointId;

		var xmlRequest = new XMLHttpRequest();
		
		xmlRequest.open("POST", pageUrl, false);
		xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		try
		{
		    xmlRequest.send(null);
		}
		catch (e) { }

		var spnPointAddress = document.getElementById('spnPointAddress');
        //alert(divPointAddress);
        divPointAddress = document.getElementById('divPointAddress');
		if (spnPointAddress != null && divPointAddress != null)
		{
		 	spnPointAddress.innerHTML = xmlRequest.responseText;
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";

			$().mousemove(function(e) { yMousePos = e.pageY });
			$().mousemove(function(e) { xMousePos = e.pageX });
            
            yMousePos = yMousePos + document.body.scrollTop;
            
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
	}

	function ShowPointWithNotes(url, legPointId)
	{
	    url = url.replace("~", webserver);
	    
		var txtPointAddress = document.getElementById('txtPointAddress');
		var pageUrl = url + "?legPointId=" + legPointId;

		var xmlRequest = new XMLHttpRequest();
		
		xmlRequest.open("POST", pageUrl, false);
		xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		try
		{
		    xmlRequest.send(null);
		}
		catch(e){}
		    
		
		var spnPointAddress = document.getElementById('spnPointAddress');
        //alert(divPointAddress);
        divPointAddress = document.getElementById('divPointAddress');
		if (spnPointAddress != null && divPointAddress != null)
		{
		 	spnPointAddress.innerHTML = xmlRequest.responseText;
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";
            yMousePos = window.event.y;
            xMousePos = window.event.x; 
            
            yMousePos = yMousePos + document.body.scrollTop;
            
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
	}

	function ShowLegPoint(url) {
	    url = url.replace("~", webserver);
	    
		var txtPointAddress = document.getElementById('txtPointAddress');

		var xmlRequest = new XMLHttpRequest();
		
		xmlRequest.open("POST", url, false);
		xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		xmlRequest.send(null);
		
		var spnPointAddress = document.getElementById('spnPointAddress');
        //alert(divPointAddress);
        divPointAddress = document.getElementById('divPointAddress');
		if (spnPointAddress != null && divPointAddress != null)
		{
		 	spnPointAddress.innerHTML = xmlRequest.responseText;
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";
            yMousePos = window.event.y;
            yMousePos = yMousePos + document.body.scrollTop;
            xMousePos = window.event.x;
            
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
	}
	
	function ShowContactInformation(identityId)
	{
	    url = "~/resource/driver/drivercontactpopup.aspx?identityId= " + identityId;
	    url = url.replace("~", webserver);


	    var xmlRequest = new XMLHttpRequest();
		
		xmlRequest.open("POST", url, false);
		xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		try
		{
		    xmlRequest.send(null);
		}
		catch(e){}
		
		var spnPointAddress = document.getElementById('spnPointAddress');
        //alert(divPointAddress);
        divPointAddress = document.getElementById('divPointAddress');
		if (spnPointAddress != null && divPointAddress != null)
		{
		 	spnPointAddress.innerHTML = xmlRequest.responseText;
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";
            yMousePos = window.event.y;
            yMousePos = yMousePos + document.body.scrollTop;
            xMousePos = window.event.x;
            
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
	}
    
    function ShowOnTrailerReturns(trailerResourceId)
    {
        var url = '~/GoodsRefused/getOnTrailerGoodshtml.aspx?resourceId=' + trailerResourceId;
	    url = url.replace("~", webserver);
	    
		var txtPointAddress = document.getElementById('txtPointAddress');

		var xmlRequest = new XMLHttpRequest();
		
		xmlRequest.open("POST", url, false);
		xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
		xmlRequest.send(null);
		
		var spnPointAddress = document.getElementById('spnPointAddress');
        //alert(divPointAddress);
        divPointAddress = document.getElementById('divPointAddress');
		if (spnPointAddress != null && divPointAddress != null)
		{
		 	spnPointAddress.innerHTML = xmlRequest.responseText;
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";
            yMousePos = window.event.y;
            xMousePos = window.event.x; 
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
		}
    }
    
	function HidePoint()
	{
		if (divPointAddress != null)
			divPointAddress.style.display = 'none';
	}
	
	function ShowNote(note)
	{
        divPointAddress = document.getElementById('divPointAddress');
		if (divPointAddress != null)
		{
			spnPointAddress.innerHTML = note;
			divPointAddress.style.display = '';
			divPointAddress.style.position = "absolute";
            yMousePos = window.event.y;
            xMousePos = window.event.x; 
			divPointAddress.style.left = xMousePos + 10 + "px";
			divPointAddress.style.top = yMousePos + 8 + "px";
    	}
	}

    function HideNote()
    {
		if (divPointAddress != null)
			divPointAddress.style.display = 'none';
    }
    
    