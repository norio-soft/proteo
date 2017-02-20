// JScript File


       
        
          var _jobId;     
          var _legId;     
          var _driver;    
          var _driverResourceId;
          var _regNo;     
          var _vehicleResourceId; 
          var _trailerRef; 
          var _trailerResourceId; 
          var _legPlannedStart; 
          var _legPlannedEnd;
          var _depotCode;
          var _lastUpdateDate ;
          var _legStateId ;
          var _linkJobSourceJobId;
          var _linkJobSourceLegId;
          
        function ShowContextMenuLite(jobId, legId, driver, driverResourceId, regNo, vehicleResourceId, trailerRef, trailerResourceId, legPlannedStart, legPlannedEnd, depotCode, lastUpdateDate, legStateId, rowId) 
        {

           _jobId =             jobId;    
           _legId =             legId;
           _driver =            driver;
           _driverResourceId =  driverResourceId;
           _regNo =             regNo;
           _vehicleResourceId = vehicleResourceId ;
           _trailerRef =        trailerRef;
           _trailerResourceId = trailerResourceId;
           _legPlannedStart =   legPlannedStart;
           _legPlannedEnd =     legPlannedEnd;
           _depotCode =         depotCode;
           _lastUpdateDate  =   lastUpdateDate;
           _legStateId  =       legStateId; 
           
          var yMousePos = window.event.y;
          var xMousePos = window.event.x; 
          
          //GridContextMenu.ShowContextMenu(window.event); 
          //window.event.cancel = true;
          //alert('here');
          HighlightRow(rowId);
          return false; 
        }
        
        
        function ContextMenuClickHandler(item)
        {
          if (top.tsResource.document.getElementById('hidDriverResourceId').value != '')
          {
            _driverResourceId = top.tsResource.document.getElementById('hidDriverResourceId').value;
            _driver = top.tsResource.document.getElementById('hidDriverResourceName').value;
          }
          if (top.tsResource.document.getElementById('hidVehicleResourceId').value != '')
          {
            _vehicleResourceId = top.tsResource.document.getElementById('hidVehicleResourceId').value;
            _regNo = top.tsResource.document.getElementById('hidVehicleResourceName').value;
          }
          if (top.tsResource.document.getElementById('hidTrailerResourceId').value != '')
          {
            _trailerResourceId = top.tsResource.document.getElementById('hidTrailerResourceId').value;
            _trailerRef = top.tsResource.document.getElementById('hidTrailerResourceName').value;
          }
          if (top.tsResource.document.getElementById('hidLinkJobSourceJobId').value != '')
          {
            _linkJobSourceJobId = top.tsResource.document.getElementById('hidLinkJobSourceJobId').value;
          }
          if (top.tsResource.document.getElementById('hidLinkJobSourceInstructionId').value != '')
          {
            _linkJobSourceLegId = top.tsResource.document.getElementById('hidLinkJobSourceInstructionId').value;
          }
          
          GridContextMenu.Hide();
          
          if (item.Text == "Sub-Contract")
             openSubContractWindow(_jobId, _lastUpdateDate);
          else if (item.Text == "Change Booked Times")
             openAlterBookedTimesWindow(_jobId, _lastUpdateDate);
          else if (item.Text == "Change Planned Times")
             openAlterPlannedTimesWindow(_jobId, _lastUpdateDate);
          else if (item.Text == "Trunk")
            openTrunkWindow(_legId, _driver, _regNo, _lastUpdateDate);
          else if (item.Text == "Resource This")
            openResourceWindow(_legId, _driver, _driverResourceId, _regNo, _vehicleResourceId, _trailerRef, _trailerResourceId, _legPlannedStart, _legPlannedEnd, _depotCode, _lastUpdateDate, _jobId);
          else if (item.Text == "Job Details")
            OpenJobDetails(_jobId);
          else if (item.Text == "Communicate This")
          {
            if (_legStateId == 2)
                openCommunicateWindow(_legId, _driver, _driverResourceId, _jobId, _lastUpdateDate);
            else
                alert("You can only communicate Planned legs");
          }
          else if (item.Text == "Remove Trunk")
          {
            openRemoveTrunkWindow(_jobId ,_legId, _lastUpdateDate); 
          }
          else if (item.Text == "Link Job")
          {
            openLinkJobWindow(_jobId, _linkJobSourceJobId, _linkJobSourceLegId, _lastUpdateDate);
          }
          else if (item.Text == "Remove Links")
          {
            openLinkJobWindow(_jobId, "undefined", "undefined", _lastUpdateDate);
          }
          else if (item.Text == "Show Load Order")
          {
            openResizableDialogWithScrollbars('LoadOrder.aspx?jobid=' + _jobId, '700', '258');
          }
           else if (item.Text == "Give Resource")
          {
            if(_driverResourceId != 0 || _vehicleResourceId != 0 || _trailerResourceId != 0)
                openGiveResourcesWindow(_legId);
            else
                alert("There is no resource to Give.");
          }
        }
        
        function onGridDoubleClick(gi)
        {
            var jobId = gi.GetMember("JobId").Value;
            openJobDetailsWindow(jobId);
        }
        
        function onSort(GridColumn, descending)
        {
            //TODO: implement a postback if the sorting is being applied to a grouped grid
            //      otherwise it will sort client side and not display correctly.
        }
        
        function ContextMenuOnShow(menu)
        {
            
            
            var miId = "<%=GridContextMenu.ClientID%>_14" ;
            var mi = document.getElementById(miId);
            mi.className = "DisabledMenuItem";
            mi.onmouseover=null;
            mi.onmouseout= null;

            // disable give resource
            var miGiveResource  = "<%=GridContextMenu.ClientID%>_14" ;
            var miGiveResource = document.getElementById(miGiveResource);
            miGiveResource.className = "DisabledMenuItem";
            miGiveResource.onmouseover=null;
            miGiveResource.onmouseout= null;              
        }
        
        
    var lastHighlightedRow = "";
	var lastHighlightedRowColour = "";
	var lastHighlightedRowClass = "";
	
	function HighlightRow(row)
	{
		var rowElement;
		
		if (lastHighlightedRow != "")
		{
			rowElement = document.getElementById(lastHighlightedRow);
			rowElement.style.backgroundColor = lastHighlightedRowColour;
            rowElement.className = lastHighlightedRowClass;
			
		}

		rowElement = document.getElementById(row);
		lastHighlightedRow = row;
		lastHighlightedRowColour = rowElement.style.backgroundColor;
		lastHighlightedRowClass = rowElement.className;
		rowElement.style.backgroundColor = "";	
		rowElement.className = 'SelectRowTrafficSheetLite';
		
		//GridContextMenu.ShowContextMenu(window.event, row);
		 //  alert(event.button);
        if (event.button != 2) {GridContextMenu.Hide();  return;}
		GridContextMenu.ShowContextMenu(window.event);
	}
	
	function ContextMenuOnShow(menu)
        {
            
            var miId = "<%=GridContextMenu.ClientID%>_14" ;
            var mi = document.getElementById(miId);
            mi.className = "DisabledMenuItem";
            mi.onmouseover=null;
            mi.onmouseout= null;
           
        }
        
    	
	
        
        