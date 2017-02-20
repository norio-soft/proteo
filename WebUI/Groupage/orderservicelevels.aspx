<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.OrderServiceLevels" MasterPageFile="~/default_tableless.master" Title="Haulier Enterprise" Codebehind="orderservicelevels.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
        function manageServiceLevel(nominalCodeID)
        {
            var url = "manageservicelevel.aspx";
            
            if (nominalCodeID > 0)
                url += "?slID=" + nominalCodeID;

            var wnd = window.radopen("about:blank", "mediuumWindow");
                wnd.SetUrl(url);
                wnd.SetTitle("Manage Service Level");
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Service Levels for Orders</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadWindowManager ID="rmwOrderServiceLevel" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="mediuumWindow" Height="325" Width="500" />
        </Windows>
    </telerik:RadWindowManager>

    <telerik:RadGrid ID="grdServiceLevels" runat="server" Skin="Office2007" AutoGenerateColumns="false">
        <MasterTableView>
            <Columns>
                <telerik:GridHyperLinkColumn  HeaderText="" DataTextField="Description" DataNavigateUrlFormatString="javascript:manageServiceLevel({0})" DataNavigateUrlFields="OrderServiceLevelID" />
                <telerik:GridBoundColumn DataField="NoOfDays" HeaderText="Number of Days" />
                <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created" DataFormatString="{0:dd/MM/yy}" />
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" />
                <telerik:GridBoundColumn DataField="LastUpdateDate" HeaderText="Last Updated" DataFormatString="{0:dd/MM/yy}" />
                <telerik:GridBoundColumn DataField="LastUpdateUserID" HeaderText="Last Updated By" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

    <div class="buttonbar">
        <input type="button" onclick="javascript:manageServiceLevel(-1);" value="Add New Service Level" style="width:145px;" />
        <asp:button id="btnRefresh" runat="server" text="Refresh" />    
    </div>
     
</asp:Content>