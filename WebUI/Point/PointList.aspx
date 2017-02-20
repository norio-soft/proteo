<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" CodeBehind="PointList.aspx.cs" Inherits="Orchestrator.WebUI.Point.PointList" %>

<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Point List</h1>
    <script type="text/javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
    <script src="/script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">

<script language="javascript" type="text/javascript">
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
    <div>
        <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1034" Height="750" Mode="Normal" URL="/Point/Geofences/GeofenceManagement.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>
        <cc1:Dialog ID="dlgAddUpdatePoint" runat="server" Width="1000" Height="900" Mode="Normal" URL="/Point/addUpdatePoint.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>
        <cc1:Dialog ID="dlgAddUpdateOrganisation" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Organisation/addUpdateOrganisation.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>


        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
                <fieldset>
            <legend runat="server"> <%= Orchestrator.Globals.Configuration.FleetMetrikInstance ? "Location List" : "Point List" %></legend>
            <table width="1000px">
                <tr>
                    <td colspan="8">
                        <asp:RadioButtonList ID="rblGeocodedOrNot" runat="server" AutoPostBack="true" RepeatDirection="Horizontal">
                            <asp:ListItem Text="All" Value="0"></asp:ListItem>
                            <asp:ListItem Text="Geocoded" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Non-Geocoded" Value="2"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Client
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cboClient" runat="server" AutoPostBack="true" AllowCustomText="true" Height="300px" ShowMoreResultsBox="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="220px" EnableLoadOnDemand="true" >
                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetOrganisations" />
                        </telerik:RadComboBox>
                    </td>
                    <td class="formCellLabel">
                        Closest Town
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cboClosestTown" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" AutoPostBack="true" MarkFirstMatch="true" AllowCustomText="true" ShowMoreResultsBox="false" Width="210px" Height="300px" Overlay="true">
                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClosestTownNoCountry" />
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel" runat="server">
                       <%= Orchestrator.Globals.Configuration.FleetMetrikInstance ? "Location Description" : "Point Description" %>
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtPointDescription" runat="server" Text="" Width="280px"></asp:TextBox>
                    </td>
                    <td class="formCellLabel">
                        Post Code
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtPostCode" runat="server" Text="" Width="100px"></asp:TextBox>
                    </td>
                    <td class="formCellLabel" runat="server">
                        <%= Orchestrator.Globals.Configuration.FleetMetrikInstance ? "Location Code" : "Point Code" %>
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtPointCode" runat="server" Text="" Width="150px"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnRefreshTop" runat="server" CssClass="buttonClass" Text="Refresh" />
        </div> 
        </div>
          
        <telerik:RadGrid runat="server" ID="rgPoints" AutoGenerateColumns="false" AllowSorting="true" Width="100%" EnableAjaxSkinRendering="true" ClientSettings-ClientEvents-OnDataBound="InitQuickSearch()">
            <MasterTableView CommandItemDisplay="Top">
                 <CommandItemTemplate>
                    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()">
                    Show filter Options</div>
                    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">
                    Close filter Options</div>
                    <asp:Button ID="btnAddPoint" runat="server" CssClass="buttonClass" OnClientClick="AddPoint(); return false;" Text="Add Location" Visible=<%#Orchestrator.Globals.Configuration.FleetMetrikInstance%>/>   
                    <asp:Label ID="lblQF" runat="server" Text="Quick Filter:" />
                    <div id="qsDiv">
                    </div>
                </CommandItemTemplate>
                <Columns>   
                    <telerik:GridTemplateColumn HeaderText="Point" UniqueName="Point">
                        <ItemTemplate>  
                            <a href="javascript:UpdatePoint(<%#Eval("PointId")%>,<%#Eval("IdentityId")%>,'<%#Eval("Client")%>');"><%#Eval("Description") %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Town" HeaderText="Town" SortExpression="Town"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="">
                        <ItemTemplate>  
                            <a href="javascript:UpdatePosition(<%#Eval("PointId")%>);">Alter Position</a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Radius" HeaderText="Radius"  HeaderStyle-Width="80px"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Custom geofence" HeaderStyle-Width="65px">
                        <ItemTemplate>  
                            <asp:CheckBox ID="chkCustomGeofence" runat="server" Enabled="false"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Latitude" HeaderText="Latitude"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Longitude" HeaderText="Longitude"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Client" SortExpression="Client">
					<ItemTemplate>
                        <%# Orchestrator.Globals.Configuration.FleetMetrikInstance ? Eval("Client") : String.Format("<a href=\"javascript:UpdateOrganisation('{0}')\">{1}</a>",Eval("IdentityId"), Eval("Client")) %>
					</ItemTemplate>
					</telerik:GridTemplateColumn>
					<telerik:GridBoundColumn DataField="PointCode" HeaderText="Point Code" UniqueName="PointCode"></telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
            <Resizing AllowColumnResize="true" AllowRowResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="true" ClipCellContentOnResize="true" />
            </ClientSettings>
        </telerik:RadGrid>
    </div>
    
    <telerik:RadAjaxManager ID="ramListPoints" runat="server" EnableHistory="True">
        <AjaxSettings>
           <telerik:AjaxSetting AjaxControlID="btnRefreshTop" EventName="Click">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rgPoints" />
            </UpdatedControls>
           </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="cboClient" EventName="SelectedIndexChanged">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rgPoints"/>
            </UpdatedControls>
           </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="cboClosestTown" EventName="SelectedIndexChanged">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rgPoints"/>
            </UpdatedControls>
           </telerik:AjaxSetting>
           <telerik:AjaxSetting AjaxControlID="rblGeocodedOrNot" EventName="SelectedIndexChanged">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="rgPoints"/>
            </UpdatedControls>
           </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
   <telerik:RadCodeBlock runat="server">
    <script type="text/javascript">

        function InitQuickSearch()
        {
            $('table#<%=rgPoints.ClientID%>_ctl00 tbody tr:not(.GroupHeader_Orchestrator)').quicksearch({ 
                position: 'after',
                attached: '#qsDiv',
                delay: 100,
                labelText: ''
            });
        };

        function AddPoint() {
            var qs = "";
            <%=dlgAddUpdatePoint.ClientID %>_Open(qs);
        }

        function UpdatePoint(pointId, identityId, orgname) {
            var qs = "PointId=" + pointId + "&IdentityId=" + identityId + "&organisationName=" + orgname;
        
            <%=dlgAddUpdatePoint.ClientID %>_Open(qs);
        }

        function UpdateOrganisation(identityId) {
            var qs = "identityId=" + identityId;
            
            <%=dlgAddUpdateOrganisation.ClientID %>_Open(qs);
        }
//            
//        function cboClosestTown_Requesting(sender, eventargs) {
//            var context = eventargs.get_context();
//            context["CountryId"] = 1;
//        }
        
        function UpdatePosition(pointId) {
            var qs = "pointId=" + pointId;
            <%=dlgAddUpdatePointGeofence.ClientID %>_Open(qs);
        }
        FilterOptionsDisplayHide();
    </script>
 
    </telerik:RadCodeBlock>
</asp:Content>

