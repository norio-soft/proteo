<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="GPSMappingList.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Vehicle.GPSMappingList" Title="Vehicle List" %>
<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>GPS Mapping Details</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>
    
    <script type="text/javascript" language="javascript">
        function showPosition(gpsUnitID) {          
            var qs = "uid=" + gpsUnitID ;
        
            <%=dlgGetCurrentLocation.ClientID %>_Open(qs);
        }

        function openInNewWindow(resourceID) {
            var qs = "resourceId=" + resourceID ;
        
            <%=dlgAddUpdateVehicle.ClientID %>_Open(qs);
        }

        function openAssignedJobs(resourceID) {
            var qs = "resourceId=" + resourceID ;
        
            <%=dlgJobsAssigned.ClientID %>_Open(qs);
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
            
    <cc1:Dialog ID="dlgJobsAssigned" runat="server" Width="580" Height="650" Mode="Normal" URL="/Resource/Vehicle/VehicleAssignedJobs.aspx" AutoPostBack="true" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgAddUpdateVehicle" runat="server" Width="550" Height="640" Mode="Normal" URL="/Resource/Vehicle/addupdatevehicle.aspx" AutoPostBack="true" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgGetCurrentLocation" runat="server" Width="630" Height="600" Mode="Normal" URL="/gps/getcurrentlocation.aspx" AutoPostBack="true" ReturnValueExpected="true"></cc1:Dialog>

    <div style="background-color:White; border:solid 1pt black; margin-bottom:5px;">
        <table style="width:100%; " cellpadding="0" cellspacing="0">
            <tr>
                <td>
                    <table style="width:100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="border-bottom:solid 1pt black; background-color:silver; padding:3px; margin-bottom:3px;" colspan="5"><span style="font-weight:bold; font-family:Verdana; font-size:11px; color:White;" >Vehciles and GPS Unit Details (<asp:Literal ID="litVehicleCount" runat="server"></asp:Literal>)</span></td>
                        </tr>
                        <tr height="38">
                            <td width="150" style="padding-left:3px;">
                                <telerik:RadComboBox ID="cboFilter" runat="server" AutoPostBack="true">
                                <Items>
                                    <telerik:RadComboBoxItem Value="-1" Text="-- Select Filter --" />
                                    <telerik:RadComboBoxItem Value="0" Text="GPS Status" />
                                    <telerik:RadComboBoxItem Value="1" Text="GPS Mapping Status" />
                                    <telerik:RadComboBoxItem Value="2" Text="Vehicle Controller" />
                                    <telerik:RadComboBoxItem Value="3" Text="Search" />
                                </Items>
                                </telerik:RadComboBox>
                            </td>
                            <td style="width:350px;">
                                <asp:MultiView ID="mvFilterOptions" runat="server">
                                    <asp:View ID="vwGPSStatus" runat="server">
                                        <asp:RadioButtonList ID="rblGPSStatus" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem  Value="RED"><img src="/images/icons/tlred.gif"  /></asp:ListItem>
                                            <asp:ListItem  Value="AMBER"><img src="/images/icons/tlamber.gif" /></asp:ListItem>
                                            <asp:ListItem  Value="GREEN"><img src="/images/icons/tlgreen.gif" /></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </asp:View>
                                    <asp:View ID="vwMappingStatus" runat="server">
                                        <asp:RadioButtonList runat="server" ID="rblMappingStatus" RepeatDirection="Horizontal">
                                            <asp:ListItem Text="Show Vehicles with NO mapping" Value="0" />
                                            <asp:ListItem Text="Show Vehicles with mapping" Value="1" />
                                        </asp:RadioButtonList>
                                    </asp:View>
                                    <asp:View ID="vwVehicleController" runat="server">
                                        Please select
                                        <telerik:RadComboBox runat="server" ID="cboVehicleController"></telerik:RadComboBox>
                                    </asp:View>
                                    <asp:View ID="vwSearch" runat="server">
                                        &nbsp;&nbsp;<asp:TextBox ID="txtSearch" runat="server" Width="150"></asp:TextBox>
                                    </asp:View>
                                </asp:MultiView>
                            </td>
                            <td style="text-align:left;"><asp:Button ID="btnFilter" runat="server" Text="Filter" width="75px" Visible="false"  /><asp:Button ID="btnReset" runat="server" Text="Clear Filter" width="75px" Visible="false"/></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </div>    
        
     <telerik:RadGrid runat="server" ID="rgVehicles" AutoGenerateColumns="false" AllowSorting="true" >
        <MasterTableView>
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Reg No">
                    <ItemTemplate>
                         <a href="javascript:openInNewWindow(<%#Eval("ResourceId")%>)">
                            <div style="color: Blue;">
                                <%#Eval("RegNo")%>
                            </div>
                        </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Assigned Jobs">
                    <ItemTemplate>  
                        <a href="javascript:openAssignedJobs('<%#Eval("ResourceID") %>')">
					        Assigned Jobs 
				        </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="VehicleType" HeaderText="Vehicle Type"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="VehicleManufacturer" HeaderText="Manufacturer"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CurrentVehicleController" HeaderText="Vehicle Controller"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="GPSUnitID" HeaderText="GPS Unit ID"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DateStamp" HeaderText="GPS Last Update"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Reason" HeaderText="GPS Last Update Type"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="GPS Status" SortExpression="GPSStatus">
                    <ItemTemplate>
                        <img src="<%#String.Format("/images/icons/tl{0}.gif", Eval("GPSStatus").ToString().ToLower()) %>" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Show On Map">
                    <ItemTemplate>                       
                        <%#(Eval("GPSUnitID") != DBNull.Value && Eval("GPSUnitID") != String.Empty) ? String.Format("<a href=\"javascript:showPosition('{0}')\">{1}</a>", Eval("GPSUnitID"), "Show on Map") : ""%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    
</asp:Content>