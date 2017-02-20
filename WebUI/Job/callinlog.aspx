<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.CallInLog" Codebehind="CallInLog.aspx.cs" MasterPageFile="~/default_tableless.master"   %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
    
<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript" defer>
	     <!--

        function openJobDetailsWindow(jobId) {

            var url = "/job/job.aspx?jobId=" + jobId+ getCSID();

            window.open(url, "JobDetails", "Height=958, Width=1200, Scrollbars=1, Resizable=1");
        }

        function viewOrder(orderID) {
            var url = "/Groupage/ManageOrder.aspx";
            url += "?oID=" + orderID;
            window.open(url, "", "height=900, width=1180,scrollbars=1");
        }
        function ShowPosition(gpsUnitID) {
            var url = "/gps/getcurrentlocation.aspx?uid=" + gpsUnitID;
            window.open(url, "", "height=600, width=630, scrollbars=0");
        }

        function GetOrderLinks(OrderIDs) {
            var retVal = "";
            var strings = OrderIDs.split("/");
            for (var i = 0; i <= strings.length; i++)
                retVal = retVal + "<a href='viewOrder(" + strings[i] + ")'>" + strings[i] + "</a> / ";
            return retVal;
        }
	     //-->

        // Function to show the filter options overlay box
        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
        }
        
	</script>
</asp:Content>    
    
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Outstanding Debriefs</h1></asp:Content>
    
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h2>This report lists outstanding call-ins for clients requiring the capture of de-brief information.</h2>
    <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">
    <fieldset>
        <legend>Filter Options</legend>
        <table border="0">
            <tr>
                <td class="formCellLabel">Control Area</td>
                <td class="formCellField" colspan="4">
                    <asp:DropDownList id="cboControlArea" runat="server" DataTextField="Description" DataValueField="ControlAreaId"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Client</td>
                <td class="formCellField" colspan="4">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true" Height="300px"
                    ShowMoreResultsBox="false" Width="355px" AllowCustomText="false" Overlay="true" ZIndex="99">
                    </telerik:RadComboBox>
                </td>
            </tr>
        </table>
        <asp:Label id="lblError" runat="server"></asp:Label>
    </fieldset>

        <div class="buttonbar">
			<asp:Button id="btnFilter" runat="server" text="Generate Report" CausesValidation="True" width="110" ValidationGroup="vgFilter"></asp:Button>

		</div>
    
    </div>
    <div class="ToolbarBlue" style="border-bottom-width: 0px;">
        
        <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
               
    &nbsp Number of O/S Call Ins <asp:label id="lblCallInCount" runat="server"></asp:label>
               </div>
        
            
 
    <asp:gridview id="gvJobs" runat="server" AllowSorting="true" autogeneratecolumns="false" width="100%" enableviewstate="true" cssclass="Grid" EmptyDataText="There are no outstanding call ins." GridLines="Vertical" >
        <headerstyle cssclass="HeadingRowLite" verticalalign="middle" />
        <rowStyle cssclass="Row" />
        <AlternatingRowStyle cssclass="AlternatingRow" />
        <SelectedRowStyle cssclass="SelectedRow" />
        <columns>
            <asp:templatefield headertext="Run Id" itemstyle-width="75">
                <itemtemplate>
                    <input type="button" alt="Call In" title="Go to Call In Screen" class="buttonClassCallIn-Add"  onclick="window.open('/Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=<%# Eval("JobId") %>&instructionid=<%#Eval("InstructionId") %>'+ getCSID())" />
                    <a href='javascript:openJobDetailsWindow(<%#Eval("JobId") %>);' title="Open this run"><%# Eval("JobId") %></a>
                </itemtemplate>
            </asp:templatefield>
            <asp:templatefield headertext="Order Ids" itemstyle-width="55">
                <itemtemplate>
                    <%#GetOrderIDs((string)Eval("OrderIDs")) %>
                </itemtemplate>
            </asp:templatefield>
            <asp:templatefield headertext="" itemstyle-width="22">
                <itemtemplate>
                    <img src="/images/<%#GetInstructionTypeImage((int)Eval("InstructionTypeID"))%>" width="20"/> 
                </itemtemplate>
            </asp:templatefield>
            <asp:boundfield datafield="LoadNo" headertext="Load No" itemstyle-width="80" SortExpression="LoadNo" />
            <asp:boundfield datafield="Client" headertext="Client" itemstyle-width="140" SortExpression="Client" />
            <asp:boundfield datafield="Destination" headertext="Destination" itemstyle-width="140" SortExpression="Destination" />                                
            <asp:boundfield datafield="BookedDateTime" dataformatstring="{0:dd/MM HH:mm}" htmlencode="false" headertext="Booked" itemstyle-width="100" SortExpression="BookedDateTime" />                
            <asp:boundfield datafield="Docket" headertext="Docket" itemstyle-width="400" SortExpression="Docket" />                                
            <asp:boundfield datafield="NoPallets" headertext="Pallets" itemstyle-width="45" SortExpression="NoPallets" />                                
            <asp:boundfield datafield="Weight" DataFormatString="{0:F2}" headertext="Weight" itemstyle-width="80" SortExpression="Weight" />                                
            <asp:boundfield datafield="Driver" headertext="Driver" itemstyle-width="120" SortExpression="Driver" />                                
            <asp:boundfield datafield="TrailerRef" headertext="Trailer" itemstyle-width="40" SortExpression="TrailerRef" /> 
             <asp:templatefield headertext="">
                    <itemtemplate>
                        <img src="/images/newmasterpage/search network.png" alt="Locate trailer" title="Locate Trailer" onclick="ShowPosition('<%# Eval("TrailerGPSUnitID")%>');" <%# Eval("TrailerGPSUnitID").ToString().Trim() == "" ? "style='display:none;'" : ""  %> />
                    </itemtemplate> 
                </asp:templatefield>                                                               
                <asp:templatefield headertext="">
                    <itemtemplate>
                        <img src="/images/newmasterpage/search network.png" alt="Locate vehicle" title="Locate Vehicle" onclick="ShowPosition('<%# Eval("VehicleGPSUnitID")%>')"; <%# Eval("VehicleGPSUnitID").ToString().Trim() == "" ? "style='display:none;'" : ""  %> />
                    </itemtemplate> 
                </asp:templatefield>                                                          
        </columns>
    </asp:gridview>
     <script language="javascript" type="text/javascript">
         FilterOptionsDisplayHide();
</script>
</asp:Content>
