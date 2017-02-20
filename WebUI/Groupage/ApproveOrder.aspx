<%@ Page Language="C#" AutoEventWireup="True" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.Groupage.ApproveOrder" Codebehind="ApproveOrder.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadWindowManager ID="rmwApproveOrder" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
        </Windows>
    </telerik:RadWindowManager>
    
    <h1>Approve Orders</h1>
    <h2>A list of orders that require approval are shown below</h2>
    <div>
        <fieldset>
            <legend>Confirm / Reject</legend>
            <table style="float:left;">
                <tr>
                    <td class="formCellLabel">Rejection reason required</td>
                    <td class="formCellField"><asp:TextBox TextMode="MultiLine" Height="48" width="500" runat="server" ID="txtRejectionReasonTop" ValidationGroup="Rejection"></asp:TextBox></td>
                </tr>
            </table>
            <div class="buttonBar" style="float:left; margin-top:16px;">
                <input type="button" value="Clear" onclick="clearRejectReason();" />
                <asp:Button id="btnConfirmOrdersTop" Width="125" runat="server" Text="Confirm Orders" />
                <asp:Button id="btnRejectOrdersTop" Width="125" runat="server" OnClientClick="if(!RejectOrders()) {return false;}" Text="Reject Orders" />
            </div>
            <div class="clearDiv"></div> 
        </fieldset>
    </div>
    
    <div class="buttonBar">
        <asp:button id="btnRefreshTop" runat="server" text="Refresh list" Width="100" />
    </div>
    
    <div class="overlayedDataBox">
        <div class="buttonbar" style="margin-bottom:1px;margin-top:3px;">
            <asp:button ID="btnSaveChangesShortcut" runat="server" Text="Update Order Information" style="width:187px; font-size:11px;" />
        </div>
    </div>  

    <telerik:RadGrid runat="server" ID="ordersRadGrid" AllowFilteringByColumn="false" AllowMultiRowSelection="true" AllowSorting="False" AutoGenerateColumns="false" EnableAjaxSkinRendering="true">
        <MasterTableView DataKeyNames="OrderID">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="chkSelectColumn" HeaderStyle-Width="10" AllowFiltering="false">
                    <HeaderTemplate>
                        <input type="checkbox" ID="chkSelectAll" onclick="javascript:selectAllCheckboxes(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox id="chkSelectOrder" runat="server"  />
                        <input type="hidden" runat="server" id="hidOrderId" />
                        <asp:HiddenField runat="server"  ID="hidOrderChanged" Value="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="Client" DataField="CustomerOrganisationName" HeaderText="Client" AllowFiltering="true" />
                <telerik:GridHyperLinkColumn UniqueName="OrderID" HeaderText="Order ID" DataTextField="OrderID" DataNavigateUrlFormatString="javascript:viewOrderProfile({0});" DataNavigateUrlFields="OrderID" SortExpression="OrderID"  AllowFiltering="false" />
                <telerik:GridBoundColumn UniqueName="LoadNo" DataField="CustomerOrderNumber" HeaderText="Load No" AllowFiltering="true" />
                <telerik:GridTemplateColumn UniqueName="CollectFrom" HeaderText="Collect From" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCollectFromPoint"></asp:Label>
                        <%#Eval("CollectionPointDescription") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="CollectAt1" HeaderText="Collect At" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:TextBox ID="txtCollectAt" runat="server" Width="70px" onchange="SetDirtyFlag(this)" EnableViewState="false" />
                        <asp:TextBox ID="txtCollectionAtTime" runat="server" Width="50px" onchange="SetDirtyFlag(this)" EnableViewState="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="DeliverTo" HeaderText="Deliver To" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDeliverToPoint"></asp:Label>
                        <asp:Literal ID="litDeliverTo" runat="server" EnableViewState="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="DeliverAt" HeaderText="Deliver At" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:TextBox ID="txtDeliverAt" runat="server" Width="70px" onchange="SetDirtyFlag(this)" EnableViewState="false" />
                        <asp:TextBox ID="txtDeliverAtTime" runat="server" Width="50px" onchange="SetDirtyFlag(this)" EnableViewState="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="DeliveryOrderNumber" DataField="DeliveryOrderNumber" HeaderText="Delivery Order Number" AllowFiltering="false" />
                <telerik:GridBoundColumn UniqueName="NoOfPallets" DataField="NoPallets" HeaderText="No of Pallets" ItemStyle-HorizontalAlign="Right" AllowFiltering="false" />
                <telerik:GridBoundColumn UniqueName="Weight" HeaderText="Weight (Kg)" DataField="Weight" DataFormatString="{0:N0}" ItemStyle-HorizontalAlign="Right" SortExpression="Weight" AllowFiltering="false" />
                <telerik:GridTemplateColumn UniqueName="ForiegnRate1" HeaderText="Rate" ItemStyle-HorizontalAlign="Right" SortExpression="ForeignRate" AllowFiltering="false">
                    <ItemTemplate>
                        <asp:TextBox ID="txtRate" runat="server" Width="75px" onchange="SetDirtyFlag(this)" EnableViewState="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="Notes" DataField="Notes" HeaderText="Notes" ItemStyle-Width="100px" AllowFiltering="false" />
                <telerik:GridBoundColumn UniqueName="ServiceLevel" DataField="OrderServiceLevel" HeaderText="Service Level" ItemStyle-Width="100px" AllowFiltering="false" />
            </Columns>                 
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="True" EnableDragToSelectRows="true" ></Selecting>
        </ClientSettings>
    </telerik:RadGrid>
    
    <div class="buttonbar">
        <asp:Button ID="btnSaveChanges" runat="server" Text="Update Order Information" />
    </div>
    
    <telerik:RadAjaxManager ID="ramApproveOrder" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnConfirmOrdersTop" EventName="Click">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ordersRadGrid" LoadingPanelID="ralpApproveOrder" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnRejectOrdersTop" EventName="Click">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ordersRadGrid" LoadingPanelID="ralpApproveOrder" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnRefreshTop" EventName="Click">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ordersRadGrid" LoadingPanelID="ralpApproveOrder" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSaveChanges" EventName="Click">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ordersRadGrid" LoadingPanelID="ralpApproveOrder" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnSaveChangesShortcut" EventName="Click">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="ordersRadGrid" LoadingPanelID="ralpApproveOrder" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
    <telerik:RadAjaxLoadingPanel ID="ralpApproveOrder" runat="server" Transparency="10" BackColor="#d3e5eb">
        <table width='100%' cellpadding='100px;' height='70%'>
            <tr>
                <td align="center" valign="top">
                    <img src='/images/postbackLoading.gif' />
                </td>
            </tr>
        </table>
    </telerik:RadAjaxLoadingPanel>
    
    <telerik:RadInputManager ID="rimApproveOrder" runat="server" >
        <telerik:DateInputSetting BehaviorID="DateInputBehavior1" InitializeOnClient="false" Culture="en-GB" EmptyMessage="Enter Collection Date" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" ></telerik:DateInputSetting>
        <telerik:DateInputSetting BehaviorID="DateInputBehavior2" InitializeOnClient="false" Culture="en-GB" EmptyMessage="Enter Delivery Date" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" ></telerik:DateInputSetting>
        <telerik:DateInputSetting BehaviorID="TimeInputBehavior1" InitializeOnClient="false" Culture="en-GB" EmptyMessage="Anytime" DateFormat="HH:mm" DisplayDateFormat="HH:mm"></telerik:DateInputSetting>
        <telerik:NumericTextBoxSetting BehaviorID="NumericBehavior1" InitializeOnClient="false" Culture="en-GB" Type="Currency" DecimalDigits="2"></telerik:NumericTextBoxSetting>
    </telerik:RadInputManager>
   
    <telerik:RadCodeBlock runat="server">
        <script language="javascript" type="text/javascript">
            function viewOrderProfile(orderID)
            {               
                var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" +  orderID;

                var wnd = window.radopen("about:blank", "largeWindow");                               
                wnd.SetUrl(url);
                wnd.add_close(clientClose); 
                wnd.SetTitle("Add/Update Order");
            }

            function clearRejectReason()
            {
                var rejectionReasonTextBox = document.getElementById('<%=txtRejectionReasonTop.ClientID%>');
                rejectionReasonTextBox.value = '';
                rejectionReasonTextBox.focus();
            }

            function RejectOrders()
            {
                var rejectionReasonTextBox = document.getElementById('<%=txtRejectionReasonTop.ClientID%>');
                if(rejectionReasonTextBox.value == '')
                {
                    alert('Please enter a rejection reason.'); 
                    rejectionReasonTextBox.focus();
                    return false;
                }
            
                var test;
                var selectedItems = $find("<%=ordersRadGrid.ClientID %>").get_masterTableView().get_selectedItems();

                if(selectedItems.length > 0)
                    test = confirm("Are you sure you want to reject the selected orders?");
                else    
                    test = false;
                    
                return test;
            }
            
            // Fires when the alter view order window is closed wired up by calling the viewOrder method with the name of the function
            // as the callback parameter.
            function clientClose()
            {
                var refreshButton = document.getElementById("<%=btnRefreshTop.ClientID %>");
                refreshButton.click();
            }
            
            function ChangeList(e,src)
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
                    gridRow.className= "SelectedRow_Orchestrator";
                }
                else
                {
                    gridRow.className= "GridRow_Orchestrator";
                }
            }

            function selectAllCheckboxes(chk) 
            {
                $('table[id*=ordersRadGrid] input:enabled[id*=chkSelectOrder]').prop('checked', chk.checked);
            }
            
            function SetDirtyFlag(item)
            {
                //set the row as dirty
                var hidID = $get($get(item.id).getAttribute('hidOrderID'))
    
                if(hidID != null && hidID != undefined)
                    hidID.value = "true";
            }            
        </script>
    </telerik:RadCodeBlock>
    
</asp:Content>
