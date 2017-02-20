<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Invoicing.InvoicePreparationNew"
    CodeBehind="InvoicePreparation.aspx.cs" MasterPageFile="~/default_tableless.master" %>

<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript">
        function checkAll(el) {
            var col = document.getElementsByName("INPUT");
            alert(col.length);
            if (el.checked) {
                for (i = 0; i < col.length; i++) {
                    alert(col.name);
                    if (col[i].name.indexOf("dvJobs") > -1)
                        col[i].checked = true;
                }
            }
            else {
                for (i = 0; i < col.length; i++) {
                    if (col[i].name.indexOf("dvJobs") > -1)
                        col[i].checked = false;
                }
            }
        }
        function onUpdate(oldItem, newItem) {
            // validate ExtraState
            var extraAmount = newItem.GetMember('ExtraAmount').Value;

            if (isNaN(parseFloat(extraAmount))) {
                // failed validation, return to editing
                alert('Extra Amount must be a number.');
                return 2;
            }
            else {
                return 1;

            }
        }

        function editGrid(rowId) {
            dgExtras.Edit(dgExtras.GetRowFromClientId(rowId));
        }

        function editRow() {
            dgExtras.EditComplete();
        }
    </script>
    
    <h1>Invoice Preparation</h1>
    <h2>The Invoice preparation summary is below</h2>

    <asp:Panel ID="pnlMessage" runat="server" Visible="false">
        <div class="MessagePanel">
            <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_warning.gif" />
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>
    </asp:Panel>
    <asp:Label ID="lblJobCount" runat="server" Width="100%" Font-Size="Medium"></asp:Label>
    <asp:Label ID="lblDetails" runat="server" Width="100%"></asp:Label>
    <input type="hidden" id="hidJobCount" runat="server" value="0" />
    <input type="hidden" id="hidJobTotal" runat="server" value="0" />
    <input type="hidden" id="hidSelectedJobs" runat="server" value="" />
    <input type="hidden" id="hidSelectedExtras" runat="server" value="" />
    <input type="hidden" id="hidSelectedExtraJobs" runat="server" value="" />
    <fieldset>
        <legend>Main Filter</legend>
        <table width="100%" border="0">
            <tr>
                <td class="formCellLabel">
                    <asp:Label ID="lblClient" Text="Client" runat="server"></asp:Label>
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" Overlay="true" SelectOnTab="false" runat="server"
                        EnableLoadOnDemand="true" AllowCustomText="false" ShowMoreResultsBox="false"
                        MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" Height="300px" TabIndex="1">
                    </telerik:RadComboBox>
                </td>
                <td class="formCellField" style="vertical-align: top;" rowspan="99" width="40%">
                    <asp:Panel ID="pnlFilterOptions" runat="server" Visible="true">
                        <fieldset>
                            <legend>Filter Options</legend>
                            <asp:Label ID="lblSaveProgressNotification" runat="server" Visible="true"></asp:Label>
                            <nfvc:NoFormValButton ID="btnLoadFilter" runat="server" Visible="true" Text="Load Filter"
                                ServerClick="btnLoadFilter_Click" NoFormValList="rfvClient" CssClass="buttonClass"></nfvc:NoFormValButton>
                            <nfvc:NoFormValButton ID="btnSaveFilter" runat="server" Visible="false" Text="Save Current Filter"
                                ServerClick="btnSaveFilter_Click" CssClass="buttonClass"></nfvc:NoFormValButton>
                            <nfvc:NoFormValButton ID="btnClearFilter" runat="server" Visible="false" Text="Clear Filter"
                                ServerClick="btnClear_Click" CssClass="buttonClass"></nfvc:NoFormValButton>
                        </fieldset>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Date From
                </td>
                <td class="formCellField">
                    <telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"
                        tabindex="2">
                    </telerik:RadDateInput>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    <asp:Label ID="lblJobState" Text="Client" runat="server" Visible="false"></asp:Label>
                </td>
                <td class="formCellField">
                    <asp:DropDownList ID="cboJobState" runat="server" Visible="false">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Date To
                </td>
                <td class="formCellField">
                    <telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" tabindex="3">
                    </telerik:RadDateInput>
                </td>
            </tr>
            <tr>
                <td class="formCellField" colspan="4">
                    <asp:Label ID="lblSelfOnHold" runat="server" Visible="false" CssClass="confirmation"
                        Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonBar" align="left">
        <nfvc:NoFormValButton ID="btnFilter2" runat="server" Text="Generate Jobs To Invoice"
            ServerClick="btnFilter_Click" CssClass="buttonClass"></nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnClear2" runat="server" Text="Clear" ServerClick="btnClear_Click">
        </nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnCreateInvoice2" runat="server" Visible="false" Text="Create Invoice"
            ServerClick="btnCreateInvoice_Click"></nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnExport" Visible="false" runat="server" Text="Export to CSV"
            ServerClick="btnExport_Click"></nfvc:NoFormValButton>
    </div>
    <fieldset>
        <legend><strong>Jobs To Invoice</strong></legend>
        <table width="100%">
            <tr valign="top">
                <td align="right">
                    <p>
                        <asp:CheckBox ID="chkSelfMarkAll" runat="server" Visible="false" Text="Un/Mark All"
                            AutoPostBack="True" TextAlign="Left" Font-Bold="True"></asp:CheckBox>
                    </p>
                </td>
                <td align="left">
                    <p>
                        <asp:CheckBox ID="chkOnlyShowTicked" runat="server" Visible="False" Text="Only Ticked Jobs"
                            TextAlign="Left" AutoPostBack="True" Font-Bold="True"></asp:CheckBox>
                    </p>
                </td>
            </tr>
        </table>
        <asp:GridView ID="dvJobs" runat="server" AllowSorting="true" GridLines="vertical"
            AutoGenerateColumns="false" CssClass="Grid" Width="100%" Visible="true">
            <HeaderStyle CssClass="HeadingRow" Height="25" VerticalAlign="middle" />
            <RowStyle Height="20" CssClass="Row" />
            <AlternatingRowStyle Height="20" BackColor="WhiteSmoke" />
            <SelectedRowStyle Height="20" CssClass="SelectedRow" />
            <Columns>
                <asp:TemplateField HeaderText="">
                    <HeaderTemplate>
                        <input id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server"
                            type="checkbox" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelect" runat="server"></asp:CheckBox>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="OrganisationName" HeaderText="Customer" Visible="true"
                    SortExpression="OrganisationName" />
                <asp:BoundField DataField="IdentityId" HeaderText="ID" Visible="false" />
                <asp:BoundField DataField="LoadNo" HeaderText="Load" SortExpression="LoadNo" />
                <asp:BoundField DataField="DocketNumbers" HeaderText="Docket" SortExpression="DocketNumbers" />
                <asp:BoundField DataField="ChargeAmount" HeaderText="Charge" DataFormatString="{0:C}"
                    HtmlEncode="false" />
                <asp:TemplateField HeaderText="Delivery">
                    <ItemTemplate>
                        <%# Eval("Customers").ToString().Replace(Environment.NewLine, "<br/>") %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Job Id">
                    <ItemTemplate>
                        <input id="hidJobId" type="hidden" value='<%# DataBinder.Eval(Container.DataItem, "JobId") %> '
                            name="hidJobId" runat="server">
                        <a id="lnkJob" href="javascript:openDialogWithScrollbars('~/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%# Eval("JobId").ToString() %>'+ getCSID(), '800','600');">
                            <%# Eval("JobId").ToString() %>
                        </a>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="CollectionPoint" HeaderText="Collection" SortExpression="CollectionPoint" />
                <asp:BoundField DataField="CompleteDate" HeaderText="Date Completed" DataFormatString="{0:dd/MM HH:mm}"
                    HtmlEncode="false" />
                <asp:BoundField DataField="ChargeType" HeaderText="Type" />
                <asp:BoundField DataField="References" HeaderText="References" />
            </Columns>
        </asp:GridView>
        <!-- NOTE: repDeliveryPoints ... Could be firstDeliveryPoint in main dataset if MR T is not happy with this one -->
    </fieldset>
    <!-- EXTRAS FOR SELF BILL INVOICE -->
    <asp:Panel ID="pnlExtraFilter" runat="server" Visible="False">
        <fieldset>
            <legend><strong>Extras</strong></legend>
            <table width="100%">
                <tr>
                    <td valign="top">
                        <fieldset>
                            <legend>Extra Filter</legend>
                            <table>
                                <tr>
                                    <td class="formCellLabel">Job Id</td>
                                    <td class="formCellField">
                                        <asp:TextBox ID="txtExtraJobId" runat="server" />
                                    </td>
                                    <td class="formCellLabel">Date from</td>
                                    <td class="formCellField">
                                        <telerik:RadDateInput id="dteExtraDateFrom" runat="server" dateformat="dd/MM/yy">
                                        </telerik:RadDateInput>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Extra type</td>
                                    <td class="formCellField">
                                        <asp:DropDownList ID="cboExtraType" runat="server" />
                                    </td>
                                    <td class="formCellLabel">Date to</td>
                                    <td class="formCellField">
                                        <telerik:RadDateInput id="dteExtraDateTo" runat="server" dateformat="dd/MM/yy">
                                        </telerik:RadDateInput>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">Extra state</td>
                                    <td class="formCellField">
                                        <asp:DropDownList ID="cboSelectExtraState" runat="server" />
                                    </td>
                                    <td colspan="2" align="right">
                                        <asp:Button ID="btnFilterExtras" CssClass="buttonClass" runat="server" Text="Filter" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </td>
                </tr>
                <tr>
                    <td align="right" valign="top">
                        <asp:Panel ID="pnlExtras" runat="server" Visible="False">
                            <fieldset>
                                <legend><strong>Include extras</strong></legend>
                                <table width="100%">
                                    <tr>
                                        <td>
                                            <div style="width: 100%; height: 400px; overflow: auto;">
                                                <telerik:RadGrid runat="server" ID="extrasRadGrid" Skin="Office2007" AllowMultiRowSelection="true"
                                                    EnableAJAX="true" AutoGenerateColumns="false">
                                                    <ClientSettings>
                                                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
                                                    </ClientSettings>
                                                    <MasterTableView DataKeyNames="ExtraId" EditMode="EditForms" AllowSorting="true">
                                                        <EditFormSettings ColumnNumber="2">
                                                            <FormTableItemStyle Wrap="False" Width="100%"></FormTableItemStyle>
                                                            <FormMainTableStyle GridLines="Horizontal" CellSpacing="0" CellPadding="3" Width="100%" />
                                                            <FormTableStyle GridLines="None" CellSpacing="0" CellPadding="2" BackColor="white"
                                                                Width="100%" />
                                                            <FormTableAlternatingItemStyle Wrap="False"></FormTableAlternatingItemStyle>
                                                            <EditColumn ButtonType="PushButton" InsertText="Insert Contact" UpdateText="Update record"
                                                                UniqueName="EditCommandColumn1" CancelText="Cancel edit">
                                                            </EditColumn>
                                                            <FormTableButtonRowStyle BackColor="#FFFFE7" HorizontalAlign="Left"></FormTableButtonRowStyle>
                                                        </EditFormSettings>
                                                        <Columns>
                                                            <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" />
                                                            <telerik:GridBoundColumn UniqueName="ExtraId" DataField="ExtraId" HeaderText="ExtraId"
                                                                Display="false" ReadOnly="true" />
                                                            <telerik:GridBoundColumn UniqueName="JobId" DataField="JobId" HeaderText="JobId"
                                                                ReadOnly="true" AllowSorting="true" />
                                                            <telerik:GridTemplateColumn UniqueName="ExtraTypeCol" DataField="ExtraType" HeaderText="Extra Type">
                                                                <ItemTemplate>
                                                                    <div runat="server" id="divExtraType">
                                                                        <%# ((System.Data.DataRowView)Container.DataItem)["ExtraType"].ToString() %>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="customDescription" HeaderText="Description">
                                                                <ItemTemplate>
                                                                    <div runat="server" id="divDescription">
                                                                        <%# ((System.Data.DataRowView)Container.DataItem)["CustomDescription"]%></div>
                                                                </ItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="ExtraStateCol" DataField="ExtraState" HeaderText="Extra State">
                                                                <ItemTemplate>
                                                                    <div runat="server" id="divExtraState">
                                                                        <%# ((System.Data.DataRowView)Container.DataItem)["ExtraState"]%>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <EditItemTemplate>
                                                                    <asp:DropDownList OnPreRender="PopulateExtraStates" runat="server" Value='<%# ((System.Data.DataRowView)Container.DataItem)["ExtraState"].ToString()%>'
                                                                        ID="cboExtraState">
                                                                    </asp:DropDownList>
                                                                    <asp:CustomValidator runat="server" ID="cvExtraState" OnServerValidate="cboExtraState_ServerValidate"
                                                                        ControlToValidate="cboExtraState" ErrorMessage="You must select an extra state."></asp:CustomValidator>
                                                                </EditItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="ClientContact" HeaderText="Client Contact">
                                                                <ItemTemplate>
                                                                    <div runat="server" id="divClientContact">
                                                                        <%# ((System.Data.DataRowView)Container.DataItem)["ClientContact"]%></div>
                                                                </ItemTemplate>
                                                                <EditItemTemplate>
                                                                    <input runat="server" id="txtClientContact" type="text" value='<%#((System.Data.DataRowView)Container.DataItem)["ClientContact"]%>' />
                                                                </EditItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridTemplateColumn UniqueName="Amount" HeaderText="Extra Amount">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblExtraAmount" runat="server" Text=""></asp:Label>
                                                                </ItemTemplate>
                                                                <EditItemTemplate>
                                                                    <telerik:RadNumericTextBox ID="txtAmount" runat="server" Type="Currency" Text='<%#((System.Data.DataRowView)Container.DataItem)["ExtraAmount"].ToString()%>'>
                                                                    </telerik:RadNumericTextBox>
                                                                </EditItemTemplate>
                                                            </telerik:GridTemplateColumn>
                                                            <telerik:GridEditCommandColumn>
                                                                <ItemStyle Width="50px" />
                                                            </telerik:GridEditCommandColumn>
                                                        </Columns>
                                                        <SortExpressions>
                                                            <telerik:GridSortExpression FieldName="JobId" SortOrder="Ascending"></telerik:GridSortExpression>
                                                        </SortExpressions>
                                                    </MasterTableView>
                                                </telerik:RadGrid>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:Panel>
    <!-- END OF THE EXTRAS SECTION -->
    <div class="buttonbar">
        <nfvc:NoFormValButton ID="btnFilter" runat="server" Text="Generate Jobs To Invoice"
            ServerClick="btnFilter_Click"></nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnClear" runat="server" Text="Clear" ServerClick="btnClear_Click">
        </nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnCreateInvoice" runat="server" Visible="false" Text="Create Invoice"
            ServerClick="btnCreateInvoice_Click"></nfvc:NoFormValButton>
    </div>

    <script language="javascript">
        var message = "You have selected ";
        var message1 = " job(s), and the total amount is £";
        var hidSelectedJobs = document.getElementById('<%=hidSelectedJobs.ClientID %>');
        var hidSelectedExtras = document.getElementById('<%=hidSelectedExtras.ClientID %>');
        var hidSelectedExtraJobs = document.getElementById('<%=hidSelectedExtraJobs.ClientID %>');
        var hidJobCount = document.getElementById('<%=hidJobCount.ClientID %>');
        var hidJobTotal = document.getElementById('<%=hidJobTotal.ClientID %>');

        function GetCheckedItems(gridItem, index, checkBoxElement) {
            amount = gridItem.Cells[6].Value;
            jobId = gridItem.Cells[8].Value;
            // Check the current CheckBox that we have just dealt with 
            if (checkBoxElement.checked) {
                // We've selected this item
                hidJobCount.value++;
                hidJobTotal.value = parseFloat(hidJobTotal.value) + parseFloat(amount, 10);
                hidSelectedJobs.value = hidSelectedJobs.value + jobId + ",";
            }
            else {
                // We've deselected this item
                hidJobCount.value--;
                hidJobTotal.value = parseFloat(hidJobTotal.value) - parseFloat(amount);


                if (hidSelectedJobs.value.substr(0, (new String(jobId).length) + 1) == jobId + ",") {
                    if (hidSelectedJobs.value == (jobId + ",")) {
                        hidSelectedJobs.value = "";
                    }
                    else {
                        hidSelectedJobs.value = hidSelectedJobs.value.substr((new String(jobId).length) + 1);
                    }
                }
                else {
                    var location = hidSelectedJobs.value.indexOf("," + jobId + ",");
                    hidSelectedJobs.value = hidSelectedJobs.value.substr(0, location + 1) + hidSelectedJobs.value.substr(location + ("," + jobId + ",").length);
                }
            }
            hidJobTotal.value = Math.round(hidJobTotal.value * 100) / 100;

            var text = message + hidJobCount.value + message1 + hidJobTotal.value;

            setLabelText("lblDetails", text);

            return true;
        }

        function GetCheckedItemsForExtras(gridItem, index, checkBoxElement) {
            var extraId = gridItem.Cells[1].Value;
            var jobId = gridItem.Cells[2].Value;

            // Check the current CheckBox that we have just dealt with 
            if (checkBoxElement.checked) {
                // We've selected this item
                hidSelectedExtras.value = hidSelectedExtras.value + extraId + ",";
                hidSelectedExtraJobs.value = hidSelectedExtraJobs.value + jobId + "|" + extraId + ",";
            }
            else {
                // We've deselected this item
                if (hidSelectedExtras.value.substr(0, (new String(extraId).length) + 1) == extraId + ",") {
                    if (hidSelectedExtras.value == (extraId + ",")) {
                        hidSelectedExtras.value = "";
                    }
                    else {
                        hidSelectedExtras.value = hidSelectedExtras.value.substr((new String(extraId).length) + 1);
                    }
                }
                else {
                    var location = hidSelectedExtras.value.indexOf("," + extraId + ",");
                    hidSelectedExtras.value = hidSelectedExtras.value.substr(0, location + 1) + hidSelectedExtras.value.substr(location + ("," + extraId + ",").length);
                }

                if (hidSelectedExtraJobs.value.substr(0, (new String(hidSelectedExtraJobs).length) + 1) == jobId + "|" + extraId + ",") {
                    if (hidSelectedExtraJobs.value == (jobId + "|" + extraId + ",")) {
                        hidSelectedExtraJobs.value = "";
                    }
                    else {
                        hidSelectedExtraJobs.value = hidSelectedExtraJobs.value.substr((new String(jobId + "|" + extraId).length) + 1);
                    }
                }
                else {
                    var location = hidSelectedExtraJobs.value.indexOf("," + jobId + "|" + extraId + ",");
                    hidSelectedExtraJobs.value = hidSelectedExtraJobs.value.substr(0, location + 1) + hidSelectedExtraJobs.value.substr(location + ("," + jobId + "|" + extraId + ",").length);
                }
            }

            return true;
        }

        //	function GetCheckedItemsForExtras(gridItem, index, checkBoxElement)
        //	{
        //        var extraId = gridItem.Cells[1].Value;
        //        var jobId  = gridItem.Cells[2].Value;
        //        
        //    	hidSelectedExtras.value     = "";
        //    	hidSelectedExtraJobs.value  = "";
        //	 
        //	    if (checkBoxElement.checked)
        //	    {
        //	       hidSelectedExtras.value = hidSelectedExtras.value + extraId + ",";
        //	       hidSelectedExtraJobs.value = hidSelectedExtraJobs.value + jobId + ",";
        //	    }
        //	    
        //        var gridItem1;
        //        var itemIndex = 0;
        //        
        //        while(gridItem1 = dgExtras.Table.GetRow(itemIndex))
        //        {
        //            checked = gridItem1.Cells[0].Value;
        //            extraId = gridItem1.Cells[1].Value;
        //            
        //            
        //            if(checked) // If checked
        //            {
        //	            hidSelectedExtras.value = hidSelectedExtras.value + extraId + ",";
        //            }
        //           
        //           itemIndex++;
        //        }
        //        
        //        return true;
        //	}
        function setLabelText(ID, Text) {
            document.getElementById(ID).innerHTML = Text;
        }
        function showMenu(e) {
            if (event.button == 2)
                alert("right click menu");
        }
        function selectItem(jobId, chargeAmount, el) {
            if (el.checked) {
                // We've selected this item
                hidJobCount.value++;
                hidJobTotal.value = parseFloat(hidJobTotal.value) + parseFloat(chargeAmount, 10);

                if (hidSelectedJobs.value.substr(0, (new String(jobId).length) + 1) == jobId + ",") {
                    if (hidSelectedJobs.value == (jobId + ",")) {
                        hidSelectedJobs.value = "";
                    }
                    else {
                        hidSelectedJobs.value = hidSelectedJobs.value.substr((new String(jobId).length) + 1);
                    }
                }
                else {
                    hidSelectedJobs.value = hidSelectedJobs.value + jobId + ",";
                }
            }
            else {
                // We've deselected this item
                hidJobCount.value--;
                hidJobTotal.value = parseFloat(hidJobTotal.value) - parseFloat(chargeAmount, 10);

                if (hidSelectedJobs.value.substr(0, (new String(jobId).length) + 1) == jobId + ",") {
                    if (hidSelectedJobs.value == (jobId + ",")) {
                        hidSelectedJobs.value = "";
                    }
                    else {
                        hidSelectedJobs.value = hidSelectedJobs.value.substr((new String(jobId).length) + 1);
                    }
                }
                else {
                    var location = hidSelectedJobs.value.indexOf("," + jobId + ",");
                    hidSelectedJobs.value = hidSelectedJobs.value.substr(0, location + 1) + hidSelectedJobs.value.substr(location + ("," + jobId + ",").length);
                }
                //			var str = hidSelectedJobs.value;
                //			str = str.replace(jobId, "");
                //			str = str.replace(",,", ",");
                //			
                //			hidSelectedJobs.value = str;
            }
            hidJobTotal.value = Math.round(hidJobTotal.value * 100) / 100;

            var text = message + hidJobCount.value + message1 + hidJobTotal.value;

            setLabelText("lblDetails", text);
        }
        function SelectAllCheckboxes(spanChk) {

            var oItem = spanChk.children;
            var theBox = (spanChk.type == "checkbox") ? spanChk : spanChk.children.item[0];
            xState = theBox.checked;

            elm = theBox.form.elements;

            for (i = 0; i < elm.length; i++)
                if (elm[i].type == "checkbox" && elm[i].id != theBox.id) {
                if (elm[i].name.indexOf("dvJobs") > -1) {
                    //elm[i].click();
                    if (elm[i].checked != xState)
                        elm[i].click();
                    //elm[i].checked=xState;
                }
            }
        }

        var lastHighlightedRow = "";
        var lastHighlightedRowColour = "";

        function HighlightRow(row) {
            var rowElement;

            if (lastHighlightedRow != "") {
                rowElement = document.getElementById(lastHighlightedRow);
                rowElement.style.backgroundColor = lastHighlightedRowColour;
            }

            rowElement = document.getElementById(row);
            lastHighlightedRow = row;
            lastHighlightedRowColour = rowElement.style.backgroundColor;
            rowElement.style.backgroundColor = 'yellow';
        }
    </script>

</asp:Content>
