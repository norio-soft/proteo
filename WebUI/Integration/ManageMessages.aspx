<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="ManageMessages.aspx.cs" Inherits="Orchestrator.WebUI.Integration.ManageMessages" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Register TagPrefix="p1" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">

        function SetFilterArea() {
            var width = $("#overlayedClearFilterBox").width();
            var height = $("#overlayedClearFilterBox").height();
            var position = $("#overlayedClearFilterBox").position();
            $("#overlayedIframe").css("width", width + 10);
            $("#overlayedIframe").css("height", height + 25);
            $("#overlayedIframe").css("top", position.top);
            $("#overlayedIframe").css("left", position.left);
        }

        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });

            SetFilterArea();

            var fr = document.getElementById("overlayedIframe");
            fr.style.display = "block";
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
            var fr = document.getElementById("overlayedIframe");
            fr.style.display = "none";

        }

     

    </script>
   
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Manage Import/Export Messages</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnRefresh">
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
       <iframe id="overlayedIframe" style="position: absolute; z-index: 95; background:white;"></iframe>
       <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block; padding-bottom:5px;">
        <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td runat="server" id="tdDateOptions" >
                        <table>
                            <tr>
                                <td class="formCellLabel">Date From</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="rdiStartDate" runat="server" ToolTip="The start date for the filter." Width="100px" >
                                    <DateInput ID="DateInput1" runat="server"
                                     DateFormat="dd/MM/yy">
                                     </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate" ValidationGroup="grpRefresh">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please enter a Start Date." alt="" />
                                    </asp:RequiredFieldValidator>    
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Date To</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="rdiEndDate" runat="server"  ToolTip="The end date for the filter." Width="100px"  >
                                    <DateInput ID="DateInput2" runat="server"
                                    DateFormat="dd/MM/yy" >
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate" ValidationGroup="grpRefresh">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please enter an End Date." alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                                
                            </tr>
                            <tr>
                                <td class="formCellLabel">Message Type</td>
                                <td class="formCellField">
                                    <asp:RadioButtonList ID="cboMessageType" runat="server" RepeatDirection="horizontal" RepeatColumns="4">
                                        <asp:ListItem Text="Import" Value="IMPORT"></asp:ListItem>
                                        <asp:ListItem Text="Export" Value="EXPORT"></asp:ListItem>
                                        <asp:ListItem Text="Both" Value="BOTH" Selected="true"></asp:ListItem>
                                   </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">System</td>
                                <td class="formCellField">
                                    <telerik:RadComboBox ID="cboSystem" runat="server" DataTextField="System" DataValueField="System" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False" ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" Width="343px" Height="300px"></telerik:RadComboBox>
                                </td>
                            </tr>
                            
                       </table>
                    </td>
                </tr>
            </table>
            
        </fieldset>
        <div class="buttonBar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" />
            </div>
        </div>
        
        <telerik:RadGrid runat="server" ID="grdMessageList" Skin="Orchestrator"
            AllowFilteringByColumn="false" AllowPaging="true" AllowSorting="true" AllowMultiRowSelection="true"
            EnableAJAX="true" AutoGenerateColumns="false" ShowStatusBar="false" PageSize="250">
            <ClientSettings AllowDragToGroup="true" AllowColumnsReorder="true">
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="false"></Selecting>
                 
            </ClientSettings>
           
            <PagerStyle NextPageText="Next &amp;gt;" PrevPageText="&amp;lt; Prev" Mode="NextPrevAndNumeric">
            </PagerStyle>

            <MasterTableView DataKeyNames="MessageID" ClientDataKeyNames="MessageID" AutoGenerateColumns="false">
                <RowIndicatorColumn Display="false">
                </RowIndicatorColumn>
                <Columns>
                    <telerik:GridBoundColumn UniqueName="MessageID" DataField="MessageID" HeaderText="Message ID" Display="false" />
                    <telerik:GridBoundColumn UniqueName="Type" DataField="Type" HeaderText="Import/Export" />
                    <telerik:GridBoundColumn UniqueName="System" DataField="System" HeaderText="System" SortExpression="System" AllowFiltering="false" />
                    <telerik:GridBoundColumn UniqueName="State" DataField="State" HeaderText="State" AllowSorting="false" />
                    <telerik:GridBoundColumn UniqueName="Message" DataField="Message" HeaderText="Message" AllowSorting="false" AllowFiltering="false" />
                    <telerik:GridBoundColumn UniqueName="Alert" DataField="Alert" HeaderText="Reason For Failure" AllowSorting="false" AllowFiltering="false" />
                    <telerik:GridBoundColumn UniqueName="OrderID" DataField="OrderID" HeaderText="Order ID" />
                    <telerik:GridBoundColumn UniqueName="CreateDate" DataField="CreateDate" HeaderText="Create Date" AllowFiltering="false" />
                    <telerik:GridBoundColumn UniqueName="CreateUserId" DataField="CreateUserId" HeaderText="Created By" />
                    <telerik:GridButtonColumn CommandName="Select"  Text="Retry" UniqueName="Select" HeaderText="Retry Message" ButtonType="PushButton" ConfirmText="Are you sure that you would like to reprocess this message?"></telerik:GridButtonColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>

    </asp:Panel>
</asp:Content>
