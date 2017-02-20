
/************************************************
 *
 *	Class RadWindow
 *
 ************************************************/
function RadWindow(id)
{
	this.IsIE = (null != document.all) && (window.opera == null);
	
	this.Id = id;
	this.Width = 0;
	this.Height = 0;  
	//TO DO: other properties - color, etc,  Header text
	this.OnClientClose = null;
	//Private members
	this.ContentWindow = null;
	this.ContentTD = null;
	this.ContentWrapperTable = null;
	this.Caption = null;
	this.X = 0;
	this.Y = 0;
	this.ShowContentWhenMoving = true;
	this.borderWidth = 0;
	
	this.CanMove = true;
	this.CanResize = true;
	
	this.DragMode = "";	// "move" "size"
	
	// dialogs-related stuff
	this.IsModal = false;
	this.Container = null;
	this.Parent = null;

	this.Argument = null;
	this.ReturnValue = null;
	this.ExitCode = null;	// OK, CANCEL, etc.
	this.ZIndex = 0;
	this.AdjustPosInterval = -1;
	this.CallbackFunc = null;	// signature: function CallbackFunc() { }
	this.OnLoadFunc = null;
	this.Param = null;
	
	this.ModalSetCapture = false;
	
	this.UseRadWindow = true;
	this.Window = null;	// IHTMLWindow object - this.UseRadWindow == true
	this.InnerHTML = null;
}

RadWindow.prototype.OnLoad = function()
{
	if (this.Window && "" != this.Window.document.title)
	{
		this.SetCaption(this.Window.document.title);
	}

	if (this.OnLoadFunc)
	{
		this.OnLoadFunc();
	}
}

RadWindow.prototype.SetCapture = function(bContainerCapture)
{
	if (this.UseRadWindow)
	{
		if (null != bContainerCapture)
		{
			this.bContainerCapture = bContainerCapture;
		}
		else if (null != this.bContainerCapture)
		{
			bContainerCapture = this.bContainerCapture;
		}
		else
		{
			bContainerCapture = false;
		}

		if (this.ModalSetCapture && this.IsIE)
		{
			this.ContentWrapperTable.setCapture(bContainerCapture);
		}
	}	
}

RadWindow.prototype.ReleaseCapture = function()
{
	if (this.UseRadWindow)
	{
		if (this.ModalSetCapture && this.IsIE)
		{
			if (this.ContentWrapperTable)
				this.ContentWrapperTable.releaseCapture();
		}
	}
}

RadWindow.prototype.SetZIndex = function(zIndex)
{
	this.ZIndex = zIndex;
	if (this.ContentWrapperTable)
	{
		this.ContentWrapperTable.style.zIndex = this.ZIndex;
	}
}

RadWindow.prototype.ToggleContent = function()
{
	return;	
	if (this.UseRadWindow && this.IsIE)
	{	
		var displayStyle = "";	
		if (parseInt(this.Height) == parseInt(this.ContentWrapperTable.style.height))
		{
			this.SetHeight(0);
			displayStyle = "none";
		}
		else
		{
			this.SetHeight(this.Height);
			displayStyle = "inline";
		}
		
		this.ContentWindow.style.display = displayStyle;
	}
}

RadWindow.prototype.IsVisible = function()
{
	if (this.ContentWrapperTable)
	{
		return this.ContentWrapperTable.style.display == "block";
	}	
	return false;
}

RadWindow.prototype.ShowWindow = function(show, x, y)
{
	if (null == show)
	{
		show = true;
	}	
	var displayStyle = show ? "block" : "none";
	
	if (this.ContentWrapperTable)
	{
		this.ContentWrapperTable.style.display = displayStyle;
	}	
	
	if (null != this.ContentWindow && show)
	{
		this.ContentWindow.style.display = displayStyle;
		if (null != x && null != y)
		{
			x += 10;
			if (this.ContentWrapperTable)
			{
				this.ContentWrapperTable.style.left = x + 'px';
				this.ContentWrapperTable.style.top = y + 'px';		
			}
		}
								
		for (var i = 0; i < this.ContentWindow.childNodes.length; i++)
		{			
			try
			{
				this.ContentWindow.childNodes[i].style.display = displayStyle;												
			}
			catch (ex)
			{			
			}
		}
	}
	
	//NS
	if (!document.all)
	{	
		//this.ContentWindow.style.width = this.Width;
	}
	
	if (this.Parent)
	{
		this.Parent.OnShowWindow(this, show);
	}
}

RadWindow.prototype.Initialize2 =  function(contentElem, show, containerElem, modal, zIndex)
{
	this.Initialize(contentElem, show);
	
	this.IsModal = modal;
	this.Container = containerElem;
	
	this.SetZIndex(zIndex);
}

RadWindow.prototype.Initialize = function(contentElem, show)
{
	//Use the Id to get a reference to the ContentWindow	
	if (this.Id) 
	{
		this.ContentWrapperTable = document.getElementById("RadWindowContentWrapper" + this.Id);	 
		this.ContentWindow = document.getElementById("RadWindowContentWindow" + this.Id);	 
		this.ContentTD = document.getElementById("RadWindowContentTD" + this.Id);
		this.Caption = document.getElementById("RadWindowCaption" + this.Id);
		
		this.borderWidth = this.GetTableBorder(this.ContentWrapperTable);
						
		//Set the content of the r.a.d.window
		if (contentElem) 
		{
			this.SetContentElement(contentElem);	 	 	 	 	 
		}
		
		if (null == show)
		{
			var show = true;
		}
		
		this.ShowWindow(show);
	}
	else 
	{
		alert ("No window Id provided");
	}
};

RadWindow.prototype.SetContentElement = function (contentElem)
{
	var parentNode = contentElem.parentNode;
	if (parentNode) 
	{
		parentNode.removeChild(contentElem);		
	}
	
	while (this.ContentWindow && this.ContentWindow.childNodes.length > 0)
	{
		this.ContentWindow.removeChild(this.ContentWindow.childNodes.item(0));		
	}
	
	if (this.ContentWindow)
	{
		this.ContentWindow.appendChild(contentElem);
		this.SetContentWindowSize(contentElem.style.width, null);	
	}
}

RadWindow.prototype.SetContentWindowSize = function (width, height)
{
	this.Width = width;	
	this.Height = height;
	
	if (!document.all)
	{	//For NS only!	    
		if (this.ContentWrapperTable)
			this.ContentWrapperTable.getElementsByTagName("TABLE").item(0).style.width = width + "px";	  
	}	
};

RadWindow.prototype.SetContentVisible = function (visible)
{
//	if (document.all)
//	{
		if (this.ContentWindow)
			this.ContentWindow.style.visibility = visible ? "visible" : "hidden";
//	}
//	else
//	{
//		this.ContentWindow.childNodes[0].childNodes[0].style.display = visible ? "block" : "none";	 //visible ? "visible" : "hidden";
//	}
};

RadWindow.prototype.Close =  function(exitCode)
{	
	if (null != this.OnClientClosing 
		&& (this.OnClientClosing(exitCode) == false))
	{
		return;
	}

	this.ShowWindow(false);	
		
	this.ExitCode = exitCode;
	
	if (this.AdjustPosInterval > -1)
	{
		window.clearInterval(this.AdjustPosInterval);
		this.AdjustPosInterval = -1;
	}
	
	if (this.IsModal)
		this.ReleaseCapture();	
		
	try
	{
		if (this.CallbackFunc)
			this.CallbackFunc(this.ReturnValue, this.Param);
	}
	catch (ex)
	{
	}
	
	if (this.Parent)
		this.Parent.DestroyWindow(this);
		
	if (!this.UseRadWindow && this.Window)
		this.Window.close();
};
			
RadWindow.prototype.ToggleCanMove =  function(oDiv)
{
	if (!this.UseRadWindow)
		return;
	
	this.CanMove = !this.CanMove;
	
	oDiv.className = this.CanMove ? "RadERadWindowButtonPinOff" : "RadERadWindowButtonPinOn";
	
	if (!this.CanMove)
	{
		if (this.IsIE)
		{
			this.TopOffset = parseInt(this.ContentWrapperTable.style.top) - RadGetScrollTop(document);
			this.StartUpdatePosTimer(100);
		}
		else
		{		
			this.ContentWrapperTable.style.position = "fixed";			
		}
	}
	else
	{
		if (this.IsIE)
		{
			window.clearInterval(this.AdjustPosInterval);		
			this.TopOffset = null;
		}
		else
		{
			this.ContentWrapperTable.style.position = "absolute";
		}
	}
}

RadWindow.prototype.StartUpdatePosTimer = function(iInterval)
{
	if (!this.UseRadWindow)
		return;
		
	this.AdjustPosInterval = window.setInterval("UpdateWindowPos('" + this.Id + "')", iInterval);
}

function UpdateWindowPos(wndId)
{
	var wnd = GetRadWindowManager().LookupWindow(wndId);
	if (wnd)	
		wnd.SetPosition();
}

RadWindow.prototype.CanDrag = function()
{
	if (!this.UseRadWindow)
		return true;
		
	return ("move" == this.DragMode && this.CanMove)
			|| ("size" == this.DragMode && this.CanResize);
}

RadWindow.prototype.OnMouseOver = function(e)
{	
	switch (e.srcElement.id)
	{
		case "BorderBottom":
			break;
			
		case "CornerBottomRight":
			//e.srcElement.className="RadETableWrapperFooterRightOver";
			e.srcElement.style.cursor = "se-resize";			
			break;
			
		default:
			return;
	}
}

RadWindow.prototype.OnMouseOut = function(e)
{	
	switch (e.srcElement.id)
	{
		case "BorderBottom":
			break;
			
		case "CornerBottomRight":
			e.srcElement.className="RadETableWrapperFooterRight";
			break;
			
		default:
			return;
	}
}

RadWindow.prototype.OnDragStart = function(e)
{   
	this.SetActive(true);	//RadWindowActiveWindow = this; 
	
	if (!this.CanDrag())
		return;
	
	this.X = (e.offsetX == null) ? e.layerX : e.offsetX;
	this.Y = (e.offsetY == null) ? e.layerY : e.offsetY;    
	
	MousePosX = e.clientX;
	MousePosY = e.clientY;
	
	this.SetContentVisible(this.ShowContentWhenMoving);
	RadWindowDragStart();
};

RadWindow.prototype.SetActive = function(activate)
{		
	if (!this.UseRadWindow)
		return;
			
	if (activate)
	{
		if (null != RadWindowActiveWindow && RadWindowActiveWindow != this)
			RadWindowActiveWindow.SetActive(false);
	
		RadWindowActiveWindow = this;
		
		if (this.Parent)
			this.Parent.ActivateWindow(this);
	}
	else
	{
		if (this == RadWindowActiveWindow)
			RadWindowActiveWindow = null;
	}
}

RadWindow.prototype.HitTest = function(x, y)
{
	var left = parseInt(this.ContentWrapperTable.style.left);
	if (isNaN(left))
		return false;
		
	var top = parseInt(this.ContentWrapperTable.style.top);
	if (isNaN(top))
		return false;
		
	return	left <= x 
			&& x <= (left + this.Width)
			&& top <= y 
			&& y <= (top + this.Height);
}

RadWindow.prototype.SetPosition = function(left, top)
{
	if (!this.UseRadWindow)
	{
		if (this.Window)
		{
			this.Window.dialogLeft = left;
			this.Window.dialogTop = top;
		}
	}
	else
	{
		if (!left)
			left = this.ContentWrapperTable.style.left;
			
		if (!top)
		{
			if (this.TopOffset != null)
				top = parseInt(this.TopOffset) + RadGetScrollTop(document);
			else		
				top = this.ContentWrapperTable.style.top;
		}
			
		left = parseInt(left);
		top = parseInt(top);
		
		if (!isNaN(left) && !isNaN(top))
		{		
			if (left <= 0)
				left = 0;
				
			if (top <= 0)
				top = 0;
		
			this.ContentWrapperTable.style.left = left + 'px';
			this.ContentWrapperTable.style.top = top + 'px';
		}
	}
}

RadWindow.prototype.GetTableBorder = function(table)
{
	var borderWidth = 0;	
	if (null != table)
	{
		var strBorderWidth = table.style.borderWidth;
		borderWidth = parseInt(strBorderWidth);
		if (isNaN(borderWidth))
		{		
			borderWidth = 0;		
			if ("" != table.className)
			{
				var rule = this.GetCssClass(table.className);
				if (null != rule)
				{
					strBorderWidth = rule.style.borderWidth;
					borderWidth = parseInt(strBorderWidth);
					if (isNaN(borderWidth))
					{		
						borderWidth = 0;
					}
				}
			}
		}
	}
	return borderWidth;
}

RadWindow.prototype.GetCssClass = function(className)
{
	try
	{
		for (var i = 0; i < document.styleSheets.length; i++)
		{
			try
			{
				var cssHref = document.styleSheets[i].href;
		
				var arrRules = null;				
				if (this.IsIE)
					arrRules = document.styleSheets[i].rules;
				else
					arrRules = document.styleSheets[i].cssRules;
					
				for (var j = 0; j < arrRules.length; j++)
				{
					try
					{	
						var rule = arrRules[j];
						if (rule.selectorText == className
							|| rule.selectorText == "." + className)
						{
							return rule;
						}
					}
					catch (ex)
					{
					}
				}
			}
			catch (ex)
			{
			}
		}
	}
	catch(ex)
	{
	}
	
	return null;
}

RadWindow.prototype.GetWidth = function()
{	
	var width = 0;
	if (!this.UseRadWindow)
	{
		if (this.Window)
			width = this.Window.dialogWidth;
	}
	else
	{
		if (this.IsIE)
		{	
			width = parseInt(this.ContentWrapperTable.clientWidth);									
		}
		else
		{
			width = parseInt(this.ContentWrapperTable.scrollWidth);			
		}
		
		if (isNaN(width))
		{
			width = 0;
		}
		
		if (width > 0)
		{
			if (this.IsIE)
				width += 2 * this.borderWidth;
			else
				width -= 2 * this.borderWidth;
		}
	}
	return width;
}

RadWindow.prototype.SetWidth = function(width)
{
	width = parseInt(width);
	if (isNaN(width))
		return;		
	
	if (!this.UseRadWindow)
	{		
		if (this.Window)
		{		
			if (this.Window.dialogWidth)
			{				
				this.Window.dialogTop = this.Window.screenTop - 30;
				this.Window.dialogLeft = this.Window.screenLeft - 4;
								
				this.Window.dialogWidth = width + "px";
			}
			else
			{
				this.Window.outerWidth = width;
			}
		}
	}
	else
	{
		if (this.IsIE)
		{
			this.ContentWrapperTable.style.width = width + "px";
		}
		else
		{			
			this.ContentWrapperTable.style.width = (width + 2 * this.borderWidth) + "px";
			this.ContentTD.style.width = (width - 2 * this.borderWidth) + "px";
			
			this.ContentWindow.style.width = this.ContentTD.style.width;
			this.ContentWindow.style.overflow = "hidden";
		}
	}
}

RadWindow.prototype.GetHeight = function()
{
	var height = 0;
	if (!this.UseRadWindow)
	{
		if (this.Window)
			height = this.Window.dialogHeight;
	}
	else
	{
		if (this.IsIE)
		{
			height = parseInt(this.ContentWrapperTable.clientHeight);
			if (isNaN(height))
				height = 0;
		
			if (height > 0)
			{
				height += 2 * this.borderWidth;
			}
		}
		else
		{
			height = this.ContentWrapperTable.scrollHeight;	
		}		
	}		
	return height;
}

RadWindow.prototype.SetHeight = function(height)
{
	height = parseInt(height);
	if (isNaN(height))
		return;
	
	if (!this.UseRadWindow)
	{
		if (this.Window)
		{
			if (this.Window.dialogWidth)
			{
				this.Window.dialogTop = this.Window.screenTop - 30;
				this.Window.dialogLeft = this.Window.screenLeft - 4;
				
				this.Window.dialogHeight = height + "px";
			}
			else
			{
				this.Window.outerHeight = height;
			}
		}
	}
	else
	{
		if (this.IsIE)
		{		
			this.ContentWrapperTable.style.height = height + "px";
			var iframes = this.ContentWrapperTable.getElementsByTagName("IFRAME");			
			if (iframes && iframes.length > 0)
			{
				var dlgHeight = (height - 50);
				if (dlgHeight > 0)
				{
					iframes.item(0).style.height = dlgHeight + "px";
				}
			}
		}
		else
		{
			var totalHeight = this.borderWidth * 2;			
			for (var i = 0; i < this.ContentWrapperTable.rows.length; i++)
			{
				if (this.ContentTD.parentNode.rowIndex == i)
					continue;				
				var rowHeight = parseInt(this.ContentWrapperTable.rows[i].scrollHeight);			
				if (!isNaN(rowHeight)) 
					totalHeight += parseInt(rowHeight);

			}
						
			this.ContentTD.height = (height - totalHeight) + "px";			
			
			this.ContentWindow.style.height = (height - totalHeight) + "px";
			this.ContentWindow.style.overflow = "hidden";
			
			this.ContentWrapperTable.style.height = height + "px";			
		}
	}
}

RadWindow.prototype.SetSize = function(width, height)
{
	this.SetWidth(width);
	this.SetHeight(height);
	
	if (width > 0)
		this.Width = width;
		
	if (height > 0)
		this.Height = height;
}

RadWindow.prototype.SetCaption = function(caption)
{
	if (this.Caption)
	{
		this.Caption.innerHTML = caption;
	}
}

RadWindow.prototype.GetCaption = function()
{
	if (this.Caption)
	{
		return this.Caption.innerHTML;
	}
	else
	{
		return "";
	}
}

//-------------------------------------MOVEMENT RELATED---------------------------------//
var RadWindowActiveWindow = null; //Points to the active window 
var RadWindowActiveWindowSpan = null;
var MousePosX = 0;
var MousePosY = 0;

/* Attach Event Handlers */
function RadWindowDragStart()
{   
	if (!RadWindowActiveWindow.CanDrag())
		return;
		
	if (document.all && document.body.attachEvent) 
	{	
		document.body.setCapture();
		document.body.attachEvent ("onmousemove", RadWindowDrag);
		document.body.attachEvent ("onmouseup", RadWindowDragEnd);
	} 
	else if (document.addEventListener)
	{
		document.addEventListener("mousemove", RadWindowDrag, false);
		document.addEventListener("mouseup", RadWindowDragEnd, false);			
	}
	
	RadWindowInitializeDrag(RadWindowActiveWindow);
}

/*Detach Event Handlers*/
function RadWindowDragEnd()
{	
	if (document.all && document.body.detachEvent)
	{
		document.body.detachEvent ("onmousemove", RadWindowDrag);
		document.body.detachEvent ("onmouseup", RadWindowDragEnd);
		
		document.body.releaseCapture();
	}
	else if (document.removeEventListener)
	{  
		document.removeEventListener("mousemove", RadWindowDrag, false);
		document.removeEventListener("mouseup", RadWindowDragEnd, false);		
	}
	
	if (RadWindowActiveWindow.IsModal)
		RadWindowActiveWindow.SetCapture();
	
	RadWindowUnInitializeDrag(RadWindowActiveWindow);	
	RadWindowActiveWindow.SetContentVisible(true);
}

function RadWindowDrag(e)
{	
	if (RadWindowActiveWindow.CanDrag())
	{
		switch (RadWindowActiveWindow.DragMode)
		{
			case "move":
				RadWindowMove(e);							
				break;
		
			case "size":
				RadWindowSize(e);
				break;		
		}
	}
	e.cancelBubble = true;
	
	MousePosX = e.clientX;
	MousePosY = e.clientY;
	
	return false;
}

function RadWindowInitializeDrag(targetWindow)
{
	if (!targetWindow)
		return;

	if (!RadWindowActiveWindowSpan)
	{
		RadWindowActiveWindowSpan = document.createElement("SPAN");
		RadWindowActiveWindowSpan.className = "RadETableWrapperResizeSpan";
				
		RadWindowActiveWindowSpan.style.position = "absolute";	
		RadWindowActiveWindowSpan.style.zIndex = 50000;
		
		document.body.appendChild(RadWindowActiveWindowSpan);
	}
	
	RadWindowActiveWindowSpan.style.visibility = "visible";
	
	RadWindowActiveWindowSpan.style.top = targetWindow.ContentWrapperTable.style.top;
	RadWindowActiveWindowSpan.style.left = targetWindow.ContentWrapperTable.style.left;
	RadWindowActiveWindowSpan.style.width = targetWindow.GetWidth();
	RadWindowActiveWindowSpan.style.height = targetWindow.GetHeight();
	
	switch (targetWindow.DragMode)
	{
		case "move":
			RadWindowActiveWindowSpan.style.cursor = "default";
			break;
	
		case "size":
			RadWindowActiveWindowSpan.style.cursor = "se-resize";
			break;		
	}
}

function RadWindowUnInitializeDrag(targetWindow)
{
	if (RadWindowActiveWindowSpan)
		RadWindowActiveWindowSpan.style.visibility = "hidden";

	targetWindow.SetPosition(RadWindowActiveWindowSpan.style.left, RadWindowActiveWindowSpan.style.top);
	targetWindow.SetSize(RadWindowActiveWindowSpan.style.width, RadWindowActiveWindowSpan.style.height);
}

function RadWindowMove(e)
{	
	var RadWindowX = RadWindowActiveWindow.X;
	var RadWindowY = RadWindowActiveWindow.Y;

	var el = RadWindowActiveWindowSpan;	//RadWindowActiveWindow.ContentWrapperTable;   

	var left = 0;
	var top = 0;

	if (document.all)
	{   
		left = e.clientX * 1 + RadGetScrollLeft(document) - RadWindowX;
		top = e.clientY * 1 + RadGetScrollTop(document) - RadWindowY;
	}
	else
	{       
		left = e.pageX * 1 - RadWindowX;
		top = e.pageY * 1 - RadWindowY;
	}
	
	if (left < 0)
	{
		left = 0;
	}
		
	if (top < 0)
	{
		top = 0;
	}
	
	el.style.left = left + "px";
	el.style.top = top + "px";	
}

var minWidth = 155;
var minHeight = 43;

function RadWindowSize(e)
{	
	var offsetX = e.clientX - MousePosX;
	var offsetY = e.clientY - MousePosY;	
	
	var width = parseInt(RadWindowActiveWindowSpan.style.width);
	var height = parseInt(RadWindowActiveWindowSpan.style.height);
	
	width += offsetX;
	height += offsetY;
	
	if (width < minWidth)
		width = minWidth;
		
	if (height < minHeight)
		height = minHeight;

	RadWindowActiveWindowSpan.style.width = width + "px";
	RadWindowActiveWindowSpan.style.height = height + "px";
}	


/***********************************************
 *
 *	RadWindowManager
 *
 ***********************************************/
function GetRadWindowManager()
{
	/* SINGLETON */	
	var topWindow = GetTopWindow();	
	if (!topWindow.radWindowManager)
	{
		topWindow.radWindowManager = new RadWindowManager();
	}
	return topWindow.radWindowManager;
}

function GetTopWindow()
{
	var topWindow = null;

	var argumentsContainParentWindow = false;
	try
	{
		if (window.dialogArguments.parentWindow && argumentsContainParentWindow)
		{
			argumentsContainParentWindow = true;
		}
	}
	catch(ex)
	{
		argumentsContainParentWindow = false;
	}
	if (window.dialogArguments != null && argumentsContainParentWindow) 
	{
		topWindow = window.dialogArguments.parentWindow;	
	}
	else if (window.opener && !document.all && window.isRadWindow)
	{	// NS
		topWindow = opener;
	}	
	else
	{
		topWindow = window;	
	}

	var stopLoop = false;
	while (topWindow.parent && !stopLoop)
	{
		try
		{	
			topWindowTagName = topWindow.parent.tagName.toUpperCase()
		}
		catch (exception)
		{
			topWindowTagName = "";
		}
			
		if (topWindow.parent == topWindow)
		{	
			break;
		}

		try
		{
			/*This check is needed because of the Access denied error when cross-site scripting*/
			if (topWindow.parent.document.domain != window.document.domain)
			{
				break;
			}
		}
		catch(exc)
		{
			stopLoop = true;
			continue;
		}

		try
		{
			if (topWindow.frameElement != null 
				&& (topWindow.frameElement.tagName != "IFRAME" 
					|| topWindow.frameElement.name != "RadWindowContent"))
			{
				break;
			}
		}
		catch(exc)
		{
			alert('in the Exception!');
			stopLoop = true;
		}

		topWindow = topWindow.parent;		
	}		
	return topWindow;
}
 
function RadWindowManager()
{
	this.ChildWindows = new Array();
	this.ActiveWindow = null;
	this.TopWindowZIndex = 10001;
	
	this.ContainerPool = new Array();
	this.IsIE = (null != document.all) && (window.opera == null);
	
	this.OverImage = null;
	
	document.body.onfocus = Document_OnFocus;
	
	if (this.IsIE && document.body.attachEvent)
	{
		document.body.attachEvent("onkeydown", Document_OnKeyDown);
	}
	else if (document.body.addEventListener)
	{
		document.body.addEventListener("keydown", Document_OnKeyDown, false);
	}
		
	if (this.IsIE && !this.OverImage)
	{
		this.OverImage = document.getElementById("OVER_IMG");
	}
}

function Document_OnFocus(e)
{
	if (!e)
	{
		e = window.event;
	}
	GetRadWindowManager().ActivateWindow();
}

function Document_OnKeyDown(e)
{
	if (!e)
	{
		e = window.event;
	}	
	return GetRadWindowManager().OnKeyDown(e);
}

function RadWindowInfo()
{
	this.IsIE = (null != document.all);
	this.ID = null;
	this.Url = "";
	this.InnerHtml = "";
	this.InnerObject = null;
	this.Width = 300;
	this.Height = 200;
	this.Caption = "";
	this.IsVisible = true;
	this.Argument = null;
	this.CallbackFunc = null;
	this.OnLoadFunc = null;
	this.Param = null; // callback func param
	this.Resizable = true;
	this.Movable = true;
	this.CloseHide = false; // true - close X hides the window; otherwise close it
	this.UseClassicDialogs = true;
	this.BlankIFrameLocation = "";
}

RadWindowManager.prototype.ShowModalWindow = function(radWindowInfo)
{
	var wnd = this.CreateWindow(true, radWindowInfo);
	return wnd;
}

RadWindowManager.prototype.ShowModallessWindow = function(radWindowInfo)
{
	var wnd = this.CreateWindow(false, radWindowInfo);
	return wnd;
}

/////////////////////////////////////////////////
// WINDOWS OPERATIONS
RadWindowManager.prototype.CreateWindow = function(bIsModal, radWindowInfo)	
{
	if (!radWindowInfo)
		return null;
		
	/********* THIS CODE MAKES MOZILLA USE REGULAR WINDOWS INSTEAD OF RAD WINDOW *********/
	radWindowInfo.UseClassicDialogs = (this.IsIE && radWindowInfo.UseClassicDialogs)
										|| (!this.IsIE && ((null != radWindowInfo.Url && "" != radWindowInfo.Url)
														|| (null != radWindowInfo.InnerHtml && "" != radWindowInfo.InnerHtml && radWindowInfo.UseClassicDialogs)));
	
	var id = "";		
	if (!radWindowInfo.ID || radWindowInfo.ID == "")
	{
		id = this.ChildWindows.length;
	}
	else
	{
		id = radWindowInfo.ID;
	}
	
	var caption = "";
	if (null == radWindowInfo.Caption)
	{
		caption = "[" + id + "]";
	}
	else
	{
		caption = radWindowInfo.Caption;
	}

	var radWindow = this.GetWindow(id);	
		
	radWindow.Argument = radWindowInfo.Argument;
	radWindow.Width = radWindowInfo.Width;
	radWindow.Height = radWindowInfo.Height;
	radWindow.CallbackFunc = radWindowInfo.CallbackFunc;
	radWindow.Param = radWindowInfo.Param;
	radWindow.CanResize = radWindowInfo.Resizable;
	radWindow.CanMove = radWindowInfo.Movable;
	radWindow.OnLoadFunc = radWindowInfo.OnLoadFunc;
	radWindow.UseRadWindow = !radWindowInfo.UseClassicDialogs;
	
	if (radWindowInfo.UseClassicDialogs && this.IsIE)
	{	
		var features = "status:no;"
						+ "resizable:yes;"
						+ "center:yes;"
						+ "help:no;"
						+ "minimize:no;"
						+ "maximize:no;"
						+ "scroll:no;"
						+ "border:thin;"
						+ "statusbar:no;"
						+ "dialogWidth:" + radWindowInfo.Width + "px;"
						+ "dialogHeight:" + radWindowInfo.Height + "px";
		
		if (radWindowInfo.InnerHtml && radWindowInfo.InnerHtml != "")
		{
			radWindow.InnerHTML = radWindowInfo.InnerHtml;
		}
	
		if (!radWindowInfo.Url || "" == radWindowInfo.Url)
			radWindowInfo.Url = "javascript:''";		
		
		var dialogArguments = {
			parentWindow : window
			, radWindow  : radWindow
			,IsRadWindowArgs : true  //TEKI -  A problem with mixing IE dialogAruments with our own!
		};
		
		window.showModalDialog(radWindowInfo.Url, dialogArguments, features);
	}
	else if (radWindowInfo.UseClassicDialogs && !this.IsIE)
	{
		if (!radWindowInfo.Url || "" == radWindowInfo.Url)
			radWindowInfo.Url = "javascript:''";		
		window.childRadWindow = radWindow;
		radWindow.Window = window.open(radWindowInfo.Url
							, "_blank"			
							, "status=no,toolbar=no,location=no,resizable=yes,menubar=no,width=" + radWindowInfo.Width + ",height=" + radWindowInfo.Height + ",modal=yes");
	}
	else if (!radWindowInfo.UseClassicDialogs)
	{	
		var container = null;
		if (this.ContainerPool.length > 0)
		{
			container = this.ContainerPool.pop();
		}
		else
		{
			container = document.createElement("SPAN");	
			document.body.appendChild(container);
		}	
		var oHtml =  this.BuildWrapperTableHtml(id
								, radWindowInfo.Width
								, radWindowInfo.Height
								, caption
								, bIsModal
								, radWindowInfo.CloseHide
								, radWindowInfo.BlankIFrameLocation);
	
		container.innerHTML = oHtml;		
		var contentElem = (null != radWindowInfo.InnerObject) 
						? radWindowInfo.InnerObject 
							: document.getElementById("RadWindowContentFrame" + id);
		radWindow.Initialize2(contentElem, true, container, bIsModal, ++this.TopWindowZIndex);					
		var frm = document.getElementById("RadWindowContentFrame" + id);		
		/*** NS toolbar when toolOnPage = false and content element is DIV ***/
		radWindow.Window = null != frm ? frm.contentWindow : null;	
		if (radWindowInfo.InnerHtml && radWindowInfo.InnerHtml != "")
		{	
			var doc = frm.contentWindow.document;
			
			doc.open();
			doc.write(radWindowInfo.InnerHtml);
			doc.close();
		}
		else if (radWindowInfo.Url && radWindowInfo.Url != "")		
		{	
			frm.src = radWindowInfo.Url;
		}
			
		if (bIsModal)
		{
			this.SetCapture(radWindow, false);
		}
		var theVisibleWidth = Math.min(window.screen.width, document.body.clientWidth);
		var theVisibleHeight = (document.documentElement && document.documentElement.clientHeight)
							? document.documentElement.clientHeight
							: Math.min(window.screen.height, document.body.clientHeight);
		
		var x = RadGetScrollLeft(document) + (theVisibleWidth - parseInt(radWindowInfo.Width)) / 2;
		var y = RadGetScrollTop(document) + (theVisibleHeight - parseInt(radWindowInfo.Height)) / 2;

		if (null == radWindowInfo.IsVisible)
		{
			radWindowInfo.IsVisible = false;
		}
		
		radWindow.SetSize(radWindowInfo.Width, radWindowInfo.Height);
		radWindow.ShowWindow(radWindowInfo.IsVisible, x, y);
	}
	
	return radWindow;
}

RadWindowManager.prototype.DestroyWindow = function(childWindow)
{		
	var nextWndToActivate = this.GetPrevWindow(childWindow.Id);
	
	this.UnregisterChild(childWindow);			
	
	if (nextWndToActivate != childWindow)
	
	{
		this.ActivateWindow(nextWndToActivate);
	}
	
	eval(this.GetWindowVarName(childWindow.Id) + " = null;");
	
	if (childWindow.Container)
	{
		this.ContainerPool.push(childWindow.Container);
	}
}

RadWindowManager.prototype.GetPrevWindow = function(id)
{
	var bNeedPrev = false;
	var retWnd = null;
	for (var i = this.ChildWindows.length - 1; i >= 0; i--)
	{
		var wnd = this.ChildWindows[i];
		if (wnd && wnd.Id == id)
		{
			bNeedPrev = true;
		}
		else if (wnd && bNeedPrev)
		{
			return wnd;
		}
		else if (null == retWnd)
		{
			retWnd = wnd;
		}
	}
	return retWnd;
}

RadWindowManager.prototype.CloseWindow = function(id)
{
	var wnd = this.LookupWindow(id);
	if (wnd)
	{
		wnd.Close();
	}
}

RadWindowManager.prototype.ActivateWindow = function(radWindow)
{	
	if (!radWindow) 
	{
		radWindow = this.ActiveWindow;
	}
	if (radWindow)
	{
		if (radWindow.IsModal)
		{
			this.SetCapture(radWindow, false);
		}
		
		if (radWindow != this.ActiveWindow)
		{
			if (this.ActiveWindow)
			{
				this.ActiveWindow.SetZIndex(this.TopWindowZIndex - 1);
			}	
			radWindow.SetZIndex(this.TopWindowZIndex);
		
			this.ActiveWindow = radWindow;	
		}
		
		if (radWindow.IsModal)
		{
			this.ShowOverImage(radWindow, true);
		}
	}
}

RadWindowManager.prototype.RegisterChild = function(childWindow)
{
	this.ChildWindows[this.ChildWindows.length] = childWindow;
}

RadWindowManager.prototype.UnregisterChild = function(childWindow)
{
	for (var i = 0; i < this.ChildWindows.length; i++)
	{
		var wnd = this.ChildWindows[i];
		if (wnd == childWindow)
		{
			this.ChildWindows[i] = null;
			return;
		}
	}
}

RadWindowManager.prototype.SetCapture = function(childWindow, bContainerCapture)
{
	try
	{
		if (childWindow)
		{
			childWindow.SetCapture(bContainerCapture);
		}
	}
	catch (ex)
	{
	}
}

RadWindowManager.prototype.LookupWindow = function(id)
{
	for (var i = 0; i < this.ChildWindows.length; i++)
	{	
		var wnd = this.ChildWindows[i];		
		if (wnd && id == wnd.Id)
		{
			return wnd;
		}
	}
	return null; //this.ChildWindows[id];
}

RadWindowManager.prototype.LookupRadWindowByBrowserWindowRef = function(browserWindow)
{
	for (var i = 0; i < this.ChildWindows.length; i++)
	{
		var radWindow = this.ChildWindows[i];		
		if (null != radWindow && browserWindow == radWindow.Window)
		{
			return radWindow;
		}
	}
	return null;
}

RadWindowManager.prototype.GetCurrentRadWindow = function(browserWindow)
{	
	if (browserWindow.dialogArguments && (true == browserWindow.dialogArguments.IsRadWindowArgs))//TEKI - A problem with mixing IE dialogAruments with our own!
	{	
		return browserWindow.dialogArguments.radWindow;
	}
	else if (browserWindow.opener != null && browserWindow.opener.childRadWindow != null)
	{
		return browserWindow.opener.childRadWindow;
	}
	else
	{
		var oLast = this.LookupRadWindowByBrowserWindowRef(browserWindow);
		return oLast;
	}
}

// If already exists a window with same id - returns it; 
// Otherwise creates a new window and returns it
RadWindowManager.prototype.GetWindow = function(id)
{
	var wnd = this.LookupWindow(id);
	if (!wnd)
	{
		var varName = this.GetWindowVarName(id);
		eval(varName + " = new RadWindow('" + id + "');");		
		wnd = eval(varName);
		wnd.Parent = this;
		
		this.RegisterChild(wnd);
	}	
	return wnd;
}

RadWindowManager.prototype.GetWindowVarName = function(id)
{
	return "window.radWindow_" + id;
}

RadWindowManager.prototype.GetWindowFromPos = function(x, y)
{
	var wnd = null;
	for (var i = 0; i < this.ChildWindows; i++)
	{
		var childWnd = this.ChildWindows[i];
		if (childWnd && childWnd.HitText(x, y))
		{
			if (!wnd || wnd.ZIndex < childWnd.ZIndex)
			{
				wnd = childWnd;
			}
		}
	}	
	return wnd;
}

RadWindowManager.prototype.OnShowWindow = function(childWindow, visible)
{
	if (visible)
	{
		this.ActiveWindow = childWindow;
	}
	else
	{
		if (this.ActiveWindow == childWindow)
		{
			this.ActiveWindow = null;
		}
	}
	
	if (childWindow.IsModal)
	{
		this.ShowOverImage(childWindow, visible);
	}
}

RadWindowManager.prototype.OnKeyDown = function(evt)
{
	switch (evt.keyCode)
	{
		case 115:	//F4
			if (evt.altKey && this.ActiveWindow)
			{
				//this.ActiveWindow.Close();
			}
			else if (evt.ctrlKey && this.ActiveWindow)
			{
				//alert("CTRL+F4");
			}			
			evt.keyCode = 8;
			break;
	
		case 27: //ESC
			if (this.ActiveWindow)
			{
				this.ActiveWindow.Close();
			}	
			break;
		
		default:
			return;
	}
	
	evt.cancelBubble = true;
	evt.returnValue = false;	
}

RadWindowManager.prototype.BuildWrapperTableHtml = function(id, width, height, caption, bIsModal, bHide, blankIFrameLocation)	//url, innerHtml, 
{
	//TEKI - RE5-2838 javascript:'' causes in Moz the Javascript console to appear sometimes
	//var url = ((null != blankIFrameLocation && "" != blankIFrameLocation) ? blankIFrameLocation : "javascript:'';");
	var url = document.all ? "javascript:'';" : "";
	
	var closeFunc = bHide ? "ShowWindow(false)" : "Close()";
	
	var html = "";		
	html +=	"		<table border='0' id='RadWindowContentWrapper" + id + "' class='RadETableWrapper' style='display:block; z-index:1000; position:absolute;' cellspacing='0' cellpadding='0' width='" + width + "px' height='" + height + "px'>\n"//width='" + width + "px' height='" + height + "px'
			+ "			<tr>\n"
			+ "				<td width='1' class='RadETableWrapperHeaderLeft' nowrap></td>\n"
			+ "				<td width='100%' class='RadETableWrapperHeaderCenter' nowrap='true' ondblclick='radWindow_" + id + ".ToggleContent();' onmousedown='radWindow_" + id + ".DragMode=\"move\"; return radWindow_" + id + ".OnDragStart(event);' onselectstart='return false;'>\n"
			+ "					<span id='RadWindowCaption" + id + "' onselectstart='return false;' class='RadERadWindowHeader'>" + caption + "</span>\n"
			+ "				</td>\n";
			
	
	if (!bIsModal)		
	{
		html	+= "		<td width='1' class='RadETableWrapperHeaderCenter' nowrap>\n"
				+ "					<span class='RadERadWindowButtonPinOff' id='ButtonPin' onclick='radWindow_" + id + ".ToggleCanMove(this)'>&nbsp;</span>"
				+ "			</td>\n";						
	}
	
	html += "				<td width='1' class='RadETableWrapperHeaderCenter' nowrap>\n"
			+ "			<span class='RadERadWindowButtonClose' id='ButtonClose' onclick='radWindow_" + id + "." + closeFunc + "'>&nbsp;</span>\n"
			+ "				</td>\n"
			+ "				<td width='1' class='RadETableWrapperHeaderRight' nowrap></td>\n"
			+ "			</tr>\n"
			+ "			<tr>\n"
			+ "				<td id='RadWindowContentTD" + id + "' colspan='5'>\n"
			+ "					<table border='0' width='100%' height='100%' cellspacing='0' cellpadding='0'>\n"
			+ "						<tr>\n"
			+ "							<td width='1' class='RadETableWrapperBodyLeft' nowrap></td>\n"
			+ "							<td width='100%' class='RadETableWrapperBodyCenter' align='left' valign='top' onselectstart='return false;'>\n"
			+ "								<span id='RadWindowContentWindow" + id + "' align='center' style='width:100%;height:100%;' >\n"
			+ "									<iframe name='RadWindowContent' frameborder='0px' style='border:0px solid green' id='RadWindowContentFrame" + id + "' src='" + url + "' scrolling='no' border='no' width='100%' height='100%' ></iframe>\n"
			+ "								</span>\n"
			+ "							</td>\n"
			+ "							<td width='1' class='RadETableWrapperBodyRight' nowrap></td>\n"
			+ "						</tr>\n"
			+ "					</table>\n"
			+ "				</td>\n"
			+ "			</tr>\n"
			+ "			<tr>\n"
			+ "				<td colspan='5' width='100%' height='1' >\n"
			+ "					<table border='0' width='100%' height='1' cellspacing='0' cellpadding='0'>\n"			
			+ "						<tr>\n"
			+ "							<td width='1' class='RadETableWrapperFooterLeft' nowrap>&nbsp;</td>\n"
			+ "							<td colspan='3' id='BorderBottom' width='100%' class='RadETableWrapperFooterCenter' nowrap>&nbsp;</td>		\n"
			+ "							<td width='1' id='CornerBottomRight' class='RadETableWrapperFooterRight' onmousedown='radWindow_" + id + ".DragMode=\"size\"; return radWindow_" + id + ".OnDragStart(event);' onselectstart='return false;' nowrap>&nbsp;</td>\n"
			+ "						</tr>\n"
			+ "					</table>\n"
			+ "				</td>\n"
			+ "			</tr>\n"
			+ "		</table>\n";			
	return html;
}

RadWindowManager.prototype.ShowOverImage = function(wnd, visible)
{
	if (this.OverImage)
	{	
		if (wnd && visible)
		{	
			this.OverImage.style.position = "absolute";
			this.OverImage.style.left = 0;
			this.OverImage.style.top = 0;			
			this.OverImage.style.width = Math.max(0, parseInt(document.body.clientWidth) - 2);
			this.OverImage.style.height = Math.max(0, parseInt(document.body.clientHeight) - 2); 			
			this.OverImage.style.zIndex = wnd.ZIndex;
			this.OverImage.style.display = "block";
		}
		else
		{
			this.OverImage.style.display = "none";
		}
	}
}

/*************************************************
 *
 * RadGetScrollTop
 *
 *************************************************/
function RadGetScrollTop(oDocument)
{
	if (oDocument.documentElement 
		&& oDocument.documentElement.scrollTop)
	{
		return oDocument.documentElement.scrollTop;
	}
	else
	{
		return oDocument.body.scrollTop;
	}
}

/*************************************************
 *
 * RadGetScrollLeft
 *
 *************************************************/
function RadGetScrollLeft(oDocument)
{
	if (oDocument.documentElement 
		&& oDocument.documentElement.scrollLeft)
	{
		return oDocument.documentElement.scrollLeft;
	}
	else
	{
		return oDocument.body.scrollLeft;
	}
}

function CloseDlg(returnValue)
{
	if (window.radWindow)
	{
		window.radWindow.ReturnValue = returnValue;
		window.radWindow.Close();
	}
}