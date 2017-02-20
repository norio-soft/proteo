<%@ Page Language="c#" Inherits="Orchestrator.WebUI.resource.vehicle.VehicleList"
    CodeBehind="VehicleList.aspx.cs" MasterPageFile="~/default_tableless.Master"
    Title="Vehicle List" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Vehicle List</h1>
</asp:Content>
<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
    

    <style type="text/css">
    .rgDataDiv
    {
        overflow-x: hidden !important;
    }
    </style>


    <script type="text/javascript">
        		function showPosition(gpsUnitID) {          
			var qs = "uid=" + gpsUnitID ;
		
			<%= dlgGetCurrentLocation.ClientID %>_Open(qs);
        		}


         function initialiseQuickSearch() {
			$('#<%=grdVehicles.ClientID %>_ctl00 tbody tr').quicksearch({
				position: 'after',
				attached: '#grdFilterHolder',
				inputClass: 'input_Class',
				labelText: '',
				delay: 100,
				randomElement: 'qsGrdVehicles'
			});

            //when we filter the grid with quicksearch, update the vehicle count
			$('input[rel="' + "qsGrdVehicles" + '"]').keydown(function (e) {
			var keycode = e.keyCode;
			if (!(keycode === 9 || keycode === 13 || keycode === 16 || keycode === 17 || keycode === 18 || keycode === 38 || keycode === 40 || keycode === 224))
			{
				updateVehicleCount();
			}
		});
		}

       		$(document).ready(function () {            
			initialiseQuickSearch();
		});

		function ajaxResponseEnd() {            
			initialiseQuickSearch();
			updateVehicleCount();
		}


        function updateVehicleCount(){            
			var vehicleCountSpan = document.getElementById("vehicleCount");
			var count = $find("<%= grdVehicles.ClientID %>").get_masterTableView().get_element().tBodies[0].rows.length;

			if (count > 0)
				vehicleCountSpan.textContent = "Vehicles Found: " + count;
			else
				vehicleCountSpan.textContent = "";            
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <cc1:Dialog ID="dlgGetCurrentLocation" runat="server" Width="630" Height="600" Mode="Normal" URL="/gps/getcurrentlocation.aspx" AutoPostBack="true" ReturnValueExpected="true"></cc1:Dialog>
    <h2>
        Please choose a vehicle from the list below.</h2>
    <asp:Label ID="lblNote" runat="server" Text=""></asp:Label>
    <fieldset runat="server" id="fsDateFilter" visible="false">
        <legend>Filter Options</legend>
        <table border="0" cellpadding="1" cellspacing="0">
            <tr>
                <td class="formCellLabel">
                    Start Date
                </td>
                <td width="105">
                    <telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yyyy" >
                    </DateInput>
                    </telerik:RadDatePicker>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate"
                        ErrorMessage="Please specify the start date." Display="Dynamic"> <img src="/images/newMasterPage/icon-warning.png" height="16" width="16" title="Please specify the start date." alt="" /></asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvStartDate" runat="server" EnableClientScript="False"
                        ControlToValidate="rdiStartDate" ErrorMessage="The start date must occur before the end date."
                        Display="Dynamic"> <img src="/images/newMasterPage/icon-warning.png" height="16" width="16" title="The start date must occur before the end date." alt="" /></asp:CustomValidator>
                </td>
                <td class="formCellLabel">
                    End Date
                </td>
                <td>
                    <telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yyyy">
                    </DateInput>
                    </telerik:RadDatePicker>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate"
                        ErrorMessage="Please specify the end date." Display="Dynamic"> <img src="/images/newMasterPage/icon-warning.png" height="16" width="16" title="Please specify the end date." alt="" /></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </fieldset>
    <asp:Panel runat="server">

    
     <div class="buttonbar" style="display: <%=fsDateFilter.Visible == true ? "" : "none" %>">
        <asp:Button ID="btnFilter" runat="server"  Text="Filter" Width="75"></asp:Button>
    </div>
        </asp:Panel>
    <cc1:dialog id="dlgAddUpdateVehicle" url="addupdatevehicle.aspx" width="580" height="640"
        autopostback="true" mode="Modal" runat="server" returnvalueexpected="false" scrollbars="false">
    </cc1:dialog>


       <telerik:RadAjaxManager ID="raxManager" runat="server" ClientEvents-OnResponseEnd="ajaxResponseEnd" >
	<AjaxSettings>
	<telerik:AjaxSetting AjaxControlID="grdVehicles">
					<UpdatedControls>
						<telerik:AjaxUpdatedControl ControlID="grdVehicles" LoadingPanelID="raxPanel" />                        
					</UpdatedControls>
				</telerik:AjaxSetting>
				</AjaxSettings>
	</telerik:RadAjaxManager>

    <telerik:RadGrid runat="server" ID="grdVehicles" AutoGenerateColumns="false" AllowSorting="true" Skin="Orchestrator" EnableViewState="true" Width="99%">
        <MasterTableView DataKeyNames="ResourceId" CommandItemDisplay="Top" CommandItemStyle-BackColor="White" TableLayout="Auto">
            <RowIndicatorColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>
            <CommandItemTemplate>
                <input type="button" class="buttonClassSmall" value="Add Vehicle" runat="server" id="btnAddVehicle" />
                <asp:Label ID="lblQF" runat="server" Text="Quick Filter:" />
                <div id="grdFilterHolder">
                </div>

                <span runat="server"  visible="<%#Orchestrator.Globals.Configuration.FleetMetrikInstance %>">Include Deleted: </span>
                <asp:CheckBox ID="chkIncludeDeleted" OnCheckedChanged="chkIncludeDeleted_CheckedChanged" OnPreRender="chkIncludeDeleted_PreRender" Visible="<%#Orchestrator.Globals.Configuration.FleetMetrikInstance %>" AutoPostBack="true" runat="server" />
                <span id="vehicleCount" style="float: right;"></span>

            </CommandItemTemplate>
            <Columns>
                <telerik:GridTemplateColumn ItemStyle-Wrap="false" UniqueName="RegNo" HeaderText="Reg"
                    DataField="RegNo">
                    <ItemTemplate>
                        <a runat="server" id="hypAddUpdateVehicle"></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="NominalCode" DataField="NominalCode" HeaderText="Nominal Code" />
                <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="IsFixedUnit" HeaderText="Fixed Unit"
                    DataField="IsFixedUnit">
                    <ItemTemplate>
                        <%# (bool)((System.Data.DataRowView)Container.DataItem)["IsFixedUnit"] == true ? "Yes" : "No" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn Display="true" UniqueName="RegularDriver" DataField="RegularDriver"
                    HeaderText="Full Name" />
                <telerik:GridBoundColumn Display="true" UniqueName="VehicleManufacturer" DataField="VehicleManufacturer"
                    HeaderText="Manufacturer" />
                <telerik:GridBoundColumn Display="true" UniqueName="ThirdPartyIntegrationID" DataField="ThirdPartyIntegrationID"
                    HeaderText="Third Party Integration ID" />
                <telerik:GridBoundColumn Display="true" UniqueName="GPSUnitID" DataField="GPSUnitID"
                    HeaderText="GPS Unit ID" />
                <telerik:GridBoundColumn DataField="DateStamp" HeaderText="GPS Last Update" HeaderStyle-Width="140" UniqueName="GPSLastUpdate" Visible ="false">
				</telerik:GridBoundColumn>
				<telerik:GridBoundColumn DataField="Reason" HeaderText="GPS Last Update Type" HeaderStyle-Width="100" UniqueName="GPSLastUpdateType" Visible ="false">
				</telerik:GridBoundColumn>
				<telerik:GridTemplateColumn HeaderText="GPS Status" SortExpression="GPSStatus" HeaderStyle-Width="40" UniqueName="GPSStatus" Visible="false">
					<ItemTemplate>
						<img src="<%#String.Format("/images/icons/tl{0}.gif", Eval("GPSStatus").ToString().ToLower()) %>" />
					</ItemTemplate>
				</telerik:GridTemplateColumn>
                <telerik:GridBoundColumn Display="true" UniqueName="MOTExpiry" DataField="MOTExpiry"
                    HeaderText="MOT Expires" DataFormatString="{0:dd/MM/yy}" />
                <telerik:GridBoundColumn Display="true" UniqueName="TelephoneNumber" DataField="TelephoneNumber"
                    HeaderText="Cab Phone" />
                <telerik:GridBoundColumn Display="true" UniqueName="DepotCode" DataField="DepotCode"
                    HeaderText="Current Controller" />
                <telerik:GridTemplateColumn ItemStyle-Width="250px" Display="true" UniqueName="CurrentLocation"
                    DataField="CurrentLocation" HeaderText="Location">
                <ItemTemplate>

						<%#(Eval("GPSUnitID") != DBNull.Value && Eval("GPSUnitID") != String.Empty) && Orchestrator.Globals.Configuration.FleetMetrikInstance ? String.Format("<a href=\"javascript:showPosition('{0}')\">{1}</a>", Eval("GPSUnitID"), Eval("Gazetteer")) : Eval("CurrentLocation")%>
                    

					</ItemTemplate>
                    </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" UniqueName="ResourceId" HeaderText="Show Plan"
                    DataField="ResourceId">
                    <ItemTemplate>
                        <a href="javascript:GetShowFutureLink('<%#((System.Data.DataRowView)Container.DataItem)["ResourceId"]%>')">
                            <div>
                                Show Future
                            </div>
                        </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <script language="javascript" type="text/javascript">

       

        function GetShowFutureLink(resourceId) {
            var dateString = '<%# DateTime.UtcNow.AddDays(-1).ToString("ddMMyyyy")%>' + '0000';
            var output = "<a href=\"javascript:ShowFuture('" + resourceId + "','1' ,'" + dateString + "');\">Resource Future</a>";
            return output;
        }

        function ShowFuture(resourceId, resourceTypeId, fromDate) {
            var url = '../../Resource/Future.aspx?wiz=true&resourceId=' + resourceId + '&resourceTypeId=' + resourceTypeId + '&fromDateTime=' + fromDate;
            openResizableDialogWithScrollbars(url, 1050, 492);
        }
        
    </script>

</asp:Content>
