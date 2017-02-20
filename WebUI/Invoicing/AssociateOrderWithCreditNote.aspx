<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.AssociateOrderWithCreditNote" Title="Haulier Enterprise" Codebehind="AssociateOrderWithCreditNote.aspx.cs" %>


<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Find Orders to Associate</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript" src="/script/scripts.js"></script>
    <script language="javascript" type="text/javascript" src="/script/tooltippopups.js"></script>
    
    <style type="text/css">
        body {font-size:12px;}
    </style>

    <script language="javascript" type="text/javascript">
    <!--
        window.onload = function(){DisableClient();}
        function DisableClient()
        {
            var cboClient = <%=cboClient.ClientID%>;
        }
        
        var orders = "";
        
        function ChangeList(e, src, orderID, noPallets)
        {
            var gridRow;
		  
            if(e.target)
            {
                gridRow = e.target.parentNode.parentNode;
            }
            else
            {
                gridRow = e.srcElement.parentNode.parentNode;
            }
		  
            if (src.checked)
            {
                // Add to the list
                if(orders.length > 0)
                    orders += ",";
                orders += orderID; 
                gridRow.className= "SelectedRow_Office2007";
            }
            else
            {
                // remove from the list
                orders = orders.replace(orderID + ",", "");
                orders = orders.replace("," + orderID, "");
                orders = orders.replace(orderID, "");
                gridRow.className= "GridRow_Office2007";
            }
        }
        
        function GetRadWindow()
        {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well)
            return oWindow;
        }
        
        function CloseWindow()
        {
            GetRadWindow().Close();
        }
    //-->
    </script>
    
    <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
        <div style="height:22px; border-bottom:solid 1pt silver; padding:2px; margin-bottom:5px; color:#ffffff; background-color:#5d7b9d; font-size:11px; font-weight:bold;">Find Orders To Associate with this Order Group</div>
        <p style="font-size:12px">Use this screen to find orders in the system, then select the orders you wish to associate.</p>
        <uc1:infringementDisplay runat="server" ID="ruleInfringements" Visible="false" EnableViewState="false"/>
        <table>
            <tr>
                <td>Order Status</td>
                <td colspan="2"><asp:checkboxlist runat="server" id="cblOrderStatus" repeatdirection="horizontal" ></asp:checkboxlist></td>
            </tr>
            <tr valign="top">
                <td>Search for <ul><li>Customer Order Number</li><li>Delivery Order Number</li><li>Order ID</li></ul></td>
                <td><asp:TextBox ID="txtSearch" runat="server" ></asp:TextBox></td>
                <td>                    
                    <table>
                        <tr>
                            <td>Date From</td>
                            <td><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                        </tr>
                        <tr>
                            <td>Date To</td>
                            <td><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                            <td></td>
                        </tr>
                   </table>
                   <asp:RadioButtonList ID="cboSearchAgainstDate" runat="server" RepeatDirection="horizontal" RepeatColumns="3">
                        <asp:ListItem Text="Collection Date" Value="COL"></asp:ListItem>
                        <asp:ListItem Text="Delivery Date" Value="DEL" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="Both" Value="BOTH"></asp:ListItem>
                   </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td>Client</td>
                <td colspan="2">
    	            <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False" ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px"></telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>Resource</td>
                <td colspan="2">
    	            <telerik:RadComboBox ID="cboResource" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td>Sub-Contractor</td>
                <td colspan="2">
                    <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px">
                    </telerik:RadComboBox>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlConfirmation" runat="server" Visible="false" EnableViewState="false">
            <div class="MessagePanel" style="vertical-align:middle;">
                <table><tr><td><asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /></td><td><asp:Label cssclass="ControlErrorMessage" id="lblNote" runat="server" /></td></tr></table>
            </div>
        </asp:Panel>
        <div style="height:22px; margin-top:5px;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;">            
            <asp:Button ID="btnSearch" runat="server" Text="Search" Width="75" />
        </div>     
    </fieldset>
    
    <br />
    
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    
    <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView Width="100%" DataKeyNames="OrderID">
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <DetailTables>
                <telerik:GridTableView DataKeyNames="OrderID"  AutoGenerateColumns="false"  >
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="OrderID" MasterKeyField="OrderID" />
                    </ParentTableRelation>
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="D/O Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                        <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription" />
                        <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                        <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="false"/>
                    </Columns>
                </telerik:GridTableView>       
            </DetailTables>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="chkSelectColumn"  ItemStyle-Width="10">
                    <ItemTemplate>
                        <asp:CheckBox id="chkOrderID" runat="server"  />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="ID" SortExpression="OrderID" UniqueName="OrderID" DataField="OrderID"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" UniqueName="CustomerOrganisationName" DataField="CustomerOrganisationName"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" UniqueName="CollectionPointDescription" DataField="CollectionPointDescription"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionRunDeliveryDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription" UniqueName="DeliveryPointDescription" DataField="DeliveryPointDescription"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false"  >
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="60" UniqueName="NoPallets" />
                <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80">
                    <ItemTemplate>
                        <%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["Weight"].ToString()).ToString("N2")%>
                        <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>                      
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
        </ClientSettings>
    </telerik:RadGrid>
    
    <div class="buttonBar">            
        <asp:Button ID="btnClose" runat="server" Text="Close" CausesValidation="false" />
        <asp:Button ID="btnAssociate" runat="server" Text="Associate" Width="75" />
    </div>     
    
</asp:Content>