<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="DepotList.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.DepotList" %>
<%@ Register TagPrefix="uc" TagName="Point" Src="~/UserControls/point.ascx" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Depots</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <fieldset runat="server" class="invisiableFieldset" id="fsDepots">
        <table>
            <tr>
                <td>Network&nbsp;</td>
                <td><telerik:RadComboBox runat="server" ID="cboNetworks" AutoPostBack="true" /></td>
            </tr>
        </table>

        <telerik:RadGrid ID="grdDepots" runat="server" AutoGenerateColumns="false" >
            <MasterTableView CommandItemDisplay="Bottom" DataKeyNames="DepotId" >
                <RowIndicatorColumn>
                    <HeaderStyle Width="20px"></HeaderStyle>
                </RowIndicatorColumn>
                <ExpandCollapseColumn>
                    <HeaderStyle Width="20px"></HeaderStyle>
                </ExpandCollapseColumn>
                <SortExpressions>
                    <telerik:GridSortExpression FieldName="Code" SortOrder="Ascending" />
                </SortExpressions>
                <Columns>
                    <telerik:GridEditCommandColumn UniqueName="Edit" ButtonType="LinkButton" EditText="Edit">
                    </telerik:GridEditCommandColumn>
                    <telerik:GridBoundColumn DataField="Code" HeaderText="Depot">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PointDescription" HeaderText="Point">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="HubIdentifier" HeaderText="Hub Identifier">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PrintOnLabel" HeaderText="Print on Label">
                    </telerik:GridBoundColumn>
                    <telerik:GridButtonColumn CommandName="Delete" Text="Delete" UniqueName="column">
                    </telerik:GridButtonColumn>
                </Columns>
                <CommandItemTemplate>
                    <asp:Button ID="btnAdd" runat="server" CssClass="buttonClass" Width="75px" Text="Add" CommandName="InitInsert"/>                
                </CommandItemTemplate>
                <EditFormSettings EditFormType="Template" >
                    <FormTemplate>
                        <table>
                            <tr>
                                <td valign="top" style="padding-top:6px" >Code<br />
                                    <asp:TextBox runat="server" ID="txtCode" CssClass="fieldInputBox" autocomplete="off" /><br />
                                    <asp:Label runat="server" ID="txtCodeProblem" Text="Code already in use." Visible="False" ForeColor="Red" /><br />
                                    <asp:Label runat="server" Text = "Hub Identifier" /><br />
                                    <asp:TextBox runat="server" ID="txtHubIdentifier" CssClass="fieldInputBox" MaxLength="1" /><br /><br />
                                    <asp:Label runat="server" Text="Print on Label?" /><br />
                                    <telerik:RadComboBox runat="server" ID="cboPrintOnLabel">
                                        <Items>
                                            <telerik:RadComboBoxItem id="Yes" runat="server" Text="Yes" />
                                            <telerik:RadComboBoxItem id="No" runat="server" Text="No" />
                                        </Items>
                                    </telerik:Radcombobox>
                                </td>
                                <td valign="top" >
                                    <uc:Point runat="server" ID="ucPoint"  />
                                </td>
                            </tr>
                        </table>
                        <div >
                            <asp:Button ID="btnSave" runat="server" CssClass="buttonClass" Width="75px" Text="Save" CommandName='<%# (Container is GridEditFormInsertItem) ? "PerformInsert" : "Update" %>' />
                            <asp:Button ID="btnCancel" runat="server" CssClass="buttonClass" Width="75px" Text="Cancel" CommandName="Cancel" CausesValidation="false" />
                        </div>
                    </FormTemplate>
                </EditFormSettings>
            </MasterTableView>
        </telerik:RadGrid>

    </fieldset>

    <script>
        $(function () {
            $('[id$=grdDepots]').on('keydown', '[id$=txtCode]', function () {
                $('[id$=txtCodeProblem]').hide();
            });
        });
    </script>
</asp:Content>
