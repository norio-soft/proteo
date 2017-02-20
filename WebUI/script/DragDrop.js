    var currentRow, movedRow = null;
    function RowCreated(row)
    {
        row.Control.RowIndex = row.RealIndex;
        var mouseDownHandler = function(e)
		{
			if (!e)
				var e = window.event;

			if (!currentRow)
			{
				currentRow = document.createElement("div");
				currentRow.innerHTML = "<table border=\"1\"><tbody><tr>"+row.Control.innerHTML+"</tr></tbody><table>";
				document.body.appendChild(currentRow);
				currentRow.style.position = "absolute";
				currentRow.style.display = "none";
				
				movedRow = row;
			}
			
			ClearDocumentEvents();				
		};
		var mouseUpHandler = function(e)
		{
			if (!e)
				var e = window.event;

			if (currentRow)
			{
				if (movedRow && currentRow.style.display != "none")
				{				
				    var targetElement = e.srcElement ? e.srcElement : e.target;				    
				    var droppedRowIndex;
				    while(typeof(droppedRowIndex) == "undefined")
				    {
				        droppedRowIndex = targetElement.parentNode.RowIndex;
				        targetElement = targetElement.parentNode;	//in case of controls in the row			        
				    }
				    
                    ReorderRows(movedRow.RealIndex, droppedRowIndex);
				}
				document.body.removeChild(currentRow);
				currentRow = null;
				movedRow = null;
			}
			RestoreDocumentEvents();
		};
		var mouseMoveHandler = function(e)
		{
			if (!e)
				var e = window.event;
			if (currentRow)
			{
				currentRow.style.display = "";
				currentRow.style.top =  e.clientY + 
										document.documentElement.scrollTop + 
										document.body.scrollTop + 3 + "px";

				currentRow.style.left = e.clientX + 
										document.documentElement.scrollLeft + 
										document.body.scrollLeft + 3 + "px";
			}
			
		};
		
		if (row.Control.attachEvent)
		{
			row.Control.attachEvent("onmousedown", mouseDownHandler);
			document.body.attachEvent("onmouseup", mouseUpHandler);
			document.body.attachEvent("onmousemove", mouseMoveHandler);
		}

		if (row.Control.addEventListener)
		{
			row.Control.addEventListener("mousedown", mouseDownHandler, true);
			document.body.addEventListener("mouseup", mouseUpHandler, true);
			document.body.addEventListener("mousemove", mouseMoveHandler, true);
		}
    }
    
	function ClearDocumentEvents()
	{
		if (document.onmousedown != this.mouseDownHandler)
		{
			this.documentOnMouseDown = document.onmousedown;
		}

		if (document.onselectstart != this.selectStartHandler)
		{
			this.documentOnSelectStart = document.onselectstart;
		}

		this.mouseDownHandler = function(e){return false;};
		this.selectStartHandler = function(){return false;};

		document.onmousedown = this.mouseDownHandler;
		document.onselectstart = this.selectStartHandler;

	};
	
	function RestoreDocumentEvents()
	{
		if ((typeof(this.documentOnMouseDown) == "function") &&
			(document.onmousedown != this.mouseDownHandler))
		{
			document.onmousedown = this.documentOnMouseDown;
		}
		else
		{
			document.onmousedown = "";
		}
		
		if ((typeof(this.documentOnSelectStart) == "function") &&
			(document.onselectstart != this.selectStartHandler))
		{
			document.onselectstart = this.documentOnSelectStart;
		}
		else
		{
			document.onselectstart = "";
		}
	};
    