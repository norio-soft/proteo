<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Organisation.ChangeReference" MasterPageFile="~/WizardMasterPage.master" Title="Haulier Enterprise" Codebehind="ChangeReference.aspx.cs" %>

<%@ Import namespace="System.Data"%>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change References</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

<script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>
<script type="text/javascript" language="javascript" src="/script/scripts.js"></script>

    <asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnUpdate">
        <h1>Update References</h1>

        <table>
            <tr>
                <td runat="server" id="tdDateOptions" >
                    <table>
                        <tr>
                            <td>Reference</td>
                            <td><telerik:RadComboBox ID="cboReference" runat="server" DataTextField="Description" DataValueField="OrganisationReferenceID" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False" ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" Width="200px" Height="15px"></telerik:RadComboBox></td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvCboReference" runat="server" ControlToValidate="cboReference" ErrorMessage="*"></asp:RequiredFieldValidator>
                                &nbsp;
                                (NB : References ending with * are deleted references)
                            </td>
                        </tr>
                        <tr>
                            <td>Value</td>
                            <td colspan="2">
                                <asp:TextBox ID="txtValue" runat="server" Width="196px" ></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvTxtValue" runat="server" ControlToValidate="txtValue" Display="Dynamic" ErrorMessage="*"><img src="../images/ico_critical_small.gif" alt="Please supply a new value for this reference." /></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cfvTxtValueInteger" runat="server" ControlToValidate="txtValue" EnableClientScript="false" Display="Dynamic" ErrorMessage="*">*</asp:CustomValidator>
                                <asp:CustomValidator ID="cfvTxtValueDecimal" runat="server" ControlToValidate="txtValue" EnableClientScript="false" Display="Dynamic" ErrorMessage="*">*</asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">&nbsp;</td>
                        </tr>
                        <tr>
                            <td colspan="3">
                                Note : If updating an <b>active</b> reference <b>ALL</b> active references will be updated. If they do not exist, they will be created.
                                <br />
                                If updating a <b>deleted</b> reference <b>ONLY</b> existing references will be updated, no new reference will be created.
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        
        <div class="buttonbar">
            <asp:Button ID="btnUpdate" runat="server" Text="Update" CausesValidation="true" />
            <asp:Button ID="btnClose" runat="server" Text="Close" CausesValidation="false" />
        </div>
    </asp:Panel>
    
    <asp:Panel ID="pnlGrids" runat="server" Height="350px">
        <div style="overflow-y:auto; overflow-x:hidden; height:100%;">
            <table style="width:100%;" align="center">
                <tr>
                    <td>
                        <telerik:RadGrid ID="grdJobs" runat="server" AllowPaging="false" ShowGroupPanel="true" allowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
                            <MasterTableView DataKeyNames="JobID" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn HeaderText="Job ID" DataField="JobID" HeaderStyle-Width="75"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Load Number" DataField="CustomerOrderNumber" HeaderStyle-Width="100"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Docket No" DataField="DeliveryOrderNumber" HeaderStyle-Width="100"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                                        <ItemTemplate>
                                             <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%#((DataRowView)Container.DataItem)["ClientCustomerOrganisationName"].ToString()%></b></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Delivery" HeaderStyle-Width="115">
                                        <ItemTemplate>
                                            <span id="spnDelivery" onclick=""><%#((bool)((DataRowView)Container.DataItem)["DeliveryIsAnytime"]) ? ((DateTime)((DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("dd/MM/yyyy") : ((DateTime)((DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("dd/MM/yyyy HH:mm")%></span>                                    
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Reference" HeaderStyle-Width="100">
                                        <ItemTemplate>
                                            <span id="spnJobReference" onclick=""><%#((DataRowView)Container.DataItem)["Reference"] == null ? "&nbsp;" : ((DataRowView)Container.DataItem)["Reference"].ToString()%></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>                                    
                                    <telerik:GridTemplateColumn HeaderText="Description" HeaderStyle-Width="100">
                                        <ItemTemplate>
                                            <span id="spnJobDescription" onclick=""><%#((DataRowView)Container.DataItem)["Description"] == null ? "&nbsp;" : ((DataRowView)Container.DataItem)["Description"].ToString()%></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>   
                </tr>
                <tr><td>&nbsp;</td></tr>
                <tr>
                    <td>
                        <telerik:RadGrid ID="grdOrders" runat="server" AllowPaging="false" ShowGroupPanel="true" allowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
                            <MasterTableView DataKeyNames="OrderID" Width="100%">
                                <Columns>
                                    <telerik:GridBoundColumn HeaderText="Order ID" DataField="OrderID" HeaderStyle-Width="75"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Customer Order Number" DataField="CustomerOrderNumber" HeaderStyle-Width="100"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Delivery Order Number" DataField="DeliveryOrderNumber" HeaderStyle-Width="100"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                                        <ItemTemplate>
                                             <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%#((DataRowView)Container.DataItem)["ClientCustomerOrganisationName"].ToString()%></b></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Delivery" HeaderStyle-Width="115">
                                        <ItemTemplate>
                                            <span id="spnDelivery" onclick=""><%#((bool)((DataRowView)Container.DataItem)["DeliveryIsAnytime"]) ? ((DateTime)((DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("dd/MM/yyyy") : ((DateTime)((DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("dd/MM/yyyy HH:mm")%></span>                                    
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Reference" HeaderStyle-Width="100">
                                        <ItemTemplate>
                                            <span id="spnOrderReference" onclick=""><%#((DataRowView)Container.DataItem)["Reference"] == null ? "&nbsp;" : ((DataRowView)Container.DataItem)["Reference"].ToString()%></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>                                    
                                    <telerik:GridTemplateColumn HeaderText="Description" HeaderStyle-Width="100">
                                        <ItemTemplate>
                                            <span id="spnOrderDescription" onclick=""><%#((DataRowView)Container.DataItem)["Description"] == null ? "&nbsp;" : ((DataRowView)Container.DataItem)["Description"].ToString()%></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

</asp:Content>

