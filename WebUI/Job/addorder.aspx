<%@ Page Title="" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="addorder.aspx.cs" Inherits="Orchestrator.WebUI.Job.addorder"  %>
<%@ Register TagPrefix="orchestrator" TagName="point" Src="~/UserControls/point.ascx" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >

<telerik:RadWindowManager ID="rmwOpenOrderWindow" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
    <Windows>
        <telerik:RadWindow runat="server" ID="largeWindow" Height="1180" Width="900" />
    </Windows>
</telerik:RadWindowManager>

<script language="javascript" src="../script/tooltippopups.js" type="text/javascript"></script>
<cc1:Dialog ID="dlgCreateOrder" URL="/groupage/ManageOrder.aspx" Width="1180" Height="900" AutoPostBack="true" Mode="Modal"
    runat="server" ReturnValueExpected="true">
</cc1:Dialog>

<h1>Order Details</h1>
<h2><asp:Label ID="lblOrderInformation" runat="server"></asp:Label></h2>
<asp:Label ID="lblError" runat="server" CssClass="errorMessage" Visible="false" EnableViewState="false"></asp:Label>
<script type="text/javascript">
<!--
$(document).ready(function() {
    $('#<%=txtSearch.ClientID %>').keydown(function(event) { if (event.keyCode == 13) {<%=Page.GetPostBackEventReference(btnFindOrder)%>; return false; } });
});
//-->
</script>
    
<asp:MultiView runat="server" ID="mvAddOrder" >
    <!-- Find the Order to Add -->
    <asp:View runat="server" ID="vwFindOrder">
        <div>
            <table>
                <tr>
                    <td>Search:</td>
                    <td colspan="3"><asp:TextBox ID="txtSearch" runat="server" Width="125" ></asp:TextBox><asp:CheckBox id="chkSearchByOrderID" runat="server" Text="Limit the Search to use the Order ID only." Checked="true" /></td>
                </tr>
                <tr>
                    <td >Start Date: </td><td><telerik:RadDateInput ID="dteStartDate" runat="server" DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy"></telerik:RadDateInput></td>
                    <td>End Date: <telerik:RadDateInput ID="dteEndDate" runat="server" DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy"></telerik:RadDateInput></td>
                </tr>
                <tr>
                    <td>Search By Client:</td><td colspan="3">
                     <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                    </telerik:RadComboBox>
                    </td>
                </tr>
                <tr>
                 <td>Resource</td>
                <td colspan="3">
    	            <telerik:RadComboBox ID="cboResource" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>Sub-Contractor</td>
                <td colspan="3">
                    <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                    </telerik:RadComboBox>
                </td>
            </tr>
            </table>
        </div>
        <div class="buttonbar">
        <asp:Button  ID="btnFindOrder" runat="server" Text="Find Order" /><asp:Button ID="btnCreateOrder" runat="server" OnClientClick="javascript:CreateOrder(); return false;" Text="Create Order" /><asp:Button  ID="btnAddOrder" runat="server" Text="Add Order" Visible="false" />
        </div>
        <div style="height: 320px; overflow: scroll;">
        <telerik:RadGrid runat="server" ID="grdOrders" Width="97%" AllowPaging="false" ShowGroupPanel="true" allowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false">
            <MasterTableView Width="100%" ClientDataKeyNames="OrderID" DataKeyNames="OrderID">
                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="bob" ItemStyle-Width="18" HeaderStyle-Width="18">
                    </telerik:GridClientSelectColumn>
                    <telerik:GridBoundColumn HeaderText="ID" DataField="OrderID" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                    <telerik:GridHyperLinkColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataNavigateUrlFormatString="javascript:viewOrderProfile({0})" DataNavigateUrlFields="OrderID" DataTextField="CustomerOrganisationName"></telerik:GridHyperLinkColumn>
                    <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessTypeDescription" DataField="BusinessTypeDescription"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                    <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Collect Details" SortExpression="CollectionPointDescription" ItemStyle-Width="200">
                        <ItemTemplate>
                            <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("CollectionPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("CollectionPointDescription")%></span>
                            <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Deliver Details" SortExpression="DeliveryPointDescription" ItemStyle-Width="200">
                        <ItemTemplate>
                            <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("DeliveryPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("DeliveryPointDescription")%></span>
                            <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                    <telerik:GridBoundColumn HeaderText="Delivering Resource" SortExpression="DeliveringResource" DataField="DeliveringResource" />
                    <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="ForeignRate">
                        <ItemTemplate>
                            <asp:Label ID="lblRate" runat="server"></asp:Label>&nbsp;
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings AllowDragToGroup="false" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                <Selecting AllowRowSelect="true" />
                <Resizing AllowColumnResize="true" AllowRowResize="false" />
            </ClientSettings>
        </telerik:RadGrid>
        </div>
    </asp:View>
    <!-- Show the Options Depending on the Instruction selected and the current position -->
    <asp:View runat="server" ID="vwCollectionInstruction">
        <div>
            <asp:label ID="lblCollectionInstructionInfo" runat="server"></asp:label>
            <asp:label ID="lblCollectionActiontext" runat="server">What would you like to do ?</asp:label><div><asp:RadioButtonList runat="server" ID="rblCollectionInstructionDeliveryAction" AutoPostBack="true"></asp:RadioButtonList></div>
            <asp:Panel ID="pnlWehere" runat="server" Visible="false">
                <div><asp:Label ID="lblWhereText" runat="server"></asp:Label></div>
                <asp:RadioButtonList ID="rblCollectionInstructionDeliveryPoint" runat="server" AutoPostBack="true"></asp:RadioButtonList>
            </asp:Panel>
        </div>
        <div class="buttonbar">
        <asp:Button ID="btnAddOrderToCollection" runat="server" Text="Add Order" />
        </div>
    </asp:View>
    <asp:View runat="server" ID="vwDropInstruction">
        <div>
            <asp:label ID="lblDropInstructionInfo" runat="server"></asp:label>
            <div><asp:RadioButtonList runat="server" ID="rblDropInstructionDeliveryAction" AutoPostBack="true"></asp:RadioButtonList></div>
            <asp:Panel ID="pnlLoadFrom" runat="server" Visible="false">
                <div><asp:Label ID="lblDropWhereText" runat="server"></asp:Label></div>
                <asp:PlaceHolder ID="phDeliveryPoints" runat="server"></asp:PlaceHolder>
                <asp:RadioButtonList ID="rblDropInstructionDeliveryPoint" runat="server" ></asp:RadioButtonList>
            </asp:Panel>
        </div>
        <div class="buttonbar">
            <asp:Button ID="btnAddOrderToDrop" runat="server" Text="Add Order" />
        </div>
    </asp:View>
    <asp:View runat="server" ID="vwTrunkInstruction">
        <div>
            <asp:label ID="lblTrunkInstructionInfo" runat="server"></asp:label>
                <div><asp:Label ID="lblLoadTrunkWhere" runat="server"></asp:Label></div>
                <asp:RadioButtonList ID="rblTrunkInstructionLoadPoint" runat="server" ></asp:RadioButtonList>
        </div>
        <div class="buttonbar">
            <asp:Button ID="btnAddOrderToTrunk" runat="server" Text="Add Order" />
        </div>
    </asp:View>
    <asp:View runat="server" ID="vwUntetheredOrder">
        <div>
            <div>What would you like to do with this order?</div>
            <asp:RadioButtonList ID="rblUntetheredOrderIntention" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" RepeatColumns="5"></asp:RadioButtonList>
            <br />
            <div>Pickup the Order from</div>
            <asp:RadioButtonList ID="rblUntetheredOrderStartPoint" runat="server"></asp:RadioButtonList>
            <asp:Panel ID="pnlUntetheredOrderStartPointPicker" runat="server">
                <orchestrator:point ID="ucUntetheredStartPoint" runat="server" CanCreateNewPoint="false" CanUpdatePoint="false" ShowFullAddress="false" />
            </asp:Panel>
            <br />
            <div>Take the Order to</div>
            <asp:RadioButtonList ID="rblUntetheredOrderEndPoint" runat="server"></asp:RadioButtonList>
            <asp:Panel ID="pnlUntetheredOrderEndPointPicker" runat="server">
                <orchestrator:point ID="ucUntetheredEndPoint" runat="server" CanCreateNewPoint="false" CanUpdatePoint="false" ShowFullAddress="false" />
            </asp:Panel>
            <br />
            <asp:HiddenField ID="hidUntetheredControl" runat="server" />
        </div>
        <div class="buttonbar">
            <asp:Button ID="btnAddUntetheredOrderToJob" runat="server" Text="Add Order" CausesValidation="false" />
        </div>
    </asp:View>
</asp:MultiView>

<script type="text/javascript">
<!--

    function CreateOrder() { 
    
        var qs = "returnId=true";
        qs += "&wiz=true";

        <%=dlgCreateOrder.ClientID %>_Open(qs);
    }
    
    function viewOrderProfile(orderID)
    {
        var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;
        var wnd = radopen("about:blank", "largeWindow");                               
        wnd.SetUrl(url);
        wnd.SetTitle("Add/Update Order");
    }

    function SetCustomPointVisibility(radioButton, pointPickerPanelID)
    {
        if (radioButton != null)
        {
            var selectedValue = radioButton.value;

            if (selectedValue != null)
            {
                var pointPickerPanel = document.getElementById(pointPickerPanelID);
                if (pointPickerPanel != null)
                {
                    if (selectedValue == '')
                        pointPickerPanel.style.display = '';
                    else
                        pointPickerPanel.style.display = 'none';
                }
            }
        }
    }
//-->
</script>

</asp:Content>
