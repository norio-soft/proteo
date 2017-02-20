<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.jobHistory" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" Codebehind="jobHistory.aspx.cs" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Run History</asp:Content>
<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">

</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="margin-bottom:10px;">
        <asp:GridView id="gvJobHistory" runat="server" CssClass="Grid" CellPadding="3" CellSpacing="0" AllowSorting="false" AutoGenerateColumns="false" ShowFooter="true" width="100%">
            <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
            <FooterStyle height="22" />
            <RowStyle  cssclass="Row" />
            <AlternatingRowStyle  backcolor="WhiteSmoke" />
            <SelectedRowStyle  cssclass="SelectedRow" />
            <Columns>
            <asp:BoundField HeaderText="Event" DataField="Description" ItemStyle-Width="30%" />
            <asp:BoundField HeaderText="Details" DataField="Details" ItemStyle-Width="40%" />
            <asp:BoundField HeaderText="User" DataField="CreateUserId" ItemStyle-Width="15%" />
            <asp:BoundField HeaderText="Time" DataField="CreateDate" DataFormatString="{0:dd/MM/yy HH:mm}" HtmlEncode="False" ItemStyle-Width="20%" />
            </Columns>
        </asp:GridView>
    </div>
    
    <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false"></cs:WebModalWindowHelper>
                            
    <div class="buttonbar">
        <asp:Button ID="btnClose" runat="server" Text="Close" width="75" />
    </div>                        
</asp:Content>     