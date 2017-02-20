<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Organisation.addupdateclientscustomer" MasterPageFile="~/default_tableless.master" Title="Client Customer" CodeBehind="addupdateclientscustomer.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Client Customer</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript">
		function AddClientRelationShip(relatedIdentityId, organisationName) {
            var qs = "ID=" + relatedIdentityId + "&ON=" + organisationName + "&CC=true";
            <%=this.dlgAddClientRelationship.ClientID%>_Open(qs);
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgAddClientRelationship" runat="server" URL="relateclient.aspx" Width="600" Height="350" ></cc1:Dialog>
    
    <asp:Label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation" ></asp:Label>
    <asp:Label ID="lblWhereAmI" Style="padding-bottom: 5px" runat="server" Visible="false"></asp:Label>
    <uc1:infringementDisplay ID="infringementDisplay" runat="server" Visible="false">
    </uc1:infringementDisplay>
    
    <fieldset>
        <legend>Details</legend>
        <table id="Table1">
            <tr>
                <td class="formCellLabel">Name</td>
                <td class="formCellField">
                    <asp:TextBox ID="txtOrganisationName" runat="server" Width="250"></asp:TextBox>
                </td>
                <td class="formCellField">
                    <asp:RequiredFieldValidator ID="rfvOrganisationName" runat="server" Display="Dynamic"
                        ControlToValidate="txtOrganisationName" ErrorMessage="Please enter a clients customer name.">
							<img src="../images/Error.gif" height='16' width='16' title='Please enter a clients customer name.'></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <fieldset>
        <legend>List of Locations</legend>
        <asp:DataGrid ID="dgLocations" runat="server" Width="100%" AllowPaging="True" PageSize="20"
            PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right" OnPageIndexChanged="dgLocations_Page"
            OnSortCommand="dgLocations_SortCommand" AutoGenerateColumns="False" AllowSorting="True">
            <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
            <ItemStyle CssClass="DataGridListItem"></ItemStyle>
            <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
            <Columns>
                <asp:TemplateColumn SortExpression="OrganisationLocationName" HeaderText="Name">
                    <ItemTemplate>
                        <a href='addupdateorganisationlocation.aspx?organisationLocationId=<%# DataBinder.Eval(Container.DataItem, "OrganisationLocationId")%>&amp;identityId=<%# DataBinder.Eval(Container.DataItem,"IdentityId")%>&amp;organisationName=<%=m_organisationName%>'>
                            <%#DataBinder.Eval(Container.DataItem, "OrganisationLocationName")%>
                        </a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn DataField="PostTown" SortExpression="PostTown" HeaderText="Post Town">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="County" SortExpression="County" HeaderText="County">
                </asp:BoundColumn>
                <asp:BoundColumn DataField="PostCode" SortExpression="PostCode" HeaderText="Post Code">
                </asp:BoundColumn>
            </Columns>
            <PagerStyle HorizontalAlign="Right" CssClass="DataGridListPagerStyle" Mode="NumericPages">
            </PagerStyle>
        </asp:DataGrid>
        <br>
        <div class="buttonbar">
            <asp:Button ID="btnViewPoints" runat="server" OnClick="btnViewPoints_Click" Text="View Points"
                CausesValidation="False"></asp:Button>
            &nbsp;
            <asp:Button ID="btnAddLocation" OnClick="btnAddLocation_Click" runat="server" Text="Add Location"
                Enabled="False" CausesValidation="False"></asp:Button>
        </div>
    </fieldset>
    
    <fieldset>
        <legend>Status</legend>
        <table id="Table2">
            <tr runat="server" id="approveNote" visible ="false">
                <td>
                    <b>Note: Approving an organisation will approve all associated Points.</b>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:DropDownList ID="ddStatuses" runat="server">
                        <%--<asp:ListItem Text="Active" Value="1"></asp:ListItem>
                        <asp:ListItem Text="Deleted" Value="2"></asp:ListItem>
                        <asp:ListItem Text="Suspended" Value="3"></asp:ListItem>
                        <asp:ListItem Text="Unavailable" Value="4"></asp:ListItem>
                        <asp:ListItem Text="Unapproved" Value="5"></asp:ListItem>--%>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
    </fieldset>

    <asp:Panel ID="pnlDeliveryNotification" runat="server" Visible="false">
        <fieldset>
            <legend>Notification</legend>
            <table>
                <tr>
                    <td></td>
                    <td>
                        <asp:CheckBox ID="chkBoxEnableNotification" runat="server" Text="Enable Notifications"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel" style="width: 120px;">Notification Type
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cbNotificationType" runat="server" Width="200px">
                            <Items>
                                <telerik:RadComboBoxItem Value="1" Text="Email" />
                                <telerik:RadComboBoxItem Value="2" Text="SMS" />
                            </Items>
                        </telerik:RadComboBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" EnableClientScript="False"
                            ErrorMessage="Please select notification type." ControlToValidate="cbNotificationType"
                            Display="Dynamic">
						    <img src="/images/Error.gif" height="16" width="16" title="Please select notification type." alt="" />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                        <tr>
                    <td class="formCellLabel" style="width: 120px;">Notify When
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cbNotifyWhen" runat="server" Width="350px" EnableLoadOnDemand="true" OnItemsRequested="cbNotifyWhen_ItemsRequested">
                        </telerik:RadComboBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" EnableClientScript="False"
                            ErrorMessage="Please select when to notify." ControlToValidate="cbNotifyWhen"
                            Display="Dynamic">
						    <img src="/images/Error.gif" height="16" width="16" title="Please select when to notify." alt="" />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Email address or Mobile Number
                    </td>
                    <td class="formCellField">
                        <asp:TextBox runat="server" ID="txtContactDetail" Width="250"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                            Display="Dynamic" runat="server" ControlToValidate="txtContactDetail"
                            ErrorMessage="You must enter the contact detail"></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:Panel>
    
    <div class="buttonbar">
        <input type="button" value="Add Customer Relationship" id="btnAddCustomer" onclick="AddClientRelationShip(<%=m_identityId%>, '<%=m_organisationName%>');" />
        <asp:Button ID="btnPromote" runat="server" Text="Promote to Client"></asp:Button>
        <asp:Button ID="btnAdd" runat="server" Text="Add" OnClick="btnAdd_Click" Width="75px">
        </asp:Button>
        <asp:Button ID="btnListClientCustomers" runat="server" Text="List Client Customers"
            CausesValidation="False" OnClick="btnListClientCustomers_Click"></asp:Button>
    </div>
</asp:Content>