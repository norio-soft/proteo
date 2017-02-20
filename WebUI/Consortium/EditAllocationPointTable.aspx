<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="EditAllocationPointTable.aspx.cs" Inherits="Orchestrator.WebUI.Consortium.EditAllocationPointTable" %>

<%@ Register TagPrefix="uc" TagName="Point" Src="~/UserControls/point.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManagerProxy ID="scriptManagerProxy" runat="server">
        <Scripts>
            <asp:ScriptReference Path="~/Consortium/EditAllocationPointTable.aspx.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    
    <script type="text/javascript">
        var pageIDs = {
            cboConsortiumMember: "<%= cboConsortiumMember.ClientID %>",
            cboPoint: "<%= cboPoint.ClientID %>",
            hidAllocations: "<%= hidAllocations.ClientID %>"
        };
    </script>
    
    <h1>Edit Allocation Point Table</h1>
    
    <fieldset>
        <legend>Table</legend>
        <table width="520px">
            <tr>
                <td class="formCellLabel">
                    Table Description
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtDescription" CssClass="fieldInputBox" runat="server" Width="250" MaxLength="50" />
                    <asp:RequiredFieldValidator ID="rfvDescription" runat="server" ErrorMessage="Please enter a description for this allocation point table" ControlToValidate="txtDescription" Display="Dynamic">
                        <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this allocation point table" alt="*" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </fieldset>

    <fieldset>
        <legend>Allocated Consortium Members</legend>
                
        <table id="tblAllocations">
            <thead>
                <tr>
                    <th align="left">Consortium member</th>
                    <th align="left">Point</th>
                    <th align="left">&nbsp;</th>
                </tr>
            </thead>
            
            <tbody>
                <asp:ListView ID="lvAllocations" runat="server">
                    <LayoutTemplate>
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id='tr<%# Eval("PointID") %>'>
                            <td><%# Eval("ConsortiumMemberName") %></td>
                            <td><%# Eval("PointDescription") %></td>
                            <td><a href="javascript:removePoint(<%# Eval("PointID") %>);">remove</a></td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>

                <tr id="trAllocationAdd">
                    <td>
                        <telerik:RadComboBox ID="cboConsortiumMember" runat="server" DataValueField="key" DataTextField="value" OnClientKeyPressing="cboConsortiumMember_KeyPressing" Width="250" DropDownWidth="350" ShowMoreResultsBox="false" ItemRequestTimeout="500" EnableLoadOnDemand="true">
                            <WebServiceSettings Path="~/ws/combostreamers.asmx" Method="GetSubContractors" />
                        </telerik:RadComboBox>
                        <asp:CustomValidator ID="cvConsortiumMember" runat="server" ValidationGroup="vgAllocation" ClientValidationFunction="cvConsortiumMember_Validate" ErrorMessage="Please select the consortium member." Display="Dynamic">
                            <img src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" title="Please select the consortium member." alt="*" />
                        </asp:CustomValidator>
                    </td>
                    <td>
                        <telerik:RadComboBox ID="cboPoint" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" OnClientDropDownClosed="cboPoint_DropDownClosed" OnClientKeyPressing="cboPoint_KeyPressing" OnClientBlur="cboPoint_Blur" ShowMoreResultsBox="false" Width="340px" HighlightTemplatedItems="true">
                            <WebServiceSettings Path="~/ws/combostreamers.asmx" Method="GetPoints" />
                        </telerik:RadComboBox>                                
                        <asp:CustomValidator ID="cvPoint" runat="server" ValidationGroup="vgAllocation" ClientValidationFunction="cvPoint_Validate" ErrorMessage="Please select the point." Display="Dynamic">
                            <img src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" title="Please select the point." alt="*" />
                        </asp:CustomValidator>
                    </td>
                    <td>
                        <asp:Button ID="btnAdd" runat="server" Text="Add" ValidationGroup="vgAllocation" OnClientClick="addAllocation(); return false;" />
                    </td>
                </tr>
            </tbody>
        </table>
    </fieldset>
       
    <div class="buttonbar">
        <asp:Button ID="btnSave" runat="server" Text="Save Changes" OnClientClick="writeAllocationsForServer();" />
        <asp:Button ID="btnList" runat="server" Text="Table List" CausesValidation="false" OnClientClick="location.href='allocationtablelist.aspx'; return false;" />
    </div>

    <asp:HiddenField ID="hidAllocations" runat="server" />    
</asp:Content>
