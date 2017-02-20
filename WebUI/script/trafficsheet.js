var _jobId
var _instructionId
var _driver
var _driverResourceId
var _regNo
var _vehicleResourceId
var _trailerRef
var _trailerResourceId
var _legPlannedStart
var _legPlannedEnd
var _depotCode
var _lastUpdateDate
var _instructionStateId
var _linkJobSourceJobId
var _linkJobSourceLegId
function ShowContextMenuLite(jobId,instructionId,driver,driverResourceId,regNo,vehicleResourceId,trailerRef,trailerResourceId,legPlannedStart,legPlannedEnd,depotCode,lastUpdateDate,_instructionStateId,rowId){
_jobId=jobId
_instructionId=instructionId
_driver=driver
_driverResourceId=driverResourceId
_regNo=regNo
_vehicleResourceId=vehicleResourceId
_trailerRef=trailerRef
_trailerResourceId=trailerResourceId
_legPlannedStart=legPlannedStart
_legPlannedEnd=legPlannedEnd
_depotCode=depotCode
_lastUpdateDate=lastUpdateDate
_instructionStateId=instructionStateId
var yMousePos=window.event.y
var xMousePos=window.event.x
HighlightRow(rowId)
return false}
function ContextMenuClickHandler(item){
if(top.document.getElementById('hidDriverResourceId').value !=''){
_driverResourceId=top.document.getElementById('hidDriverResourceId').value
_driver=top.document.getElementById('hidDriverResourceName').value}
if(top.document.getElementById('hidVehicleResourceId').value !=''){
_vehicleResourceId=top.document.getElementById('hidVehicleResourceId').value
_regNo=top.document.getElementById('hidVehicleResourceName').value}
if(top.document.getElementById('hidTrailerResourceId').value !=''){
_trailerResourceId=top.document.getElementById('hidTrailerResourceId').value
_trailerRef=top.document.getElementById('hidTrailerResourceName').value}
if(top.document.getElementById('hidLinkJobSourceJobId').value !=''){
_linkJobSourceJobId=top.document.getElementById('hidLinkJobSourceJobId').value}
else
_linkJobSourceJobId=''
if(top.document.getElementById('hidLinkJobSourceInstructionId').value !=''){
_linkJobSourceLegId=top.document.getElementById('hidLinkJobSourceInstructionId').value}
else
_linkJobSourceLegId=''
GridContextMenu.Hide()
if(item.Text=="Sub-Contract")
openSubContractWindow(_jobId,_lastUpdateDate)
else if(item.Text=="Change Booked Times")
openAlterBookedTimesWindow(_jobId,_lastUpdateDate)
else if(item.Text=="Change Planned Times")
openAlterPlannedTimesWindow(_jobId,_lastUpdateDate)
else if(item.Text=="Trunk")
openTrunkWindow(_instructionId,_driver,_regNo,_lastUpdateDate)
else if(item.Text=="Resource This")
openResourceWindow(_instructionId,_driver,_driverResourceId,_regNo,_vehicleResourceId,_trailerRef,_trailerResourceId,_legPlannedStart,_legPlannedEnd,_depotCode,_lastUpdateDate,_jobId)
else if(item.Text=="Job Details")
OpenJobDetails(_jobId)
else if(item.Text=="Communicate This"){
if(_instructionStateId==2)
openCommunicateWindow(_instructionId,_driver,_driverResourceId,_jobId,_lastUpdateDate)
else
alert("You can only communicate Planned legs")}
else if(item.Text=="Remove Trunk"){
openRemoveTrunkWindow(_jobId,_instructionId,_lastUpdateDate)}
else if(item.Text=="Link Job"&&_linkJobSourceJobId !=''&&_linkJobSourceLegId !=''){
openLinkJobWindow(_jobId,_linkJobSourceJobId,_linkJobSourceLegId,_lastUpdateDate)}
else if(item.Text=="Remove Links"){
openLinkJobWindow(_jobId,"undefined","undefined",_lastUpdateDate)}
else if(item.Text=="Show Load Order"){
openResizableDialogWithScrollbars('LoadOrder.aspx?jobid='+_jobId,'700','258')}
else if(item.Text=="Give Resource"){
if(_driverResourceId !=0 || _vehicleResourceId !=0 || _trailerResourceId !=0)
openGiveResourcesWindow(_instructionId)
else
alert("There is no resource to Give.")}
else if(item.Text=="Add Destination"){
openAddDestination(_jobId,_lastUpdateDate)}
else if(item.Text=="Remove Destination"){
openRemoveDestination(_jobId,_lastUpdateDate)}}
function onGridDoubleClick(gi){
var jobId=gi.GetMember("JobId").Value
openJobDetailsWindow(jobId)}
var lastHighlightedRow=""
var lastHighlightedRowColour=""
var lastHighlightedRowClass=""
function HighlightRow(row){
var rowElement
if(lastHighlightedRow !=""){
rowElement=document.getElementById(lastHighlightedRow)
rowElement.style.backgroundColor=lastHighlightedRowColour
rowElement.className=lastHighlightedRowClass}
rowElement=document.getElementById(row)
lastHighlightedRow=row
lastHighlightedRowColour=rowElement.style.backgroundColor
lastHighlightedRowClass=rowElement.className
rowElement.style.backgroundColor=""
rowElement.className='SelectRowTrafficSheetLite'
if(event.button !=2){GridContextMenu.Hide();return;}
GridContextMenu.ShowContextMenu(window.event)}
function ContextMenuOnShow(menu){

var mi=document.getElementById(miId)
mi.className="DisabledMenuItem"
mi.onmouseover=null
mi.onmouseout=null}



function _showModalDialog(url,width,height,windowTitle){
MyClientSideAnchor.WindowHeight=height+"px"
MyClientSideAnchor.WindowWidth=width+"px"
MyClientSideAnchor.URL=url
MyClientSideAnchor.Title=windowTitle
var returnvalue=MyClientSideAnchor.Open()
if(returnvalue==true){
if(MyClientSideAnchor.OutputData.indexOf('filterData')>0){
top.document.getElementById('tsResource').src=top.document.getElementById('tsResource').src}
document.all.form1.submit()}
return returnvalue}
function GetImage(legPosition){
switch(legPosition){
case "First":
return "<img src='../images/legTop.gif' height=20 width=5 />"
case "Middle":
return "<img src='../images/legMiddle.gif' height=20 width=5 />"
case "Last":
return "<img src='../images/legBottom.gif' height=20 width=5 />"}
return "<img src='../images/spacer.gif' height=20 width=5 />"}
function openJobDetailsWindow(jobId){
    openResizableDialogWithScrollbars('../Job/Job.aspx?wiz=true&jobId=' + jobId + getCSID(), '1220', '800')
}

function openTrafficAreaWindow(url){
_showModalDialog(url,500,221,"Change Traffic Area")}
function openAlterBookedTimesWindow(jobId,lastUpdateDate){
var url="changeBookedTimes.aspx?jobId="+jobId+"&LastUpdateDate="+lastUpdateDate
_showModalDialog(url,500,320,"Change Booked Times")}
function openAlterPlannedTimesWindow(jobId,lastUpdateDate){
var url="changePlannedTimes.aspx?jobId="+jobId+"&LastUpdateDate="+lastUpdateDate
_showModalDialog(url,700,320,"Change Planned Times")}
function openSubContractWindow(jobId,lastUpdateDate){
var url="SubContract.aspx?jobId="+jobId+"&lastUpdateDate="+lastUpdateDate+"&CA="+activeControlAreaId+"&TA="+activeTrafficAreaIds
_showModalDialog(url,650,580,'Sub-Contract Job')}
function openResourceWindow(InstructionId,Driver,DriverResourceId,RegNo,VehicleResourceId,TrailerRef,TrailerResourceId,legStart,legEnd,DepotCode,lastUpdateDate,jobId){
if(top.document.getElementById('hidDriverResourceId').value !=''){
DriverResourceId=top.document.getElementById('hidDriverResourceId').value
Driver=top.document.getElementById('hidDriverResourceName').value}
if(top.document.getElementById('hidVehicleResourceId').value !=''){
VehicleResourceId=top.document.getElementById('hidVehicleResourceId').value
RegNo=top.document.getElementById('hidVehicleResourceName').value}
if(top.document.getElementById('hidTrailerResourceId').value !=''){
TrailerResourceId=top.document.getElementById('hidTrailerResourceId').value
TrailerRef=top.document.getElementById('hidTrailerResourceName').value}
var url = "resourceThis.aspx?iID=" + InstructionId + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&RegNo=" + RegNo + "&VR=" + VehicleResourceId + "&TrailerRef=" + TrailerRef + "&TR=" + TrailerResourceId + "&LS=" + legStart + "&LE=" + legEnd + "&DC=" + DepotCode + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds + "&LastUpdateDate=" + lastUpdateDate + "&jobId=" + jobId + "&depotId=<%=DepotId %>" + getCSID();
_showModalDialog(url,750,500,"Resource This")}
function openCommunicateWindow(InstructionId,Driver,DriverResourceId,JobId,lastUpdateDate){
var url="communicateThis.aspx?iID="+InstructionId+"&Driver="+Driver+"&DR="+DriverResourceId+"&jobId="+JobId+"&LastUpdateDate="+lastUpdateDate
_showModalDialog(url,500,600,"Communicate This")}
function openTrunkWindow(InstructionId,Driver,RegNo,LastUpdateDate){
var url="trunk.aspx?iID="+InstructionId+"&Driver="+Driver+"&RegNo="+RegNo+"&LastUpdateDate="+LastUpdateDate
_showModalDialog(url,550,358,"Trunk Leg")}
function openRemoveTrunkWindow(JobId,InstructionId,LastUpdateDate){
var url="removetrunk.aspx?jobId="+JobId+"&iID="+InstructionId+"&LastUpdateDate="+LastUpdateDate
_showModalDialog(url,550,358,"Remove Trunk Leg")}
function openLinkJobWindow(JobId,SourceJobJobId,SourceJobInstructionId,LastUpdateDate){
    var url = "linkJob.aspx?jobId=" + JobId + "&SourceJobJobId=" + SourceJobJobId + "&SourceJobInstructionId=" + SourceJobInstructionId + "&LastUpdateDate=" + LastUpdateDate + getCSID()
_showModalDialog(url,550,358,"Link Jobs")}
function openUpdateLocation(instructionId){
var url="updateResourceLocations.aspx?instructionid="+instructionId
_showModalDialog(url,400,320,'Update Resource Locations')}
function openPalletHandlingWindow(jobId){
var url="JobManagement/addupdatepallethandling.aspx?jobId="+jobId
_showModalDialog(url,550,725,'Configure Pallet Handling')}
function OpenJobDetails(jobId){
    openDialogWithScrollbars('../job/job.aspx?wiz=true&jobId=' + jobId + getCSID(), '600', '400')
}
function openAddDestination(jobId,LastUpdateDate){
_showModalDialog('../job/addDestination.aspx?wiz=true&jobId='+jobId+'&LastUpdateDate='+LastUpdateDate,'550','358','Add Destination')}
function openRemoveDestination(jobId,LastUpdateDate){
_showModalDialog('../job/removeDestination.aspx?wiz=true&jobId='+jobId+'&LastUpdateDate='+LastUpdateDate,'550','258','Remove Destination')}
function GetLegAction(instructionStateId,jobId,lastUpdateDate){
if(instructionStateId==1)
return "<a href='javascript:openSubContractWindow("+jobId+", "+lastUpdateDate+"'>Sub-Contract</a>"}
function GetShowPoint(dataItem){
if(dataItem.GetMember("PointId").Value !="")
return "<span onmouseover=\"ShowPoint('../point/getPointAddresshtml.aspx', "+dataItem.GetMember("PointId").Value+");\" onmouseout=\"HidePoint();\"> "+dataItem.GetMember("Description").Value+"</span>"
else
return ""}
function showResources() {

    var qs = '';

if (sessionStorage && sessionStorage.sessionID) {
    qs = '?csid=' + sessionStorage.sessionID;
}

top.tsResource.location.href = "TSResource.aspx" + qs;
top.startCollapsed()}
function openGiveResourcesWindow(instructionId){
var url="giveResources.aspx?iId="+instructionId+"&dr=null&vr=null&tr=null&ca=null&ta=null"
_showModalDialog(url,400,320,'Give Resources')}



