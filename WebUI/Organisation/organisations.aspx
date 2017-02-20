<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Organisation_organisations" Title="Client List" Codebehind="organisations.aspx.cs"   %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Client List</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        function doFilter(sender, e) {
            if (e.keyCode == 13) {
                var btn = document.getElementById('<%= grdOrganisations.MasterTableView.GetItems(Telerik.Web.UI.GridItemType.CommandItem)[0].FindControl("btnSearch").ClientID %>');

                if (btn != null) {
                    e.cancelBubble = true;
                    e.returnValue = false;

                    if (e.preventDefault) {
                        e.preventDefault();
                    }

                    btn.click();
                }
            }
        }

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
    
    <h2><asp:Label ID="lblPageDescription" runat="server" /></h2>
    
    <div class="buttonbar" style="margin-bottom:5px; display: none;">
        <asp:Button ID="btnExportTop" class="buttonClassSmall" runat="server" Text="Export" Width="75" />
    </div>
    
    <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: none;">
        <fieldset>
            <table style="margin-top:30px">    
                <tr>
                    <td class="formCellLabel">Only Show Active Organisations</td>
                    <td class="formCellField"><asp:RadioButton ID="optShowActive" runat="server" Checked="true" AutoPostBack="true" GroupName="FilterOptions" /></td>
                    <td class="formCellLabel">Only Show Organisations Missing a Credit Application Form</td>
                    <td class="formCellField"><asp:RadioButton ID="optShowMissingCreditApplicationForm" runat="server" AutoPostBack="true" GroupName="FilterOptions"  /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Show Suspended As Well as Active Organisations</td>
                    <td class="formCellField"><asp:RadioButton ID="optShowSuspended" runat="server" AutoPostBack="true" GroupName="FilterOptions" /></td>
                    <td class="formCellLabel">Only Show Organisations Missing Client T&Cs</td>
                    <td class="formCellField"><asp:RadioButton ID="optShowMissingClientTnCs" runat="server" AutoPostBack="true" GroupName="FilterOptions" /></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                    <td class="formCellLabel">Only Show Subcontractors Missing SubContractor T&Cs</td>
                    <td class="formCellField"><asp:RadioButton  ID="optShowMissingSubbyTnCs" runat="server" AutoPostBack="true" GroupName="FilterOptions" /></td>
                </tr>
            </table>
        </fieldset>
    </div>
    
    <telerik:RadGrid EnableAJAX="true" ID="grdOrganisations" runat="server" AllowSorting="true" AutoGenerateColumns="false" Skin="Orchestrator" SkinsPath="/RadControls/Grid/Skins" Width="100%" AllowCustomPaging="true">
        <MasterTableView CommandItemDisplay="Top">
            <CommandItemTemplate>
                <div style="float: left; font-size: 1.1em; width: 800px;">
                    <input type="button" class="buttonClassSmall" value="Add New Client" style="width: 105px" onclick="location.href='addupdateorganisation.aspx'" />
                    <asp:Button ID="btnExport" runat="server" Text="Export" CommandName="btnExport" class="commandItemButton"/>
                    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()">Show filter Options</div>
                    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()" style="display: none;">Close filter Options</div>
                    <div style="vertical-align:middle; padding: 5px 0 0 10px;">
                        <asp:LinkButton ID="LinkButton0" runat="server" CommandArgument="%" CommandName="alpha" Text="All" />&#160
                        <asp:LinkButton ID="LinkButton1" runat="server" CommandArgument="NUMS" CommandName="alpha" Text="Nums" />&#160;
                        <asp:LinkButton ID="LinkButton2" runat="server" CommandArgument="A" CommandName="alpha" Text="A" />&#160;
                        <asp:LinkButton ID="LinkButton3" runat="server" CommandArgument="B" CommandName="alpha" Text="B" />&#160;
                        <asp:LinkButton ID="LinkButton4" runat="server" CommandArgument="C" CommandName="alpha" Text="C" />&#160;
                        <asp:LinkButton ID="LinkButton5" runat="server" CommandArgument="D" CommandName="alpha" Text="D" />&#160;
                        <asp:LinkButton ID="LinkButton6" runat="server" CommandArgument="E" CommandName="alpha" Text="E" />&#160;
                        <asp:LinkButton ID="LinkButton7" runat="server" CommandArgument="F" CommandName="alpha" Text="F" />&#160;
                        <asp:LinkButton ID="LinkButton8" runat="server" CommandArgument="G" CommandName="alpha" Text="G" />&#160;
                        <asp:LinkButton ID="LinkButton9" runat="server" CommandArgument="H" CommandName="alpha" Text="H" />&#160;
                        <asp:LinkButton ID="LinkButton10" runat="server" CommandArgument="I" CommandName="alpha" Text="I" />&#160;
                        <asp:LinkButton ID="LinkButton11" runat="server" CommandArgument="J" CommandName="alpha" Text="J" />&#160;
                        <asp:LinkButton ID="LinkButton12" runat="server" CommandArgument="K" CommandName="alpha" Text="K" />&#160;
                        <asp:LinkButton ID="LinkButton13" runat="server" CommandArgument="L" CommandName="alpha" Text="L" />&#160;
                        <asp:LinkButton ID="LinkButton14" runat="server" CommandArgument="M" CommandName="alpha" Text="M" />&#160;
                        <asp:LinkButton ID="LinkButton15" runat="server" CommandArgument="N" CommandName="alpha" Text="N" />&#160;
                        <asp:LinkButton ID="LinkButton16" runat="server" CommandArgument="O" CommandName="alpha" Text="O" />&#160;
                        <asp:LinkButton ID="LinkButton17" runat="server" CommandArgument="P" CommandName="alpha" Text="P" />&#160;
                        <asp:LinkButton ID="LinkButton18" runat="server" CommandArgument="Q" CommandName="alpha" Text="Q" />&#160;
                        <asp:LinkButton ID="LinkButton19" runat="server" CommandArgument="R" CommandName="alpha" Text="R" />&#160;
                        <asp:LinkButton ID="LinkButton20" runat="server" CommandArgument="S" CommandName="alpha" Text="S" />&#160;
                        <asp:LinkButton ID="LinkButton21" runat="server" CommandArgument="T" CommandName="alpha" Text="T" />&#160;
                        <asp:LinkButton ID="LinkButton22" runat="server" CommandArgument="U" CommandName="alpha" Text="U" />&#160;
                        <asp:LinkButton ID="LinkButton23" runat="server" CommandArgument="V" CommandName="alpha" Text="V" />&#160;
                        <asp:LinkButton ID="LinkButton24" runat="server" CommandArgument="W" CommandName="alpha" Text="W" />&#160;
                        <asp:LinkButton ID="LinkButton25" runat="server" CommandArgument="X" CommandName="alpha" Text="X" />&#160;
                        <asp:LinkButton ID="LinkButton26" runat="server" CommandArgument="Y" CommandName="alpha" Text="Y" />&#160;
                        <asp:LinkButton ID="LinkButton27" runat="server" CommandArgument="Z" CommandName="alpha" Text="Z" />
                    </div>
                </div>
                <div style="float: right;">
                    <asp:Label ID="Label50" runat="server" Text="Filter:"></asp:Label>
                    <asp:TextBox ID="txtSearch" onkeypress="doFilter(this, event);" runat="server"></asp:TextBox>
                    <asp:Button ID="btnSearch" runat="server" CommandName="search" Text="Filter" />
                </div>
            </CommandItemTemplate>
            <Columns>
                <telerik:GridHyperLinkColumn DataTextField="OrganisationName" DataNavigateUrlFormatString="addupdateorganisation.aspx?identityId={0}"
                    DataNavigateUrlFields="IdentityId">
                </telerik:GridHyperLinkColumn>
                <telerik:GridBoundColumn DataField="PhoneNumber" HeaderText="Phone">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AccountCode" HeaderText="Account Code" SortExpression="AccountCode">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AddressLine1" HeaderText="Address Line 1">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AddressLine2" HeaderText="Address Line 2">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="AddressLine3" HeaderText="Address Line 3">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PostTown" HeaderText="Town">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="County" HeaderText="County">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PostCode" HeaderText="PostCode">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="InvoiceEmailAddress" HeaderText="Invoice Email">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    

</asp:Content>
