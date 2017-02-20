<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" CodeBehind="ListPointsWithUnknownTown.aspx.cs" Inherits="Orchestrator.WebUI.Point.ListPointsWithUnknownTown" %>
<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>
<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div>
        <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Point/addUpdatePointGeofence.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>
        <cc1:Dialog ID="dlgAddUpdatePoint" runat="server" Width="1000" Height="900" Mode="Normal" URL="/Point/addUpdatePoint.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>
        <cc1:Dialog ID="dlgAddUpdateOrganisation" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Organisation/addUpdateOrganisation.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>

        <h1>Point List</h1>  
        <table width="100%">
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
                <td class="formCellLabel-Horizontal">
                    Client
                </td>
                <td class="formCellField-Horizontal">
                    <telerik:RadComboBox ID="cboClient" runat="server" AutoPostBack="true" AllowCustomText="true" Height="300px" ShowMoreResultsBox="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="220px" EnableLoadOnDemand="true" ><WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" /></telerik:RadComboBox>
                </td>
                <td class="formCellLabel">
                    Closest Town
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboClosestTown" runat="server" EnableLoadOnDemand="true" OnClientItemsRequesting="cboClosestTown_Requesting" ItemRequestTimeout="500" AutoPostBack="true" MarkFirstMatch="true" AllowCustomText="true" ShowMoreResultsBox="false" Width="210px" Height="300px" Overlay="true">
                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClosestTown" />
                    </telerik:RadComboBox>
                </td>
                <td class="formCellLabel">
                    Point Description
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
            </tr>
        </table>
        <div class="buttonbar">
            <asp:Button ID="btnRefreshTop" runat="server" CssClass="buttonClass" Text="Refresh" />
        </div>
        
        <telerik:RadGrid runat="server" ID="rgPoints" AutoGenerateColumns="false" AllowSorting="true" Width="100%" EnableAjaxSkinRendering="true">
            <MasterTableView>
                <Columns>   
                    <telerik:GridTemplateColumn HeaderText="Point">
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
                    <telerik:GridBoundColumn DataField="Radius" HeaderText="Geofence Radius"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Custom geofence">
                        <ItemTemplate>  
                            <asp:CheckBox ID="chkCustomGeofence" runat="server" Enabled="false"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="Latitude" HeaderText="Latitude"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Longitude" HeaderText="Longitude"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Client" SortExpression="Client">
					<ItemTemplate>
                        <a href="javascript:UpdateOrganisation(<%#Eval("IdentityId")%>)">
							<%#Eval("Client")%>
						</a>
					</ItemTemplate>
					</telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Scrolling AllowScroll="true" ScrollHeight="500" />
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
        function UpdatePoint(pointId, identityId, orgname) {
            var qs = "PointId=" + pointId + "&IdentityId=" + identityId + "&organisationName=" + orgname;
        
            <%=dlgAddUpdatePoint.ClientID %>_Open(qs);
        }

        function UpdateOrganisation(identityId) {
            var qs = "identityId=" + identityId;
            
            <%=dlgAddUpdateOrganisation.ClientID %>_Open(qs);
        }
            
        function cboClosestTown_Requesting(sender, eventargs) {
            var context = eventargs.get_context();
            context["CountryId"] = 1;
        }
        
        function UpdatePosition(pointId) {
            var qs = "pointId=" + pointId;
            <%=dlgAddUpdatePointGeofence.ClientID %>_Open(qs);
        }
    </script>
    </telerik:RadCodeBlock>
</asp:Content>
