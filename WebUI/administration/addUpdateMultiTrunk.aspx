<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addUpdateMultiTrunk.aspx.cs"
    Inherits="Orchestrator.WebUI.administration.addUpdateMultiTrunk" MasterPageFile="~/WizardMasterPage.master"
    Title="Add-Update Multi-Trunk" %>

<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add-Update Multi-Trunk</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <fieldset>
        <legend>Multi-Trunk Maintenance</legend>
        <table>
        <tr>
            <td class="formCellLabel">Multi-Trunk Description</td>
            <td class="formCellField">
                <asp:TextBox ID="txtMultiTrunkName" runat="server" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtMultiTrunkName" ErrorMessage="*" ValidationGroup="grpMultiTrunkDescription" ToolTip="Please supply a description for the multi-trunk." Display="Dynamic">
                    <img src="../images/Error.gif" width="16" height="16" alt="Please supply a description for the multi-trunk." />
                </asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Is Enabled</td>
            <td class="formCellField">
                <asp:CheckBox ID="chkIsEnabled" Checked="true" runat="server" Text="" />
            </td>
        </tr>
        </table>
    </fieldset>
    <fieldset>
        <legend>Add Trunk Point</legend>
        <div style="width: 400px;">
            <p1:Point style="float: left;" runat="server" ID="ucCollectionPoint" ShowFullAddress="true"
                CanClearPoint="true" CanUpdatePoint="true" ShowPointOwner="true" Visible="true" />
                <table runat="server" width="400px">
                <tr runat="server" id="tblTimeFromLastPointRow">
                    <td>
                        Time from last point (mins)
                    </td>
                    <td align="right">
                        <telerik:RadNumericTextBox style="text-align:right; width:100px;" ID="txtDefaultDuration" runat="server" Width="120px" ToolTip="Please enter the default duration for this point.">
                        </telerik:RadNumericTextBox>
                        <asp:RequiredFieldValidator ID="rfvTimeFromLastPoint" runat="server" ControlToValidate="txtDefaultDuration" ErrorMessage="*" ToolTip="Please supply the time from last point." Display="Dynamic">
                        <img src="../images/Error.gif" width="16" height="16" alt="Please supply the time from last point." />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="2" align="right">
                    <div class="buttonbar">
                        <asp:Button ID="btnAddTrunkPoint" runat="server" Text="Add Trunk Point" Width="110"  />
                    </div>
                    </td>
                </tr>
            </table>
        </div>
    </fieldset>
    <fieldset>
        <legend>Current Trunk Points</legend>
        <telerik:RadAjaxPanel ID="repeaterPanel" runat="server">
            <asp:Repeater ID="repeatCurrentTrunkPoints" runat="server">
                <HeaderTemplate>
                    <table class="Grid" width="500" cellpadding="3" cellspacing="0">
                    <thead class="HeadingRowLite">
                        <tr>
                        <th>
                            Point Description
                        </th>
                        <th colspan="4">
                            Time from last point (mins)
                        </th>
                        </tr>
                    </thead>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr class="Row">
                        <td align="left">
                            <asp:Label ID="lblPointDescription" runat="server" Text='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Point.Description %>'></asp:Label>
                        </td>
                        <td runat="server" style="width:60px" align="left">
                            <telerik:RadNumericTextBox style="text-align:right; width:50px;" ID="txtMinutesFromPreviousPoint" runat="server" Text='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).MinutesFromPreviousPoint.ToString() %>' Width="120px" ToolTip="Please enter the default duration for this point.">
                            </telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgBtnDown" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-arrow-down.png"
                                CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Order %>' 
                                CommandName="Down" ToolTip="Move trunk point down" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgBtnUp" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-arrow-up.png"
                                CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Order %>'
                                CommandName="Up" ToolTip="Move trunk point up" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgBtnDelete" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-cross.png"
                                CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Order %>'
                                CommandName="Delete" ToolTip="Delete trunk point" />
                        </td>
                    </tr>
                </ItemTemplate>
                <AlternatingItemTemplate>
                    <tr class="AlternatingRow">
                        <td align="left">
                            <asp:Label ID="lblPointDescription" runat="server" Text='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Point.Description %>'></asp:Label>
                        </td>
                        <td style="width:60px" align="left">
                            <telerik:RadNumericTextBox style="text-align:right; width:50px;" ID="txtMinutesFromPreviousPoint" runat="server" Text='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).MinutesFromPreviousPoint.ToString() %>' Width="120px" ToolTip="Please enter the default duration for this point.">
                            </telerik:RadNumericTextBox>
                        </td>
                        <td>
                            <asp:ImageButton ID="imgBtnDown" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-arrow-down.png"
                                CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Order %>'
                                CommandName="Down" ToolTip="Move trunk point down" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgBtnUp" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-arrow-up.png"
                                CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Order %>'
                                CommandName="Up" ToolTip="Move trunk point up" />
                        </td>
                        <td>
                            <asp:ImageButton ID="imgBtnDelete" runat="server" ImageUrl="/App_Themes/Orchestrator/img/masterpage/icon-cross.png"
                                CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.MultiTrunkPoint)Container.DataItem).Order %>'
                                CommandName="Delete" ToolTip="Delete trunk point" />
                        </td>
                    </tr>
                </AlternatingItemTemplate>
                <FooterTemplate>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </telerik:RadAjaxPanel>
    </fieldset>
    <div class="buttonbar">
        <asp:button id="btnCancel" CausesValidation="false" OnClientClick="CloseWindow();" runat="server" text="Cancel" Width="100" />
        <asp:button id="btnSaveMultiTrunk" runat="server" ValidationGroup="grpMultiTrunkDescription" text="Save Multi-Trunk" Width="120" />
    </div>
    <div class="cleardiv"></div>
    
    <script language="javascript" type="text/javascript">
        function CloseWindow() {
            self.close();
        }

    </script>
</asp:Content>
