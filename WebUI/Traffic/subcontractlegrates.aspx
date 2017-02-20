<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic_subcontractlegrates" MasterPageFile="~/WizardMasterPage.master" CodeBehind="subcontractlegrates.aspx.cs" %>

<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script language="javascript" type="text/javascript" src="/script/scripts.js"></script>
    <script language="javascript" type="text/javascript" src="/script/tooltippopups.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <fieldset runat="server" id="fldWholeJobSubcontractInformation" visible="false">
        <legend>Whole Job Subcontract Information</legend>
        <table cellpadding="2px">
            <thead>
                <tr>
                    <th>
                        Rate
                    </th>
                    <th>
                        Attended
                    </th>
                    <th>
                        Reference
                    </th>
                </tr>
            </thead>
            <tr>
                <td>
                    <telerik:RadNumericTextBox ID="txtWholeJobSubcontractRate" Type="Currency" runat="server"
                        Width="80">
                    </telerik:RadNumericTextBox>
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkIsAttended" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtReference"></asp:TextBox>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <asp:Repeater ID="repLegSubbyRates" runat="server">
        <HeaderTemplate>
            <table border="0" cellpadding="2" cellspacing="0" class="Grid">
                <thead>
                    <tr class="HeadingRow">
                        <th>
                            Extras
                        </th>
                        <th width="75">
                            Start
                        </th>
                        <th>
                            From
                        </th>
                        <th width="75">
                            Finish
                        </th>
                        <th>
                            To
                        </th>
                        <th>
                            Pallets
                        </th>
                        <th>
                            Weight
                        </th>
                        <th>
                            Driver
                        </th>
                        <th>
                            Trailer
                        </th>
                        <th>
                            Subby&nbsp;Rate
                        </th>
                        <th>
                            Attended
                        </th>
                        <th>
                            Subcontract Ref.
                        </th>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr valign="top" class="Row">
                <td>
                    <span id="spnExtra" onclick="" onmouseover="ShowExtrasDetailsToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["Extra"].ToString().Length > 0 ? ((System.Data.DataRowView)Container.DataItem)["Extra"].ToString() : "-1" %>);"
                        onmouseout="closeToolTip();" class="orchestratorLink">
                        <%#((System.Data.DataRowView)Container.DataItem)["Extra"].ToString().Length > 0 ? "Extra" : "&nbsp;"%></span>
                </td>
                <td>
                    <%# Eval("LegPlannedStartDateTime", "{0:dd/MM hh:mm}")%>
                </td>
                <td>
                    <span id="spnCollectionPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["StartPointId"].ToString() %>);"
                        onmouseout="closeToolTip();" class="orchestratorLink"><b>
                            <%#((System.Data.DataRowView)Container.DataItem)["StartPointDisplay"].ToString()%></b></span>
                </td>
                <td>
                    <%# Eval("LegPlannedEndDateTime", "{0:dd/MM hh:mm}") %>
                </td>
                <td>
                    <span id="spnEndPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["EndPointId"].ToString() %>);"
                        onmouseout="closeToolTip();" class="orchestratorLink"><b>
                            <%#((System.Data.DataRowView)Container.DataItem)["EndPointDisplay"].ToString()%></b></span>
                </td>
                <td align="right">
                    <%# Eval("EndInstructionPallets") %>
                </td>
                <td align="right">
                    <%# Eval("EndInstructionWeight", "{0:F}") %>
                </td>
                <td>
                    <span id="spnDriver" onclick="" onmouseover="ShowContactInformationToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DriverIdentityId"].ToString().Length > 0 ? ((System.Data.DataRowView)Container.DataItem)["DriverIdentityId"].ToString() : "-1" %>);"
                        onmouseout="closeToolTip();" class="orchestratorLink">
                        <%#((System.Data.DataRowView)Container.DataItem)["DrivingDisplayName"].ToString().Length > 0 ? ((System.Data.DataRowView)Container.DataItem)["DrivingDisplayName"].ToString() : "N/A"%></span>
                </td>
                <td>
                    <%# Eval("TrailerRef") %>&nbsp;
                </td>
                <td align="right">
                    <telerik:RadNumericTextBox ID="rntSubContractRate" Type="Currency" runat="server"
                        Width="60">
                    </telerik:RadNumericTextBox>
                    <asp:HiddenField ID="hidJobSubContractID" runat="server" Value='<%# Eval("JobSubContractID") %>' />
                </td>
                <td>
                    <asp:CheckBox runat="server" ID="chkIsAttended" />
                </td>
                <td>
                    <asp:TextBox runat="server" ID="txtSubcontractReference"></asp:TextBox>
                </td>
            </tr>
            <tr id="rowSubbedOrders" runat="server" valign="top" class="Row">
                <td class="DataCell">
                    &nbsp;
                </td>
                <td colspan="9" class="DataCell">
                    <asp:Repeater ID="repOrderSubbyRates" runat="server" OnItemDataBound="repOrderSubbyRates_ItemDataBound"
                        OnPreRender="repOrderSubbyRates_PreRender">
                        <HeaderTemplate>
                            <table border="0" cellpadding="2" cellspacing="1" class="Grid">
                                <thead>
                                    <tr class="HeadingRowLite">
                                        <th>
                                            Client
                                        </th>
                                        <th>
                                            Collection
                                        </th>
                                        <th>
                                            Delivery
                                        </th>
                                        <th width="100">
                                            References
                                        </th>
                                        <th>
                                            Pallets
                                        </th>
                                        <th>
                                            Weight
                                        </th>
                                        <th>
                                            Cases
                                        </th>
                                        <th>
                                            Subby&nbsp;Rate
                                        </th>
                                        <th>
                                            Attended
                                        </th>
                                        <th>
                                            Subcontract Ref.
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr valign="top" class="Row">
                                <td>
                                    <%# Eval("OrganisationName") %>
                                </td>
                                <td>
                                    <span id="spnCollectionPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>);"
                                        onmouseout="hideAd();" class="orchestratorLink"><b>
                                            <%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
                                    <br />
                                    <%# ((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"]).ToString("dd/MM/yy") %>
                                    &nbsp;
                                    <%# ((bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"]) ? "AnyTime" : ((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"]).ToString("HH:mm") %>
                                </td>
                                <td>
                                    <span id="spnDeliveryPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);"
                                        onmouseout="hideAd();" class="orchestratorLink"><b>
                                            <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>
                                    <br />
                                    <%# ((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("dd/MM/yy") %>
                                    &nbsp;
                                    <%# ((bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"]) ? "AnyTime" : ((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("HH:mm") %>
                                </td>
                                <td width="150">
                                    <asp:Repeater ID="repReferences" runat="server">
                                        <ItemTemplate>
                                            <span title='<%# (Container.DataItem as System.Data.DataRow)["Description"].ToString().Replace("'", "''")%>'>
                                                <%# (Container.DataItem as System.Data.DataRow)["Reference"].ToString().Trim().Length > 0 ? (Container.DataItem as System.Data.DataRow)["Reference"].ToString() : "<b>Not Supplied</b>" %></span>
                                        </ItemTemplate>
                                        <SeparatorTemplate>
                                            <br />
                                        </SeparatorTemplate>
                                    </asp:Repeater>
                                </td>
                                <td>
                                    <%# DataBinder.Eval(Container.DataItem, "PalletCount") %>&nbsp;<%# DataBinder.Eval(Container.DataItem, "PalletTypeDescription") %>
                                </td>
                                <td>
                                    <%# ((decimal)((System.Data.DataRowView)Container.DataItem)["Weight"]).ToString("F2") %>
                                    <%# DataBinder.Eval(Container.DataItem, "WeightTypeDescription") %>
                                </td>
                                <td>
                                    <%# Eval("Cases") %>
                                </td>
                                <td align="right">
                                    <telerik:RadNumericTextBox ID="rntSubContractRate" Type="Currency" runat="server"
                                        Width="60">
                                    </telerik:RadNumericTextBox>
                                    <asp:HiddenField ID="hidJobSubContractID" runat="server" Value='<%# Eval("JobSubContractID") %>' />
                                </td>
                                <td>
                                    <asp:CheckBox runat="server" ID="chkIsAttended" />
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSubcontractReference"></asp:TextBox>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody> </table>
                        </FooterTemplate>
                        <SeparatorTemplate>
                            <tr>
                                <td colspan="12">
                                    <hr noshade />
                                </td>
                            </tr>
                        </SeparatorTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </ItemTemplate>
        <SeparatorTemplate>
            <tr>
                <td colspan="12">
                    <hr noshade />
                </td>
            </tr>
        </SeparatorTemplate>
        <FooterTemplate>
                </tbody> 
            </table>
        </FooterTemplate>
    </asp:Repeater>
    
    <div class="buttonbar">
        <asp:Button ID="btnUpdateRates" runat="server" Text="Update Information" />
        <input type="Button" onclick="CloseOnReload();" value="Close" style="width: 75px;" />
    </div>

    <script type="text/javascript" language="javascript">
    <!--
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well) 
            return oWindow;
        }

        function CloseOnReload()
        {
            GetRadWindow().Close();
        }

        function RefreshParentPage()
        {
           GetRadWindow().BrowserWindow.location.reload();
        }
    //-->
    </script>

    <asp:Label ID="lblInjectScript" runat="server"></asp:Label>
</asp:Content>