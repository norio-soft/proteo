<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="True" Title="Haulier Enterprise" CodeBehind="ExtraType.aspx.cs" Inherits="Orchestrator.WebUI.ExtraType.ExtraType" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Extra Types for Your Organisation</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <telerik:RadGrid runat="server" ID="grdExtraTypes" AutoGenerateColumns="false" Width="700px">
        <MasterTableView CommandItemDisplay="Bottom" DataKeyNames="ExtraTypeId" EditMode="EditForms">
            <EditFormSettings ColumnNumber="2">
                <FormTableItemStyle VerticalAlign="Top" Wrap="true" Width="100%"></FormTableItemStyle>
                <FormMainTableStyle GridLines="Horizontal" CellSpacing="0" CellPadding="3" Width="100%" />
                <FormTableStyle GridLines="None" CellSpacing="0" CellPadding="2" BackColor="white"
                    Width="100%" />
                <FormTableAlternatingItemStyle VerticalAlign="Top" Wrap="true"></FormTableAlternatingItemStyle>
                <EditColumn ButtonType="PushButton" InsertText="Insert" UpdateText="Update" UniqueName="EditCommandColumn1"
                    CancelText="Cancel">
                </EditColumn>
                <FormTableButtonRowStyle BackColor="#FFFFE7" HorizontalAlign="Left"></FormTableButtonRowStyle>
            </EditFormSettings>
            <CommandItemTemplate>
                <asp:Button CssClass="buttonClass" ID="btnAddNewExtraType" CommandName="InitInsert" runat="server" Text="Add New Extra Type">
                </asp:Button>
            </CommandItemTemplate>
            <Columns>
                <telerik:GridEditCommandColumn UniqueName="EditLink" Visible="true">
                    <ItemStyle Width="50px" />
                </telerik:GridEditCommandColumn>
                <telerik:GridBoundColumn HeaderText="ExtraTypeId" DataField="ExtraTypeId" ReadOnly="True"
                    UniqueName="ExtraTypeId" Display="False" />
                <telerik:GridTemplateColumn ItemStyle-Width="50px" UniqueName="ShortDescription" HeaderText="Short Description"
                    DataField="ShortDescription">
                    <ItemTemplate>
                        <div runat="server" id="divShortDescription">
                            <%# String.IsNullOrEmpty((string)((Orchestrator.Entities.ExtraType)Container.DataItem).ShortDescription) ? "-" : (string)((Orchestrator.Entities.ExtraType)Container.DataItem).ShortDescription%></div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <input runat="server" id="txtShortDescription" type="text" value='<%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() ? (String.IsNullOrEmpty((string)((Orchestrator.Entities.ExtraType)Container.DataItem).ShortDescription) ? "-" : (string)((Orchestrator.Entities.ExtraType)Container.DataItem).ShortDescription) : "" %>' />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="150px" UniqueName="Description" HeaderText="Description"
                    DataField="Description">
                    <ItemTemplate>
                        <div runat="server" id="divDescription">
                            <%# String.IsNullOrEmpty((string)((Orchestrator.Entities.ExtraType)Container.DataItem).Description) ? "-" : (string)((Orchestrator.Entities.ExtraType)Container.DataItem).Description%></div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <input runat="server" id="txtDescription" type="text" value='<%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() ? (String.IsNullOrEmpty((string)((Orchestrator.Entities.ExtraType)Container.DataItem).Description) ? "-" : (string)((Orchestrator.Entities.ExtraType)Container.DataItem).Description) : "" %>' />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn Display='<%$ AppSettings: EnableExtraTypeNominalCodes %>'
                    UniqueName="NominalCode" HeaderText="Nominal Code" DataField="NominalCode">
                    <ItemTemplate>
                        <div runat="server" id="divNominalCode">
                            <%# String.IsNullOrEmpty((string)((Orchestrator.Entities.ExtraType)Container.DataItem).NominalCode.Description) ? "-" : (string)((Orchestrator.Entities.ExtraType)Container.DataItem).NominalCode.Description%></div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList Visible="<%$ AppSettings: EnableExtraTypeNominalCodes %>" OnPreRender="PopulateNominalCodes" runat="server" ID="cboNominalCodes">
                        </asp:DropDownList>
                        <asp:CustomValidator runat="server" ID="cvNominalCodes" OnServerValidate="cboNominalCodesValidator_ServerValidate"
                            ControlToValidate="cboNominalCodes" ErrorMessage="You must select a nominal code."></asp:CustomValidator>
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="90px" UniqueName="FuelSurchargeApplies"
                    HeaderText="Fuel Surcharge Applies" DataField="FuelSurchargeApplies">
                    <ItemTemplate>
                        <div runat="server" id="divFuelSurchargeApplies" style='<%# (bool)((Orchestrator.Entities.ExtraType)Container.DataItem).FuelSurchargeApplies == true ? "background-color:#ffffff": "background-color:InfoBackground" %>'>
                            <%# (bool)((Orchestrator.Entities.ExtraType)Container.DataItem).FuelSurchargeApplies == true ? "Yes" : "No"%></div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox runat="server" ID="chkFuelSurchargeApplies" Checked='<%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() ? ((bool)((Orchestrator.Entities.ExtraType)Container.DataItem).FuelSurchargeApplies) : false %>' />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="90px" UniqueName="IsEnabled" HeaderText="Is Enabled" DataField="IsEnabled">
                    <ItemTemplate>
                        <div runat="server" id="divIsEnabled" style='<%# (bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsEnabled == true ? "background-color:#ffffff": "background-color:InfoBackground" %>'>
                            <%# (bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsEnabled == true ? "Yes" : "No"%></div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox runat="server" ID="chkIsEnabled" Checked='<%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() ? ((bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsEnabled) : false %>' />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="90px" UniqueName="IsSystem" HeaderText="Is System" DataField="IsSystem">
                    <ItemTemplate>
                        <div runat="server" id="divIsSystem" style='<%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() && (bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsSystem == true ? "background-color:#ffffff": "background-color:InfoBackground" %>'>
                            <%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() && (bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsSystem == true ? "Yes" : "No"%></div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="90px" UniqueName="IsDisplayedOnAddUpdateOrder" HeaderText="Display on Add/Update order" DataField="IsDisplayedOnAddUpdateOrder" HeaderTooltip="Is treated as a surcharge and added to the Order rate">
                    <ItemTemplate>
                        <div runat="server" id="divIsDisplayedOnAddUpdateOrder" title="This is my tooltip"  >
                            <%# (bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsDisplayedOnAddUpdateOrder == true ? "Yes" : "No"%>

                        </div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox runat="server" ToolTip="Is treated as a surcharge and added to the Order rate" ID="chkIsDisplayedOnAddUpdateOrder" Checked='<%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() ? ((bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsDisplayedOnAddUpdateOrder) : false %>' />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="90px" UniqueName="IsAcceptanceRequired" HeaderText="Is Acceptance Required" DataField="IsAcceptanceRequired">
                    <ItemTemplate>
                        <div runat="server" id="divIsAcceptanceRequired" >
                            <%#((Orchestrator.Entities.ExtraType)Container.DataItem).IsAcceptanceRequired ? "Yes" : "No"%></div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox runat="server" ID="chkIsAcceptanceRequired" Checked='<%# Container.DataItem.GetType() == new Orchestrator.Entities.ExtraType().GetType() ? ((bool)((Orchestrator.Entities.ExtraType)Container.DataItem).IsAcceptanceRequired) : false %>' />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="90px" UniqueName="IsTimeBased" HeaderText="Is Time Based" DataField="IsTimeBased">
                    <ItemTemplate>
                        <div runat="server" id="divIsTimeBased">
                            <%# ((Orchestrator.Entities.ExtraType)Container.DataItem).IsTimeBased ? "Yes" : "No" %>
                        </div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <table>
                            <tr>
                                <td>
                                    <asp:CheckBox runat="server" ID="chkIsTimeBased" CssClass="chkIsTimeBased" OnClick="javascript:ToggleIsTimeBased(this);" />
                                </td>
                            </tr>
                            <tr style="display: none" class="trExtraTimeSelection">
                                <td>
                                    <telerik:RadTimePicker ID="rdiStartTime" runat="server" ToolTip="The start time to the extra time span" Width="100" >
                                        <DateInput runat="server"
                                            DateFormat="HH:mm">
                                        </DateInput>
                                    </telerik:RadTimePicker>
                                </td>
                                <td>
                                    <telerik:RadTimePicker ID="rdiEndTime" runat="server" ToolTip="The end time to the extra time span" Width="100" >
                                        <DateInput runat="server"
                                            DateFormat="HH:mm">
                                        </DateInput>
                                    </telerik:RadTimePicker>
                                </td>
                            </tr>
                        </table>
                    </EditItemTemplate>
   
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-Width="90px" UniqueName="IsDayBased" HeaderText="Is Day Based">
                    <ItemTemplate>
                        <div runat="server" id="divIsDayBased">
                            <%# ((Orchestrator.Entities.ExtraType)Container.DataItem).IsDayBased ? "Yes" : "No" %>
                        </div>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <table style="width: 400px;">
                            <tr>
                                <td>
                                    <asp:CheckBox runat="server" ID="chkIsDayBased" CssClass="chkIsDayBased" OnClick="javascript:ToggleIsDayBased(this);" />
                                </td>
                            </tr>
                            <tr style="display: none" class="trExtraDaySelection">

                                <td>
                                    <asp:CheckBox ID="chkMonday" runat="server"></asp:CheckBox>
                                    <asp:Label runat="server" AssociatedControlID="chkMonday" CssClass="control-label">Monday</asp:Label>
                                    
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkTuesday" runat="server"></asp:CheckBox>
                                    <asp:Label runat="server" AssociatedControlID="chkTuesday" CssClass="control-label">Tuesday</asp:Label>
                                    
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkWednesday" runat="server"></asp:CheckBox>
                                    <asp:Label runat="server" AssociatedControlID="chkWednesday" CssClass="control-label">Wednesday</asp:Label>
                                    
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkThursday" runat="server"></asp:CheckBox>
                                    <asp:Label runat="server" AssociatedControlID="chkThursday" CssClass="control-label">Thursday</asp:Label>
                                </td>

                            </tr>
                            <tr style="display: none" class="trExtraDaySelection">
                                <td>
                                    <asp:CheckBox ID="chkFriday" runat="server"></asp:CheckBox>
                                    <asp:Label runat="server" AssociatedControlID="chkFriday" CssClass="control-label">Friday</asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkSaturday" runat="server"></asp:CheckBox>
                                    <asp:Label runat="server" AssociatedControlID="chkSaturday" CssClass="control-label">Saturday</asp:Label>
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkSunday" runat="server"></asp:CheckBox>
                                    <asp:Label runat="server" AssociatedControlID="chkSunday" CssClass="control-label">Sunday</asp:Label>
                                </td>
                            </tr>
                        </table>
                    </EditItemTemplate>
             </telerik:GridTemplateColumn>


            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

        <script type="text/javascript">
            function pageLoad() {
                if (document.getElementsByClassName('chkIsTimeBased')[0].children[0].checked)
                    document.getElementsByClassName('trExtraTimeSelection')[0].style.display = "inherit";
                else
                    document.getElementsByClassName('trExtraTimeSelection')[0].style.display = "none";

                if (document.getElementsByClassName('chkIsDayBased')[0].children[0].checked) {
                    $('.trExtraDaySelection').each(function (index, tr) {
                        tr.style.display = "inherit";
                    });
                }
                else {
                    $('.trExtraDaySelection').each(function (index, tr) {
                        tr.style.display = "none";
                    });
                }
            }

            function ToggleIsTimeBased(checkbox) {
                if (checkbox.checked)
                    document.getElementsByClassName('trExtraTimeSelection')[0].style.display = "inherit";
                else
                    document.getElementsByClassName('trExtraTimeSelection')[0].style.display = "none";
                
            }

            function ToggleIsDayBased(checkbox) {
                if (checkbox.checked) {
                    $('.trExtraDaySelection').each(function (index, tr) {
                        tr.style.display = "inherit";
                    });
                }
                else {
                    $('.trExtraDaySelection').each(function (index, tr) {
                        tr.style.display = "none";
                    });
                }
            }
        </script>

</asp:Content>