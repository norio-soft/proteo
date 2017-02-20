<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" Title="Shunt Loading Sheet" CodeBehind="ShuntLoadingSheet.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.ShuntLoadingSheet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js" ></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Shunt Loading Sheet</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset>
        <legend>Filter Options</legend>
        <table width="100%">
            <tr>
                <td runat="server" id="tdDateOptions" >
                    <table width="100%">
                        <tr>
                            <td class="formCellLabel" style="width:10%;">
                                Loading Date
                            </td>
                            <td class="formCellField" style="width:5%;"><telerik:RadDatePicker id="dteLoadingDate" runat="server" Width="100" ToolTip="The loading Date for the filter">
                                <DateInput ID="DateInput1" runat="server"
                                dateformat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                            <td class="formCellField">
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="dteLoadingDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter a Loading Date" Display="Dynamic"></asp:RequiredFieldValidator>
                            </td>
                            <td class="formCellField" style="width:40%;">
                                <asp:RadioButtonList ID="rdoLoaded" runat="server" AutoPostBack="true" RepeatDirection="Horizontal" RepeatColumns="4"> 
                                    <asp:ListItem Text="Not Loaded" Value="0" Selected="True" />
                                    <asp:ListItem Text='Not Loaded Excluding "Live Loads"' Value="4" Selected="false" />
                                    <asp:ListItem Text="Loaded" Value="1" />
                                    <asp:ListItem Text="All" Value="2"/>
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </fieldset>

    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" />
        <asp:Button ID="btnPrint" runat="server" Text="Print" OnClientClick="JavaScript:window.print();" ValidationGroup="grpRefresh" CausesValidation="true" />
        <asp:Button ID="btnUpdate" runat="server" Text="Save" ValidationGroup="grpRefresh" CausesValidation="true" />
        <asp:label ID="lblTotalOrders" runat="server" ForeColor="white" Font-Size="Larger" Font-Bold="false"></asp:label>
        <asp:Label ID="lblTotalGroups" runat="server" ForeColor="white" Font-Size="Larger" Font-Bold="false"></asp:Label>
	</div>
    
    <div class="RadGrid_OrchestratorBig">
        <telerik:RadGrid ID="grdShuntLoading" runat="server" AllowSorting="true" AutoGenerateColumns="false" Width="100%" EnableLinqExpressions="false">
            <MasterTableView>
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Order Id" HeaderStyle-Width="5%">
                        <ItemTemplate>
                            <a id="ancOrderId" runat="server" ></a>
                            <asp:HiddenField ID="hidRowDirty" runat="server" Value="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="priority" HeaderText="Priority" HeaderStyle-Width="5%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtPriority" runat="server" Width="98%" onchange="javascript:rowDirty_OnChange(this);" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="releaseNumber" HeaderText="Release No" HeaderStyle-Width="15%">
                        <ItemTemplate>
                            <asp:Label ID="lblReleaseNumber" runat="server" Width="98%"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="deliveryPoint" HeaderText="Point Description" HeaderStyle-Width="20%" SortExpression="DeliveryPoint.Description">
                        <ItemTemplate>
                            <asp:Label ID="lblDeliveryPoint" runat="server" Width="98%"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="time" HeaderText="Time" HeaderStyle-Width="7%">
                        <ItemTemplate>
                            <asp:Label id="lblDeliveryTime" runat="server" Width="98%"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="weight" HeaderText="Weight (KG)" HeaderStyle-Width="8%">
                        <ItemTemplate>
                            <asp:Label id="lblWeight" runat="server" Width="98%"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="dropOrder" HeaderText="Drop Order" HeaderStyle-Width="5%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtDropOrder" runat="server" Width="98%" onchange="javascript:rowDirty_OnChange(this);" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="isLiveLoader" HeaderText="Is Live Loader" HeaderStyle-Width="5%">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkIsLiveLoader" runat="server" onclick="javascript:chkIsLiveLoader_CheckChanged(this);" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="loadingNow" HeaderText="Loading Now" HeaderStyle-Width="12%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtLoadingNow" runat="server" Width="98%" onchange="javascript:rowDirty_OnChange(this);" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="plannedTrailerRef" HeaderText="Trailer No Or Subby" HeaderStyle-Width="15%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtPlannedTrailerRef" runat="server" Width="98%" onchange="javascript:rowDirty_OnChange(this);" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="actualTrailerRef" HeaderText="Despatched Trailer" HeaderStyle-Width="15%">
                        <ItemTemplate>
                            <asp:TextBox ID="txtActualTrailerRef" runat="server" Width="98%" onchange="javascript:trailerDirty_OnChange(this);" />
                            <asp:HiddenField ID="hidTrailerIsDirty" runat="server" Value="false" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Resizing AllowColumnResize="true" AllowRowResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="true" ClipCellContentOnResize="true" />
            </ClientSettings>
    </telerik:RadGrid>
    
    <div class="buttonbar">
        <asp:Button ID="btnBottomRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" />
        <asp:Button ID="btnBottomPrint" runat="server" Text="Print" OnClientClick="JavaScript:window.print();" ValidationGroup="grpRefresh" CausesValidation="true" />
        <asp:Button ID="btnBottomUpdate" runat="server" Text="Save" ValidationGroup="grpRefresh" CausesValidation="true" />
    </div>
    
    </div>
    
    <script type="text/javascript">
        function viewOrderProfile(orderID) {
            var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

            var wnd = window.open(url, "Order", "width=1180, height=900, resizable=1, scrollbars=1");
        }

        function HandleGroupedOrders(txtPriority) {
            var priority = $(txtPriority).attr('value');
            var orderGroupId = $(txtPriority).attr('OrderGroupId');
            
            if(orderGroupId != null && orderGroupId != "" && orderGroupId > 0) {
                var orderGrouptxtPriority = $('input[id*=txtPriority][OrderGroupId=' + orderGroupId + ']');
                    
                orderGrouptxtPriority.each(function(index,ele) {
                    ele.value = priority;
                });
            
                for (var i = 0; i < orderGrouptxtPriority.length; i++)
                    rowDirty_OnChange(orderGrouptxtPriority[i]);
            }
            else
                rowDirty_OnChange(txtPriority);
        }

        function chkIsLiveLoader_CheckChanged(src) {
            var chkIsLiveLoader = $('#' + src.id);

            if (chkIsLiveLoader != null) {
                var groupAttribute = chkIsLiveLoader.attr("OrderGroupId");

                if (groupAttribute != undefined && groupAttribute > 0) {
                    var orderGroupCheckboxes = $('input[id*=chkIsLiveLoader][OrderGroupId=' + groupAttribute + ']');
                    
                    orderGroupCheckboxes.prop("checked", src.checked);

                    for (var i = 0; i < orderGroupCheckboxes.length; i++)
                        rowDirty_OnChange(orderGroupCheckboxes[i]);
                }
                else
                    rowDirty_OnChange(src);
            }
        }

        function rowDirty_OnChange(sender) {
            var currentItem = $('#' + sender.id);
            if (currentItem != null) {
                var currentRow = $('#' + currentItem.parent().parent().attr("id"));
                if (currentRow != null);
                {
                    var rowIsDirty = currentRow.find('input[id*=hidRowDirty]');
                    rowIsDirty.val('true');
                }                
            }            
        }

        function trailerDirty_OnChange(sender) {
            var currentItem = $('#' + sender.id);
            if (currentItem != null) {
                var currentRow = $('#' + currentItem.parent().parent().attr("id"));
                if (currentRow != null);
                {
                    var rowIsDirty = currentRow.find('input[id*=hidTrailerIsDirty]');
                    rowIsDirty.val('true');
                }
            }
        }

        // refresh the data every 10 minutes
        var timerId = setInterval(function () {
            
            $('input[name="#ctl00$ContentPlaceHolder1$btnRefresh"]').click();
        }, (1000 * 60) * 10);
    </script>
</asp:Content>