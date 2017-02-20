<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.autorunstart" Title="Groupage Auto Batch Run Invoice" Codebehind="autorunstart.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Orders Ready To Invoice</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
   
    <asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnRefresh">

        <fieldset>
            <legend>Ready To Invoice - Please Note that this page has been replaced by the Multi-Batch Invoice Screen and is no longer supported</legend>
            <table>
                <tr>
                     <td runat="server" id="tdDateOptions" >
                        <table>
                        <tr>
                            <td class="formCellLabel">Date From</td>
                            <td class="formCellField">
                                <telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100" ToolTip="The start date for the filter" >
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Date To</td>
                            <td class="formCellField">
                                <telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100" ToolTip="The end date for the filter" >
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                            <td class="formCellField"></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Client</td>
                            <td class="formCellField"><telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px" Height="300px" SelectOnTab="true"
                                DataTextField="OrganisationName" DataValueField="IdentityID" ></telerik:RadComboBox></td>
                            <td class="formCellField"></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Type</td>
                            <td class="formCellField">
                                <asp:RadioButtonList ID="rdoListInvoiceType" runat="server" RepeatDirection="Horizontal" RepeatColumns="3"> 
                                   <asp:ListItem Text="Normal" Value="NORM" Selected="True"/>
                                   <asp:ListItem Text="Self Bill" Value="SELF" />
                                   <asp:ListItem Text="Both" Value="BOTH"/>
                                </asp:RadioButtonList>
                            </td>
                            <td></td>
                        </tr>
                       </table>
                    </td>
                </tr>
            </table>
            <div class="buttonbar">
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" />
            </div>
        </fieldset>
    </asp:Panel>

    <asp:Label ID="lblError" runat="server"></asp:Label>
 
    <div class="buttonbar">
        <asp:Button ID="Button1" runat="server" Text="Create Batch" />
    </div>
    
    <div style="background-color:White;">
        <table>
            <tr>
                <td>Invoice Date</td>
                <td nowrap="nowrap">
                    <telerik:RadDatePicker ID="rdiInvoiceDate" runat="server" Width="100" ToolTip="The invoice date." >
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                </td>
                <td>
                    <asp:RequiredFieldValidator ID="rfvInvoiceDate" runat="server" ControlToValidate="rdiInvoiceDate" Display="dynamic"><img src="/images/ico_critical_small.gif" height="16" width="16" title="Please supply an invoice date." alt="" /></asp:RequiredFieldValidator>
                </td>

                <asp:CustomValidator ID="cvInvoiceDate" runat="server" Enabled="true" ControlToValidate="rdiInvoiceDate"
	                Display="Dynamic" EnableClientScript="true" ClientValidationFunction="ValidateInvoiceDate"
	                ErrorMessage="This Date may be Wrong">
	        </asp:CustomValidator>


            </tr>
        </table>
    </div>
    
    <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView Width="100%" ClientDataKeyNames="OrderID,OrderGroupID" DataKeyNames="OrderID,OrderGroupID" Name="Master">
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
             <DetailTables>
                <telerik:GridTableView DataKeyNames="OrderID" AutoGenerateColumns="false" Name="Detail">
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="OrderID" MasterKeyField="OrderID" />
                    </ParentTableRelation>
                    <Columns>
                        
                        <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription" />
                        <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                        <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="60" UniqueName="NoPallets" />
                        <telerik:GridBoundColumn HeaderText="Pallet Type" SortExpression="PalletTypeDescription" DataField="PalletTypeDescription" HeaderStyle-Width="80" />
                        <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80">
                            <ItemTemplate>
                                <%# ((decimal)((System.Data.DataRowView)Container.DataItem)["Weight"]).ToString("F4")%>
                                <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="false"/>
                    </Columns>
                </telerik:GridTableView>
            </DetailTables>
            <Columns>
                <telerik:GridTemplateColumn UniqueName="CheckColumn" HeaderStyle-Width="40" HeaderText="">
                    <HeaderTemplate>
                        <asp:checkBox id="chkSelectAll" runat="server" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                <telerik:GridBoundColumn HeaderText="Single Inv" SortExpression="InvoiceSeperatley" DataField="InvoiceSeperatleyDisplay" />
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" DataField="CollectionPointDescription" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription" DataField="DeliveryPointDescription" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false"  >
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                <telerik:GridTemplateColumn HeaderText="Rate" HeaderStyle-Width="135">
                    <ItemTemplate>
                        <asp:Label ID="lblCharge" runat="server" />
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </telerik:GridTemplateColumn>                
            </Columns>
            <NoRecordsTemplate>
                <div>There are no records to display</div>
            </NoRecordsTemplate>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </telerik:RadGrid>
    
    <div class="buttonbar">
        <asp:Button ID="btnCreateBatch" runat="server" Text="Create Batch" />
    </div>
    
    <script language="javascript" type="text/javascript">
        var groupHandlingIsActive = false;
        
        function HandleGridSelection()
        {
            var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
            var columns = mtv.get_columns();
            var rows = mtv.get_dataItems();
            var masterChkID = GetCheckBox(columns[1].get_element());

            for (var rowIndex = 0; rowIndex < rows.length; rowIndex++)
                try
                {
                    var gridRowTable = rows[rowIndex].get_owner();
                    var cell = gridRowTable.getCellByColumnUniqueName(rows[rowIndex], "CheckColumn"); 
                    if(!cell) return;
                    
                    var chkOrderID = GetCheckBox(cell);
                    if(!chkOrderID) return;

                    chkOrderID.checked = masterChkID.checked;
                }
                catch (error){}
        }

        function GetCheckBox(control) { 

          if (!control) return; 
          
          for (var i = 0; i < control.childNodes.length; i++) { 
            if (!control.childNodes[i].tagName) continue; 
              if ((control.childNodes[i].tagName.toLowerCase() == "input") && 
                  (control.childNodes[i].type.toLowerCase() == "checkbox")) { 
                  return control.childNodes[i]; 
              } 
          } 
        } 

        function GroupHandling(orderGroupID, selectRow)
        {
            if (groupHandlingIsActive)
                return;
                
            groupHandlingIsActive = true;
            
            var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
            var rows = mtv.get_dataItems();
            
            for (var rowIndex = 0; rowIndex < rows.length; rowIndex++)
                try
                {
                    if (rows[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID)
                    {
                        var gridRowTable = rows[rowIndex].get_owner();
                        var cell = gridRowTable.getCellByColumnUniqueName(rows[rowIndex], "CheckColumn"); 
                        if(!cell) return;
                    
                        var chkOrderID = GetCheckBox(cell);
                        if(!chkOrderID) return;
                        
                        chkOrderID.checked = selectRow;
                    }
                }
                catch (error)
                {
                }

            groupHandlingIsActive = false;
        }
             
        function HandleSelection(e, src, orderGroupID)
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
                gridRow.className = "SelectedRow_Orchestrator";                
            else
                gridRow.className = "GridRow_Orchestrator";

            // Is the order part of a group that is grouped planning enabled?
            // Automatically select the other orders in the grid that belong to this group.
            if (orderGroupID > 0)
                GroupHandling(orderGroupID, src.checked);
        }

        function ValidateInvoiceDate(source, args) {

            var dteDateTime = $find("<%= rdiInvoiceDate.ClientID %>");
            var dteDateTimeValue = dteDateTime.get_dateInput().get_selectedDate();

            //  Create Today's date
            var today = new Date();
            var upperBound = today.setDate(today.getDate() + 30);
            var today = new Date();
            var lowerBound = today.setDate(today.getDate() - 30);

            if (dteDateTimeValue > upperBound) {
                var r = confirm('Warning. The selected Invoice date is more than 30 days in the future.');
                if (r == true)
                    args.IsValid = true;
                else {
                    args.IsValid = false;
                }
            }

            if (dteDateTimeValue < lowerBound) {
                var r = confirm('Warning. The selected Invoice date is more than 30 days in the past.');
                if (r == true)
                    args.IsValid = true;
                else {
                    args.IsValid = false;
                }
            }
        }

    </script>
     
</asp:Content>