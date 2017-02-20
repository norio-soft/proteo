<%@ Page Language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.administration.usergroups.addupdategroup" CodeBehind="addupdategroup.aspx.cs" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript">
        function onUpdate(oldItem, newItem) {
            return 1;
        }

        function editGrid(rowId) {
            <%= dgUserGroups.ClientID %>.Edit(<%= dgUserGroups.ClientID %>.GetRowFromClientId(rowId));
        }

        function editRow() {
            <%= dgUserGroups.ClientID %>.EditComplete();
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Add/Update User Groups</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>You can add or change the Roles for users here</h2>
    <fieldset>
        <legend><strong>Add New Group</strong></legend>
        <asp:TextBox ID="txtDescription" runat="server" Width="150"></asp:TextBox>
        <asp:Button ID="btnAdd" CssClass="buttonClass" Text="Add" runat="server"></asp:Button>
        &nbsp;
        <asp:Label EnableViewState="False" ID="lblError" runat="server" CssClass="ControlErrorMessage" Visible="False"></asp:Label>
    </fieldset>
    <div style="height: 10px">
    </div>
    <div class="searchDeviant">
        <!-- Need To Add The Grid Here -->
        <componentart:Grid ID="dgUserGroups" EnableViewState="true" AllowTextSelection="true"
            RunningMode="Client" CssClass="Grid" HeaderCssClass="GridHeader" FooterCssClass="GridFooter"
            GroupByTextCssClass="GroupByText" GroupingNotificationTextCssClass="GridHeaderText"
            GroupBySortAscendingImageUrl="group_asc.gif" GroupBySortDescendingImageUrl="group_desc.gif"
            GroupBySortImageWidth="10" GroupBySortImageHeight="10" GroupingPageSize="100"
            ShowHeader="true" ShowSearchBox="False" SearchOnKeyPress="False" PageSize="100"
            PagerStyle="Slider" PagerTextCssClass="GridFooterText" PagerButtonWidth="41"
            PagerButtonHeight="22" SliderHeight="20" PreExpandOnGroup="true" SliderWidth="150"
            SliderGripWidth="9" SliderPopupOffsetX="20" SliderPopupClientTemplateId="SliderTemplate1"
            ImagesBaseUrl="../../images/" PagerImagesFolderUrl="../../images/pager/" TreeLineImagesFolderUrl="../../images/lines/"
            TreeLineImageWidth="22" TreeLineImageHeight="19" Width="100%" runat="server"
            DataKeyField="RoleId" KeyboardEnabled="true" AutoPostBackOnUpdate="true" LoadingPanelClientTemplateId="LoadingFeedbackTemplate"
            LoadingPanelPosition="MiddleCenter" ClientSideOnUpdate="onUpdate" AllowEditing="true">
            <Levels>
                <componentart:GridLevel DataKeyField="RoleId" HeadingCellCssClass="HeadingCell" HeadingRowCssClass="HeadingRow"
                    HeadingTextCssClass="HeadingCellText" DataCellCssClass="DataCell" RowCssClass="Row"
                    SelectedRowCssClass="SelectedRow" SortAscendingImageUrl="asc.gif" SortDescendingImageUrl="desc.gif"
                    SortImageWidth="10" SortImageHeight="10" GroupHeadingCssClass="GroupHeading"
                    ShowTableHeading="false" EditCellCssClass="EditDataCell" EditFieldCssClass="EditDataField"
                    EditCommandClientTemplateId="EditCommandTemplate">
                    <Columns>
                        <componentart:GridColumn HeadingText="Group Id" Visible="false" DataField="RoleId" />
                        <componentart:GridColumn HeadingText="Description" DataField="Description" />
                        <componentart:GridColumn HeadingText="Edit Command" DataCellClientTemplateId="EditTemplate"
                            EditControlType="EditCommand" Width="100" Align="Center" />
                    </Columns>
                </componentart:GridLevel>
            </Levels>
            <ClientTemplates>
                <componentart:ClientTemplate ID="EditTemplate">
                    <a href="javascript:editGrid('## DataItem.ClientId ##');">Edit</a>
                </componentart:ClientTemplate>
                <componentart:ClientTemplate ID="EditCommandTemplate">
                    <a href="javascript:editRow();">Update</a> | <a href="javascript:dgUserGroups.EditCancel();">
                        Cancel</a>
                </componentart:ClientTemplate>
                <componentart:ClientTemplate ID="SliderTemplate1">
                    <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td valign="top" style="padding: 5px;">
                                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td>
                                            <table cellspacing="0" cellpadding="2" border="0" style="width: 255px;">
                                                <tr>
                                                    <td style="font-family: verdana; font-size: 11px;">
                                                        <div style="overflow: hidden; width: 115px;">
                                                            Desciption:<nobr>## DataItem.GetMember("Description").Value ##</nobr></div>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td colspan="2">
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="height: 14px; background-color: #757598;">
                                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td style="padding-left: 5px; color: white; font-family: verdana; font-size: 10px;">
                                            Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgUserGroups.PageCount ##</b>
                                        </td>
                                        <td style="padding-right: 5px; color: white; font-family: verdana; font-size: 10px;"
                                            align="right">
                                            Extra States <b>## DataItem.Index + 1 ##</b> of <b>## dgUserGroups.RecordCount ##</b>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </componentart:ClientTemplate>
                <componentart:ClientTemplate ID="LoadingFeedbackTemplate">
                    <table cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td style="font-size: 10px; font-family: Verdana;">
                                Loading...&nbsp;
                            </td>
                            <td>
                                <img src="../../images/spinner.gif" width="16" height="16" border="0">
                            </td>
                        </tr>
                    </table>
                </componentart:ClientTemplate>
            </ClientTemplates>
        </componentart:Grid>
    </div>
</asp:Content>
