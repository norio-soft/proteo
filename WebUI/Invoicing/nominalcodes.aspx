<%@ Page Language="C#" AutoEventWireup="true" Inherits="Invoicing_nominalcodes" MasterPageFile="~/default_tableless.Master" Codebehind="nominalcodes.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
        function manageNominalCode(nominalCodeID) {
            var url = "managenominalcode.aspx";
            if (nominalCodeID > 0)
                url += "?nc=" + nominalCodeID;

            var wnd = window.radopen("about:blank", "MediumWindow");
            wnd.SetUrl(url);
            wnd.SetTitle("Manage Nominal Code");
        }
    </script> 
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Nominal Code Administration</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadWindowManager ID="rmwNominalCodes" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="MediumWindow" Height="350px" Width="600" />
        </Windows>
    </telerik:RadWindowManager>
    
    <telerik:RadGrid ID="grdNominalCodes" runat="server" Skin="Office2007" AutoGenerateColumns="false" Width="700">
        <MasterTableView>
            <Columns>
                <telerik:GridHyperLinkColumn DataTextField="NominalCode" HeaderText="Nominal Code" DataNavigateUrlFormatString="javascript:manageNominalCode({0})" DataNavigateUrlFields="NominalCodeID"></telerik:GridHyperLinkColumn>
                <telerik:GridBoundColumn DataField="Description" HeaderText="Description"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Active">  
                    <ItemTemplate>  
                        <asp:Label ID="lblIsActive" runat="server" Text='<%# Convert.ToBoolean(Eval("IsActive")) == true ? "Yes" : "No" %>'></asp:Label>  
                    </ItemTemplate>  
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastUpdateDate" HeaderText="Last Updated" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastUpdateUserID" HeaderText="Last Updated By"></telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <div class="buttonbar">
        <input type="button" onclick="javascript:manageNominalCode(-1);" value="Add New Code" style="width:125px;" />
        <asp:button id="btnRefresh" runat="server" text="Refresh" />    
    </div>
    <div style="display:none;">
        <h1>Default Nominal Codes for Job Types</h1>
        <telerik:RadGrid ID="grdDefaultCodes" runat="server" Skin="Office2007" AutoGenerateColumns="false" width="300">
            <MasterTableView>
                <Columns>
                    <telerik:GridBoundColumn DataField="JobType" HeaderText="JobType"></telerik:GridBoundColumn>
                    <telerik:GridHyperLinkColumn DataTextField="NominalCode" HeaderText="Nominal Code" DataNavigateUrlFormatString="javascript:manageNominalCode({0})" DataNavigateUrlFields="NominalCodeID"></telerik:GridHyperLinkColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>
</asp:Content>