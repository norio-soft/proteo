<%@ Page Language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Job.addupdaterate"
    CodeBehind="addupdaterate.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Add Rate</h1>
    <h2><asp:Label ID="lblTitle" runat="server" Text="Set up rates for automated rate calculation below." ></asp:Label></h2>
    <asp:Label ID="lblConfirmation" runat="server" Text="" CssClass="confirmation" Visible="false"></asp:Label>
    <table border="0" cellpadding="1" cellspacing="0">
        <tr>
            <td>
                <fieldset>
                    <legend><strong>Client</strong></legend>
                    <table>
                        <tr>
                            <td class="formCellLabel">Client</td>
                            <td class="formCellInput">
                                <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" ShowMoreResultsBox="false"
                                    AllowCustomText="false" Skin="WindowsXP" Width="355px">
                                </telerik:RadComboBox>
                            </td>
                            <td class="formCellInput">
                                <asp:RequiredFieldValidator ID="rfvClient" runat="server" ControlToValidate="cboClient"
                                    ErrorMessage="Please supply a company that owns the collection point."><img src="../images/Error.gif" height="16" width="16" title="Please supply the company that owns the collection point to which this rate applies." /></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cfvClient" runat="server" ControlToValidate="cboClient"
                                    EnableClientScript="False" ErrorMessage="Please supply a company that owns the collection point."><img src="../images/error.gif" title="Please supply the company that owns the collection point to which this rate applies." /></asp:CustomValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td>
                <fieldset>
                    <legend><strong>Collection Point</strong></legend>
                    <table>
                        <tr>
                            <td class="formCellLabel">Collection Point Owner</td>
                            <td class="formCellInput">
                                <telerik:RadComboBox ID="cboCollection" runat="server" EnableLoadOnDemand="true"
                                    ItemRequestTimeout="500" MarkFirstMatch="true" AllowCustomText="false" OnClientSelectedIndexChanged="CollectionOwnerClientSelectedIndexChanged"
                                    RadControlsDir="~/script/RadControls/" ShowMoreResultsBox="false" Width="355px">
                                </telerik:RadComboBox>
                            </td>
                            <td class="formCellInput">
                                <asp:RequiredFieldValidator ID="rfvCollection" runat="server" ControlToValidate="cboCollection"
                                    ErrorMessage="Please supply a company that owns the collection point."><img src="../images/Error.gif" height="16" width="16" title="Please supply the company that owns the collection point to which this rate applies." /></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cfvCollection" runat="server" ControlToValidate="cboCollection"
                                    EnableClientScript="False" ErrorMessage="Please supply a company that owns the collection point."><img src="../images/error.gif" title="Please supply the company that owns the collection point to which this rate applies." /></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Collection Point</td>
                            <td class="formCellInput">
                                <telerik:RadComboBox ID="cboCollectionPoint" runat="server" EnableLoadOnDemand="true"
                                    ItemRequestTimeout="500" MarkFirstMatch="true" AllowCustomText="false" RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="false" Width="355px" OnClientItemsRequesting="CollectionPointRequesting">
                                </telerik:RadComboBox>
                            </td>
                            <td class="formCellInput">
                                <asp:RequiredFieldValidator ID="rfvCollectionPoint" runat="server" ControlToValidate="cboCollectionPoint"
                                    ErrorMessage="Please supply a collection point."><img src="../images/Error.gif" height="16" width="16" title="Please supply the collection point to which this rate applies." /></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cfvCollectionPoint" runat="server" ControlToValidate="cboCollectionPoint"
                                    EnableClientScript="False" ErrorMessage="Please supply a collection point."><img src="../images/error.gif" alt="Please supply the collection point to which this rate applies." /></asp:CustomValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
            <td>
                <fieldset>
                    <legend><strong>Delivery Point</strong></legend>
                    <table>
                        <tr>
                            <td class="formCellLabel">Client</td>
                            <td class="formCellInput">
                                <telerik:RadComboBox ID="cboDelivery" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" AllowCustomText="false" OnClientSelectedIndexChanged="DeliveryOwnerClientSelectedIndexChanged"
                                    RadControlsDir="~/script/RadControls/" ShowMoreResultsBox="false" Width="355px">
                                </telerik:RadComboBox>
                            </td>
                            <td class="formCellInput">
                                <asp:RequiredFieldValidator ID="rfvDelivery" runat="server" ControlToValidate="cboDelivery"
                                    ErrorMessage="Please select a delivery customer."><img src="../images/Error.gif" alt="Please supply a delivery customer." /></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cfvDelivery" runat="server" ControlToValidate="cboDelivery"
                                    EnableClientScript="False" ErrorMessage="Please select a delivery customer."><img src="../images/error.gif" alt="Please supply a delivery customer." /></asp:CustomValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Client Point</td>
                            <td class="formCellInput">
                                <telerik:RadComboBox ID="cboDeliveryPoint" runat="server" EnableLoadOnDemand="true"
                                    ItemRequestTimeout="500" MarkFirstMatch="true" AllowCustomText="false" RadControlsDir="~/script/RadControls/"
                                    ShowMoreResultsBox="false" Width="355px" OnClientItemsRequesting="DeliveryPointRequesting">
                                </telerik:RadComboBox>
                            </td>
                            <td class="formCellInput">
                                <asp:RequiredFieldValidator ID="rfvDeliveryPoint" runat="server" ControlToValidate="cboDeliveryPoint"
                                    ErrorMessage="Please supply a delivery point."><img src="../images/Error.gif" height="16" width="16" title="Please supply the delivery point to which this rate applies." /></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cfvDeliveryPoint" runat="server" ControlToValidate="cboDeliveryPoint"
                                    EnableClientScript="False" ErrorMessage="Please supply a delivery point."><img src="../images/error.gif" alt="Please supply a delivery point." /></asp:CustomValidator>
                            </td>
                        </tr>
                    </table>
                </fieldset>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <fieldset>
                    <legend><strong>Rate information</strong></legend>
                    <table>
                        <tr>
                            <td class="formCellLabel">Full Load Rate</td>
                            <td class="formCellInput">
                                <asp:TextBox ID="txtFullLoadRate" runat="server" />
                                <asp:RequiredFieldValidator ID="rfvFullLoadRate" runat="server" ControlToValidate="txtFullLoadRate"
                                    ErrorMessage="Please supply a full load rate."><img src="../images/Error.gif" height="16" width="16" title="Please supply a job rate." /></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revFullLoadRate" runat="server" Display="Dynamic"
                                    ControlToValidate="txtFullLoadRate" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$"
                                    ErrorMessage="Please enter a valid currency value for the full load amount."><img src="../images/Error.gif" height="16" width="16" title="Please enter a valid currency value for the full load rate rate." /></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Multi Drop Rate</td>
                            <td class="formCellInput">
                                <asp:TextBox ID="txtMultiDropRate" runat="server" />
                                <asp:RequiredFieldValidator ID="rfvMultiDropRate" runat="server" ControlToValidate="txtMultiDropRate"
                                    ErrorMessage="Please supply a multi drop rate."><img src="../images/Error.gif" height="16" width="16" title="Please supply a multi drop rate." /></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revMultiDropRate" runat="server" Display="Dynamic"
                                    ControlToValidate="txtMultiDropRate" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$"
                                    ErrorMessage="Please enter a valid currency value for the job charge amount."><img src="../images/Error.gif" height="16" width="16" title="Please enter a valid currency value for the multi drop rate." /></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Part Load Rate</td>
                            <td class="formCellInput">
                                <asp:TextBox ID="txtPartLoadRate" runat="server" />
                                <asp:RequiredFieldValidator ID="rfvPartLoadRate" runat="server" ControlToValidate="txtPartLoadRate"
                                    ErrorMessage="Please supply a part load rate."><img src="../images/Error.gif" height="16" width="16" title="Please supply a part load rate." /></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revPartLoadRate" runat="server" Display="Dynamic"
                                    ControlToValidate="txtPartLoadRate" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$"
                                    ErrorMessage="Please enter a valid currency value for the part load amount."><img src="../images/Error.gif" height="16" width="16" title="Please enter a valid currency value for the part load rate." /></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Start Date</td>
                            <td class="formCellInput">
                                <table>
                                    <tr>
                                        <td>
                                            <telerik:RadDateInput id="dteStartDate" runat="server" ToolTip="The date this rate comes into effect."
                                                dateformat="dd/MM/yy" Width="60px">
                                            </telerik:RadDateInput>
                                        </td>
                                        <td>
                                            <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" Display="Dynamic" ControlToValidate="dteStartDate"
                                                ErrorMessage="Please supply a start date."><img src="../images/Error.gif" height="16" width="16" title="Please supply a start date." /></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">End Date</td>
                            <td class="formCellInput">
                                <table>
                                    <tr>
                                        <td>
                                            <telerik:RadDateInput id="dteEndDate" runat="server" ToolTip="The expiry date of the rate. If omitted, rate will never expire."
                                                dateformat="dd/MM/yy" Width="60px"  EmptyMessage="no end date">
                                            </telerik:RadDateInput>
                                        </td>
                                        <td>
                                            &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </fieldset> 
            </td> 
        </tr> 
    </table>
    <div class="buttonbar">
        <asp:Button ID="btnSubmit" runat="server" Text="Add Rate"></asp:Button>
    </div>

    <script language="javascript">
        function CollectionOwnerClientSelectedIndexChanged(item)
        {
            var pointCombo = $find("<%=cboCollectionPoint.ClientID %>");
            pointCombo.set_text("");
            pointCombo.requestItems(item.get_value(),false);
        }

        function CollectionPointRequesting(sender, eventArgs)
        {
            var collectionOwnerClientCombo = $find("<%=cboCollection.ClientID %>");

            var context = eventArgs.get_context();
            context["FilterString"] = collectionOwnerClientCombo.get_value() + ";" + sender.get_text();
        } 
       
        function DeliveryOwnerClientSelectedIndexChanged(item)
        {
            var pointCombo = $find("<%=cboDeliveryPoint.ClientID %>");
            pointCombo.set_text("");
            pointCombo.requestItems(item.get_value(),false);
        }

        function DeliveryPointRequesting(sender, eventArgs)
        {
            var deliveryOwnerClientCombo = $find("<%=cboDelivery.ClientID %>");

            var context = eventArgs.get_context();
            context["FilterString"] = deliveryOwnerClientCombo.get_value() + ";" + sender.get_text();
        } 
    </script>

    <script language="javascript" type="text/javascript">
<!--
        var storedCollectionIdentityId;
        var storedCollectionOrganisationName;

        var storedCollectionTownId;
        var storedCollectionTown;

        var storedCollectionPointId;
        var storedCollectionPoint;

        var storedDeliveryIdentityId;
        var storedDeliveryOrganisationName;

        var storedDeliveryTownId;
        var storedDeliveryTown;

        var storedDeliveryPointId;
        var storedDeliveryPoint;

        function StateCollection() {
            var state = new Object();

            state["CollectionIdentityId"] = storedCollectionIdentityId;
            return state;
        }

        function StateCollectionTown() {
            var state = new Object();

            state["CollectionTownId"] = storedCollectionTownId;
            return state;
        }

        function StateCollectionPoint() {
            var state = new Object();

            state["CollectionIdentityId"] = storedCollectionIdentityId;
            state["CollectionTownId"] = storedCollectionTownId;
            state["CollectionPointId"] = storedCollectionPointId;
            return state;
        }

        function StateDelivery() {
            var state = new Object();

            state["DeliveryIdentityId"] = storedDeliveryIdentityId;
            return state;
        }
        function StateDeliveryTown() {
            var state = new Object();

            state["DeliveryTownId"] = storedDeliveryTownId;
            return state;
        }

        function StateDeliveryPoint() {
            var state = new Object();

            state["DeliveryIdentityId"] = storedDeliveryIdentityId;
            state["DeliveryTownId"] = storedDeliveryTownId;
            state["DeliveryPointId"] = storedDeliveryPointId;
            return state;
        }

        function StoreCollection(Value, Text, SelectionType) {
            if (SelectionType == 2 || SelectionType == 9 || SelectionType == 7 || SelectionType == 3) {
                storedCollectionIdentityId = Value;
                storedCollectionOrganisationName = Text;
            }
        }

        function StoreCollectionTown(Value, Text, SelectionType) {
            if (SelectionType == 2 || SelectionType == 9 || SelectionType == 7 || SelectionType == 3) {
                storedCollectionTownId = Value;
                storedCollectionTown = Text;
            }
        }

        function StoreCollectionPoint(Value, Text, SelectionType) {
            if (SelectionType == 2 || SelectionType == 9 || SelectionType == 7 || SelectionType == 3) {
                storedCollectionPointId = Value;
                storedCollectionPoint = Value;
            }
        }

        function StoreDelivery(Value, Text, SelectionType) {
            if (SelectionType == 2 || SelectionType == 9 || SelectionType == 7 || SelectionType == 3) {
                storedDeliveryIdentityId = Value;
                storedDeliveryOrganisationName = Text;
            }
        }

        function StoreDeliveryTown(Value, Text, SelectionType) {
            if (SelectionType == 2 || SelectionType == 9 || SelectionType == 7 || SelectionType == 3) {
                storedDeliveryTownId = Value;
                storedDeliveryTown = Text;
            }
        }

        function StoreDeliveryPoint(Value, Text, SelectionType) {
            if (SelectionType == 2 || SelectionType == 9 || SelectionType == 7 || SelectionType == 3) {
                storedDeliveryPointId = Value;
                storedDeliveryPoint = Value;
            }
        }
-->
    </script>

</asp:Content>
