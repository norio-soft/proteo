<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="MatchOrder.aspx.cs" Inherits="Orchestrator.WebUI.Invoicing.MatchOrder" Title="Match Order" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="dlg" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <base target="_self" />  
    <script language="javascript" src="/script/tooltippopups.js" type="text/javascript"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Match Order</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dlg:Dialog ID="dlgOrder" runat="server" URL="/Groupage/ManageOrder.aspx" Width="1080" Height="900" AutoPostBack="false" ReturnValueExpected="false" Mode="Modal" />
    
    <div>
        Note : When searching for depot charges, the order will not be selectable until the order has been subcontracted. If the order does not contain a Hub Charge, one will be created when the match is confirmed.
    </div>

    <div>
        <div style="float:left;">
            <table>
                <tr>
                    <td class="formCellLabel">Search:</td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtSearch" runat="server" Width="125" ></asp:TextBox>
                        <asp:CheckBox id="chkSearchByOrderID" runat="server" Text="Limit the Search to use the Order ID only." Checked="false" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Start Date:</td>
                    <td class="formCellField"><telerik:RadDateInput ID="dteStartDate" runat="server" DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy"></telerik:RadDateInput></td>
                </tr>
                <tr>
                    <td class="formCellLabel">End Date:</td>
                    <td class="formCellField"><telerik:RadDateInput ID="dteEndDate" runat="server" DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy"></telerik:RadDateInput></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Business Types</td>
                    <td class="formCellField">
                        <asp:CheckBoxList ID="cblBusinessType" runat="server" RepeatDirection="Horizontal" RepeatColumns="4" DataTextField="Description" DataValueField="BusinessTypeID" >
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Search By Client:</td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                            ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Resource</td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cboResource" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                            ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                        </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Sub-Contractor</td>
                    <td class="formCellField">
                        <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                            ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                        </telerik:RadComboBox>
                    </td>
                </tr>
            </table>
        </div>
        <div style="float:left;">
            <h3>Item to Match</h3>
            <table>
                <tr>
                    <td class="formCellLabel"><asp:Label id="lblRefCaption" runat="server" /></td>
                    <td class="formCellField"><asp:Label id="lblRef" runat="server" /></td>
                </tr>
            </table>
        </div>
    </div>
    
    <div class="clearDiv"></div>
    
    <div class="buttonbar">
        <asp:Button ID="btnClose" runat="server" Text="Close" />
        <asp:Button ID="btnFindOrder" runat="server" Text="Find Order" />
        <asp:Button ID="btnMatchOrder" runat="server" Text="Confirm Match" style="display:none;" />
    </div>
    
    <div style="height: 320px; overflow: scroll;">
        <telerik:RadGrid runat="server" ID="grdOrders" Width="97%" AllowPaging="false" ShowGroupPanel="true" allowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false">
            <MasterTableView Width="100%" ClientDataKeyNames="OrderID" DataKeyNames="OrderID">
                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                <Columns>
                    <telerik:GridTemplateColumn>
                        <ItemTemplate>
                            <input runat="server" id="chkSelect" type="checkbox" class="chkGroupOrderSelect" onclick="javascript:selectedOrder(this);" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="ID">
                        <ItemTemplate><%# "<a href=\"javascript:viewOrderProfile('" + ((System.Data.DataRow)Container.DataItem)["OrderID"] + "');\" > " + ((System.Data.DataRow)Container.DataItem)["OrderID"] + "</a>"%></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Client">
                        <ItemTemplate><%# ((System.Data.DataRow)Container.DataItem)["CustomerOrganisationName"]%></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Business Type">
                        <ItemTemplate><%# ((System.Data.DataRow)Container.DataItem)["BusinessTypeDescription"]%></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Service">
                        <ItemTemplate><%# ((System.Data.DataRow)Container.DataItem)["OrderServiceLevel"]%></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Order No">
                        <ItemTemplate><%# ((System.Data.DataRow)Container.DataItem)["CustomerOrderNumber"]%></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Collect Details" SortExpression="CollectionPointDescription" ItemStyle-Width="200">
                        <ItemTemplate>
                            <%#"<span onmouseover=\"javascript:ShowPointToolTip(this, '" + ((System.Data.DataRow)Container.DataItem)["CollectionPointID"]  + "');\" onmouseout=\"closeToolTip();\" style=\"font-weight:bold;\">" + ((System.Data.DataRow)Container.DataItem)["CollectionPointDescription"] + "</span>"%>
                            <%# GetDate((DateTime)((System.Data.DataRow)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRow)Container.DataItem)["CollectionIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Deliver Details" SortExpression="DeliveryPointDescription" ItemStyle-Width="200">
                        <ItemTemplate>
                            <%#"<span onmouseover=\"javascript:ShowPointToolTip(this, '" + ((System.Data.DataRow)Container.DataItem)["DeliveryPointID"] + "');\" onmouseout=\"closeToolTip();\" style=\"font-weight:bold;\">" + ((System.Data.DataRow)Container.DataItem)["DeliveryPointDescription"] + "</span>"%>
                            <%# GetDate((DateTime)((System.Data.DataRow)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRow)Container.DataItem)["DeliveryIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Delivery Order Number">
                        <ItemTemplate><%# ((System.Data.DataRow)Container.DataItem)["DeliveryOrderNumber"]%></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Delivering Reource">
                        <ItemTemplate><%# ((System.Data.DataRow)Container.DataItem)["DeliveringResource"]%></ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="CurrentRate" HeaderText="Rate" SortExpression="ForeignRate">
                        <ItemTemplate>
                            <asp:Label ID="lblRate" runat="server"></asp:Label>&nbsp;
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <NoRecordsTemplate>
                    No Orders Found.
                </NoRecordsTemplate>
            </MasterTableView>
            <ClientSettings AllowDragToGroup="false">
                <Selecting AllowRowSelect="false" />
            </ClientSettings>
        </telerik:RadGrid>
    </div>
    
    <div><asp:HiddenField ID="hdnItemID" runat="server" Value="" /></div>

    <script type="text/javascript">
    <!--
        var hdnItemID = $("#" + "<%=hdnItemID.ClientID %>");
    
        function viewOrderProfile(orderID) {
            var qs = "oID=" + orderID;
            <%=dlgOrder.ClientID %>_Open(qs);
        }

        function selectedOrder(chkBox) {
            var btnMatchOrder = $("#" + "<%=btnMatchOrder.ClientID%>");
            var showConfirm = false;
        
            //var jchkBox = $("#" + chkBox.id);

            if (chkBox != null) {
                var itemID = $("#" + chkBox.id).attr("ItemID");
                hdnItemID.val(itemID);
            
                showConfirm = chkBox.checked;
                var checkBoxes = $(".chkGroupOrderSelect:checked");

                if(checkBoxes.length > 0)
                    for (var i = 0; i < checkBoxes.length; i++) {
                        if (checkBoxes[i].id != chkBox.id)
                            checkBoxes[i].checked = false;
                    }
            }

            if (showConfirm)
                btnMatchOrder.show();
            else
                btnMatchOrder.hide();                    
        }
        
        function showLoading(messageContent) {
            $.blockUI({
            message: '<div style="margin-left:30px;"><span id="UpdatableMessage">' + messageContent + '</span></div>',
            css: {        
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: '.5',
                color: '#fff'
            }
            });
        }
        
        function hideLoading() {
            $.unblockUI();
        }
    //-->
    </script>

</asp:Content>