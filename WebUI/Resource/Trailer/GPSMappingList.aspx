<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="GPSMappingList.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Trailer.GPSMappingList" Title="Trailers and GPS Unit Details" %>
<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Trailers and GPS Unit Details (<asp:Literal ID="litTrailerCount" runat="server"></asp:Literal>)</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script type="text/javascript" language="javascript" src="../script/tooltippopups.js"></script>
            
<!--<h2>Description here</h2>-->
        <cc1:Dialog ID="dlgJobsAssigned" runat="server" Width="580" Height="650" Mode="Normal" URL="/Resource/Trailer/TrailerAssignedJobs.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>
        <cc1:Dialog ID="dlgAddUpdateVehicle" runat="server" Width="550" Height="640" Mode="Normal" URL="/Resource/Trailer/addupdatetrailer.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>
        <cc1:Dialog ID="dlgGetCurrentLocation" runat="server" Width="700" Height="650" Mode="Normal" URL="/gps/getcurrentlocation.aspx" 
            AutoPostBack="true" ReturnValueExpected="true">
        </cc1:Dialog>
<fieldset>
    <legend>Filter Options</legend>
    <table>
        <tr height="38">
            <td width="150" style="padding-left:3px;">   
                <telerik:RadComboBox ID="cboFilter" runat="server" AutoPostBack="true">
                    <Items>
                        <telerik:RadComboBoxItem Value="-1" Text="-- Select Filter --" />
                        <telerik:RadComboBoxItem Value="0" Text="GPS Status" />
                        <telerik:RadComboBoxItem Value="1" Text="GPS Mapping Status" />
                        <telerik:RadComboBoxItem Value="2" Text="Trailer Controller" />
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
                            <asp:ListItem Text="Show trailers with NO mapping" Value="0" />
                            <asp:ListItem Text="Show trailers with mapping" Value="1" />
                        </asp:RadioButtonList>
                    </asp:View>
                    <asp:View ID="vwTrailerController" runat="server">
                        Please select
                        <telerik:RadComboBox runat="server" ID="cboTrailerController"></telerik:RadComboBox>
                    </asp:View>
                    <asp:View ID="vwSearch" runat="server">
                        &nbsp;&nbsp;<asp:TextBox ID="txtSearch" runat="server" Width="150"></asp:TextBox>
                    </asp:View>
                </asp:MultiView>
            </td>
            <td style="text-align:left;">
                <asp:Button ID="btnFilter" runat="server" CssClass="buttonClass" Text="Filter" Visible="false"  />
                <asp:Button ID="btnReset" runat="server" CssClass="buttonClass" Text="Clear Filter" Visible="false"/>
            </td>
        </tr>
    </table> 
</fieldset>

<telerik:RadGrid runat="server" ID="rgTrailers" Skin="Orchestrator" AutoGenerateColumns="false" AllowSorting="true" >
    <MasterTableView>
        <Columns>   
            <telerik:GridTemplateColumn HeaderText="Trailer Ref">
                <ItemTemplate>  
                    <a href="javascript:openInNewWindow('<%#Eval("ResourceID") %>')">
					    <%# Eval("TrailerRef").ToString().Trim() %>
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
            <telerik:GridBoundColumn DataField="TrailerType" HeaderText="Trailer Type"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="TrailerDescription" HeaderText="Trailer Desc"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="CurrentTrailerController" HeaderText="Trailer Controller"></telerik:GridBoundColumn>
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

<script type="text/javascript">

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

    $(document).ready(function() {
        $('#<%=rgTrailers.ClientID %>_ctl01 tbody tr').quicksearch({
            position: 'before',
            attached: '#ctl00_ContentPlaceHolder1_rgTrailers_ctl01',
            labelText: 'Filter the trailer list ',
            delay: 100
        });

    });

    </script>
    
</asp:Content>

