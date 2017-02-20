<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Organisation.Organisation_workForClient" Title="All Work For Organisation" Codebehind="workForClient.aspx.cs" %>

<%@ Import namespace="System.Data"%>

<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">

    <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550"
        Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true">
    </cc1:Dialog>


    <script language="javascript" type="text/javascript">
         
        function showExtrasBreakdown(el, data) {
            var displayString = getDisplayString(data);

            //Show the tooltip
            showToolTip(el, "Extras Breakdown", displayString);
        }

        var orders = "";
        var jobs = "";
    
        function nothing(){}
    
        function viewChangeReference()
        {
            if(orders.length > 0 || jobs.length > 0)
            { 
                var clientID = $find("<%=cboClient.ClientID%>");
                var qs = "clientID=" + clientID.get_value();
                <%=dlgChangeReference.ClientID %>_Open(qs);
            }
            else
            {
                alert("No Jobs or Orders are currently selected");
                    return;
            }
        }
    
        function viewOrderProfile(orderID)
        {
            var qs = "oID=" + orderID;
            <%=dlgViewOrder.ClientID %>_Open(qs);
        }
        
        function viewJobDetails(jobID)
        {
            var url = "/job/job.aspx?jobId=" + jobID + getCSID();

            openResizableDialogWithScrollbars(url,'1100','800');
        }
        
        function ChangeOrderList(e, src, orderID)
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
            
            UpdateReferencState()
        }
        
        function ChangeJobList(e, src, jobID)
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
                if(jobs.length > 0)
                    jobs += ",";
                jobs += jobID; 
                gridRow.className= "SelectedRow_Office2007";
            }
            else
            {
                // remove from the list
                jobs = jobs.replace(jobID + ",", "");
                jobs = jobs.replace("," + jobID, "");
                jobs = jobs.replace(jobID, "");
                gridRow.className= "GridRow_Office2007";
            }
            
            UpdateReferencState()
        }
        
        function HandleGridSelection(grid)
        {
            var mtv = grid.MasterTableView;
            masterChkID = mtv.Columns[0].I14[0].Element.childNodes[0];
            
            for (var rowIndex = 0; rowIndex < mtv.Rows.length; rowIndex++)
            {
                try
                {
                    var chkOrderID = mtv.Rows[rowIndex].Control.childNodes[0].childNodes[0];
                    // If the checkbox has been found, and is not selected - tick the checkbox.
                    if (chkOrderID != null)
                    {
                        if(masterChkID.checked && !chkOrderID.checked) { chkOrderID.click(); }
                        else if(!masterChkID.checked && chkOrderID.checked) { chkOrderID.click(); }
                    }

                }
                catch (error)
                {
                }
            }
        }
        
        function UpdateReferencState()
        {
            var btnUpdateReference = document.getElementById("btnUpdateReference");
            
            if(orders.length > 0 || jobs.length > 0)
                btnUpdateReference.disabled = false;
            else
                btnUpdateReference.disabled = true;        
        }

        $(document).ready(function () {
            SetFilterArea();
            FilterOptionsDisplayHide();

            //Re-add the options to the select clients list because they aren't stored server side
            var oldValues =  document.getElementById("<%=hidSelectedClientsValues.ClientID%>").value;
            var oldText =  document.getElementById("<%=hidSelectedClientsText.ClientID%>").value;

            var valuesArray = oldValues.split(',');
            var textArray = oldText.split(',');


            for(var i = 0; i < valuesArray.length; i++)
            {
                //The split creates an empty string at the end of the array
                if(valuesArray[i] !== ""){
                    $('.seSelectedClients')
                        .append($('<option>', { value: valuesArray[i] })
                        .text(textArray[i]));
                }
            }

        });

        function AddClientToSelected() {

            var selectedClient = $find('<%=cboClient.ClientID%>');
            var value = selectedClient._value;
            var text = selectedClient._text;


            if (value != "") {
                var exists = false;
                $('.seSelectedClients option').each(function () {
                    if (this.value == value) {
                        exists = true;
                        return false;
                    }
                });

                if (!exists) {
                    $('.seSelectedClients')
                 .prepend($('<option>', { value: value })
                 .text(text));

                    //store both value and text
                    var oldValues = document.getElementById("<%=hidSelectedClientsValues.ClientID%>").value;
                    var newValues = value + ',' + oldValues;

                    var oldText = document.getElementById("<%=hidSelectedClientsText.ClientID%>").value;
                    var newText = text + ',' + oldText;

                    document.getElementById("<%=hidSelectedClientsValues.ClientID%>").value = newValues;
                    document.getElementById("<%=hidSelectedClientsText.ClientID%>").value = newText;
                }
            }

            return false;

        }

        function RemoveClientToSelected() {

            
            var selectedOption = $('.seSelectedClients option:selected');
            var indexToRemove = selectedOption.index();
            if(selectedOption.val()){
                selectedOption.remove();

                //Update hidden field for values
                var oldValues =  document.getElementById("<%=hidSelectedClientsValues.ClientID%>").value;
                var arrayValues = oldValues.split(',');

                arrayValues.splice(indexToRemove, 1);
                var newValues = arrayValues.join(',');

                document.getElementById("<%=hidSelectedClientsValues.ClientID%>").value = newValues;

                //Update hidden field for text
                var oldText =  document.getElementById("<%=hidSelectedClientsText.ClientID%>").value;
                var arrayText = oldText.split(',');

                arrayText.splice(indexToRemove, 1);
                var newText = arrayText.join(',');

                document.getElementById("<%=hidSelectedClientsText.ClientID%>").value = newText;

            }

            return false;

        }

        function SetFilterArea() {
            var width = $("#overlayedClearFilterBox").width();
            var height = $("#overlayedClearFilterBox").height();
            var position = $("#overlayedClearFilterBox").position();
            $("#overlayedIframe").css("width", width + 10);
            $("#overlayedIframe").css("height", height + 25);
            $("#overlayedIframe").css("top", position.top);
            $("#overlayedIframe").css("left", position.left);
        }

        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });

            SetFilterArea();

            var fr = document.getElementById("overlayedIframe");
            fr.style.display = "block";
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
            var fr = document.getElementById("overlayedIframe");
            fr.style.display = "none";

        }

        function OpenPODWindow(orderID) {
            var podFormType = 2;
            var qs = "ScannedFormTypeId=" + podFormType;
            qs += "&OrderID=" + orderID;

            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }

        function disableRefreshButtonBeforePostBack() {
            if (Page_IsValid) {
                var btnRefresh = $('[id$=btnRefresh]');
                btnRefresh.prop('disabled', true);
                btnRefresh.val('Please wait');
            }

            return Page_IsValid;
        }
        
    </script>
     <script type="text/javascript" >
         $(document).ready(function () {
             $('.ShowPointTooltip').each(function (i, item) {
                 $(item).qtip({
                     style: { name: 'dark',
                         width: { min: 176 }
                     },
                     position: { adjust: { screen: true} },
                     content: {
                         url: $(item).attr('rel'),
                         data: { pointId: $(item).attr('pointid') },
                         method: 'get'
                     }
                 }


                );
             });
         });
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>All work for Organisation</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <cc1:Dialog ID="dlgViewOrder" runat="server" URL="/Groupage/ManageOrder.aspx" Height="900" Width="1200" Mode="Modal" AutoPostBack="false" ReturnValueExpected="false" />
    <cc1:Dialog ID="dlgChangeReference" runat="server" URL="/Organisation/preChangeReference.aspx" Height="700" Width="1050" Mode="Normal" AutoPostBack="false" ReturnValueExpected="false" />
    
    <asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnRefresh">

                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
       <iframe id="overlayedIframe" style="position: absolute; z-index: 95; background:white;"></iframe>
       <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block; padding-bottom:5px;">
        <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td runat="server" id="tdDateOptions" >
                        <table>
                            <tr>
                                <td class="formCellLabel">Date From</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="rdiStartDate" runat="server" ToolTip="The start date for the filter." Width="100px" >
                                    <DateInput runat="server"
                                     DateFormat="dd/MM/yy">
                                     </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate" ValidationGroup="grpRefresh">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please enter a Start Date." alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Date To</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="rdiEndDate" runat="server"  ToolTip="The start date for the filter." Width="100px" >
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate" ValidationGroup="grpRefresh">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please enter an End Date." alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Resource</td>
                                <td class="formCellField">
                                    <asp:RadioButtonList ID="cboSearchAgainstWorker" runat="server" RepeatDirection="horizontal" RepeatColumns="4">
                                        <asp:ListItem Text="In House" Value="HOUSE"></asp:ListItem>
                                        <asp:ListItem Text="Sub Contracted" Value="SUB"></asp:ListItem>
                                        <asp:ListItem Text="Unresourced" Value="NONE"></asp:ListItem>
                                        <asp:ListItem Text="All" Value="ALL" Selected="true"></asp:ListItem>
                                   </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel"><asp:Label ID="lblMode" runat="server"></asp:Label></td>
                                <td class="formCellField">
                                    <telerik:RadComboBox ID="cboClient" runat="server" DataTextField="OrganisationName" DataValueField="IdentityId"  EnableLoadOnDemand="true" 
                                        ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False" 
                                        ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" Width="343px" Height="300px">

                                    </telerik:RadComboBox>
                                                                      
                                    <asp:RequiredFieldValidator ID="rfvCboClient" runat="server" ControlToValidate="cboClient" ValidationGroup="grpRefresh">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please select a Client or Sub-Contractor." alt="" />
                                    </asp:RequiredFieldValidator>


                                </td>
                                <td class="formCellField">
                                    <asp:button runat="server" type="button" ID="btnAddCboClient" name="add" OnClientClick="javascript:AddClientToSelected(); return false;" UseSubmitBehavior="false"  Text="Add"  />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel"><asp:Label ID="lblSelectedClients" runat="server" Text="Selected Clients"></asp:Label></td>

                                <td class="formCellField">
                                    <select ID="seSelectedClients" class="seSelectedClients" name="seSelectedClients" multiple="true" runat="server" style="width:200px">
                                    </select>
                                    <asp:HiddenField ID="hidSelectedClientsValues" runat="server" />
                                    <asp:HiddenField ID="hidSelectedClientsText" runat="server" />
                                </td>
                                <td class="formCellField">
                                    <asp:button runat="server" type="button" ID="btnRemoveSelectedClients"  name="remove" OnClientClick="javascript:RemoveClientToSelected(); return false;" UseSubmitBehavior="false" Text="Remove"  />
                                </td>
                            </tr>
                             <tr>
                                <td class="formCellLabel">Collection Point</td>
                                <td class="formCellField">
                                    <telerik:RadAjaxPanel ID="raxCollectionPoint" runat="server">
                                        <p1:Point runat="server" ID="ucCollectionPoint" CanCreateNewPoint="false" CanClearPoint="true" CanUpdatePoint="false"/>
                                    </telerik:RadAjaxPanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Delivery Point</td>
                                <td class="formCellField">
                                    <telerik:RadAjaxPanel ID="raxDeliveryPoint" runat="server">
                                        <p1:Point runat="server" ID="ucDeliveryPoint" CanCreateNewPoint="false" CanClearPoint="true" CanUpdatePoint="false"/>
                                    </telerik:RadAjaxPanel>
                                </td>
                            </tr>
                       </table>
                    </td>
                </tr>
            </table>
            
        </fieldset>
        <div class="buttonBar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" OnClientClick="if (!disableRefreshButtonBeforePostBack()) {return false;} else {__doPostBack('ctl00$ContentPlaceHolder1$btnRefresh','');}" />
            </div>
        </div>
        <div class="buttonBar">
            <input type="button" id="btnUpdateReference" value="Update Reference" causesvalidation="false" disabled="disabled" onclick="javascript:viewChangeReference();" />
        </div>
        
    </asp:Panel>
    
    <table id="tblSummary" runat="server" style="display:none" cellpadding="0" cellspacing="0"> 
        <tbody>
            <tr>
                <td>
                    <h3><asp:Label ID="lblSummary" runat="server" Text="Summary" /></h3>
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid ID="grdSummary" runat="server" AllowPaging="false" ShowGroupPanel="true" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
                        <MasterTableView DataKeyNames="Client" Width="100%" AllowSorting="true">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Client" DataField="Client" HeaderStyle-Width="200"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Business Type" DataField="Job Type" HeaderStyle-Width="100"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Delivery Run Count" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <%# ((int)((DataRowView)Container.DataItem)["RunCount"]).ToString() %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalCountOfDeliveryRuns" runat="server"></asp:Label>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="right" />
                                    <FooterStyle HorizontalAlign="right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Order Count" HeaderStyle-Width="80">
                                    <ItemTemplate>
                                        <%# ((int)((DataRowView)Container.DataItem)["CountOfJobs"]).ToString() %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalCountOfJobs" runat="server"></asp:Label>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="right" />
                                    <FooterStyle HorizontalAlign="right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Charge" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <%# ((decimal)((DataRowView)Container.DataItem)["Total Charge Amount"]).ToString("C") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalRate" runat="server"></asp:Label>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="right" />
                                    <FooterStyle HorizontalAlign="right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Extra Charges" HeaderStyle-Width="155">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnTotalExtraCharges" runat="server" onMouseOut="closeToolTip();"></span>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        &nbsp;<span id="spnTotalExtrasBreakdown" runat="server" onMouseOut="closeToolTip();"></span>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="right" />
                                    <FooterStyle HorizontalAlign="right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Fuel Surcharges" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <%# ((decimal)((DataRowView)Container.DataItem)["TotalFuelSurcharge"]).ToString("C") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalOfFuelSurchargeAmount" runat="server"></asp:Label>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="right" />
                                    <FooterStyle HorizontalAlign="right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Weight" HeaderStyle-Width="100" >
                                    <ItemTemplate>
                                        <%# ((decimal)((DataRowView)Container.DataItem)["TotalWeight"]).ToString("F0") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalWeight" runat="server"></asp:Label>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="right" />
                                    <FooterStyle HorizontalAlign="right" />
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </tbody>
    </table>
    
    <table id="tblNormal" runat="server" style="width:100%; display:none" cellpadding="0" cellspacing="0"> 
        <tbody>
            <tr>
                <td>
                    <h3><asp:Label ID="lblNormalTitle" runat="server" Text="Normal"/></h3>
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid ID="grdNormal" runat="server" AllowPaging="false" ShowGroupPanel="true" allowSorting="false" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
                       <MasterTableView width="100%" DataKeyNames="JobID" AllowSorting="true">
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="chkReference">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkHeader" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkRow" runat="server" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridHyperLinkColumn HeaderText="Run ID" SortExpression="JobID" DataNavigateUrlFormatString="javascript:viewJobDetails({0})" DataNavigateUrlFields="JobID" DataTextField="JobID" HeaderStyle-Width="40"></telerik:GridHyperLinkColumn>
                                <telerik:GridBoundColumn HeaderText="Load No" DataField="CustomerOrderNumber" HeaderStyle-Width="150"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Job Charge" HeaderStyle-Width="75">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnCharge" runat="server"><%#((DataRowView)Container.DataItem)["Rate"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((DataRowView)Container.DataItem)["Rate"]).ToString("C")%></span>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalJobCharge" runat="server" />
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Extra Charges" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnExtraCharges" runat="server" onMouseOut="closeToolTip();"></span>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        &nbsp;<span id="spnExtrasBreakdown" runat="server" onMouseOut="closeToolTip();"></span>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Docket Number" DataField="DeliveryOrderNumber" HeaderStyle-Width="150"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Collect From" SortExpression="CollectionPointDescription">
                                <ItemTemplate>
                                    <span id="spnCollectionPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
                                </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                                    <ItemTemplate>
                                        <span id="Span1" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="PostTown" DataField="PostTown" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Delivery" HeaderStyle-Width="115">
                                    <ItemTemplate>
                                        <span id="spnDelivery" onclick=""><%#((bool)((DataRowView)Container.DataItem)["DeliveryIsAnytime"]) ? ((DateTime)((DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy") : ((DateTime)((DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy HH:mm")%></span>                                    
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="No Pallets" HeaderStyle-Width="50">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnNoPallets" runat="server"><%#((DataRowView)Container.DataItem)["NoPallets"].ToString().Length < 1 ? "&nbsp;" : ((int)((DataRowView)Container.DataItem)["NoPallets"]).ToString()%></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                
                                <telerik:GridBoundColumn HeaderText="Driver" DataField="Driver" HeaderStyle-Width="125"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Trailer" DataField="Trailer" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Has POD" HeaderStyle-Width="50">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="lnkPOD" runat="server" NavigateUrl="" ForeColor="black" Target="_blank"></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="JobStateID" Visible="false" UniqueName="JobStateID"></telerik:GridBoundColumn>
                            </Columns>
                       </MasterTableView>
                       <ClientSettings>
                            <ClientEvents OnRowSelected="nothing" />
                       </ClientSettings>
                    </telerik:RadGrid>
                </td>
            </tr>
        </tbody>
    </table>
    
    <asp:Repeater ID="repBusinessType" runat="server" Visible="true">
        <HeaderTemplate>
            <table style="width:100%" cellpadding="0" cellspacing="0"> 
                <tbody>    
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <h3><asp:Label ID="lblTitle" runat="server" Text="" /></h3>
                    <asp:HiddenField ID="hidBusinessTypeID" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid ID="grdBusinessType" runat="server" AllowPaging="false" ShowGroupPanel="true" allowSorting="false" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" OnNeedDataSource="grd_NeedDataSource" OnItemDataBound="grd_ItemDataBound" ShowFooter="true">
                       <MasterTableView width="100%" DataKeyNames="OrderID" AllowSorting="true">
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="chkReference">
                                    <HeaderTemplate>
                                        <asp:CheckBox ID="chkHeader" runat="server" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkRow" runat="server" />
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Run ID" SortExpression="JobID" HeaderStyle-Width="40">
                                    <ItemTemplate>
                                        <a href="javascript:viewJobDetails(<%#((DataRowView)Container.DataItem)["JobID"].ToString() %>)"><%#((DataRowView)Container.DataItem)["JobID"].ToString() == string.Empty ? "&nbsp;" : ((DataRowView)Container.DataItem)["JobID"].ToString()%></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridHyperLinkColumn HeaderText="Order ID" SortExpression="OrderID" DataNavigateUrlFormatString="javascript:viewOrderProfile({0})" DataNavigateUrlFields="OrderID" DataTextField="OrderID" HeaderStyle-Width="50"></telerik:GridHyperLinkColumn>
                                <telerik:GridBoundColumn HeaderText="Client" DataField="Client" HeaderStyle-Width="200"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Customer Order Number" DataField="CustomerOrderNumber" HeaderStyle-Width="135"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Order Charge" HeaderStyle-Width="75">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnCharge" runat="server"><%#((DataRowView)Container.DataItem)["Rate"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((DataRowView)Container.DataItem)["Rate"]).ToString("C")%></span>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalOrderCharge" runat="server" />
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Extra Charges" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnExtraCharges" runat="server" onMouseOut="closeToolTip();"></span>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        &nbsp;<span id="spnExtrasBreakdown" runat="server" onMouseOut="closeToolTip();"></span>
                                    </FooterTemplate>
                                    <ItemStyle HorizontalAlign="Right" />
                                    <FooterStyle HorizontalAlign="Right" />
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Collect From" SortExpression="CollectionPointDescription">
                                <ItemTemplate>
                                    <span id="spnCollectionPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
                                </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Delivery Order Number" DataField="DeliveryOrderNumber" HeaderStyle-Width="135"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                                    <ItemTemplate>
                                         <span id="Span1" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="PostTown" DataField="PostTown" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Delivery" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <span id="spnDelivery" onclick=""><%#((bool)((DataRowView)Container.DataItem)["DeliveryIsAnytime"]) ? ((DateTime)((DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy") : ((DateTime)((DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy HH:mm")%></span>                                    
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="No Pallets" HeaderStyle-Width="50">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnNoPallets" runat="server"><%#((DataRowView)Container.DataItem)["NoPallets"].ToString().Length < 1 ? "&nbsp;" : ((int)((DataRowView)Container.DataItem)["NoPallets"]).ToString()%></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Pallet Spaces" HeaderStyle-Width="50">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnPalletSpaces" runat="server"><%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["PalletSpaces"].ToString()).ToString("0.##") %></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Weight" DataField="Weight" HeaderStyle-Width="50" DataFormatString="{0:F0}" ></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Driver" DataField="Driver" HeaderStyle-Width="125"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Trailer" DataField="Trailer" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Has POD" HeaderStyle-Width="50">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="lnkPOD" runat="server" NavigateUrl="" ForeColor="black" Target="_blank"><%#((bool)((System.Data.DataRowView)Container.DataItem)["HasPOD"]) ? "Yes" : "No"%></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="JobStateID" Visible="false" UniqueName="JobStateID"></telerik:GridBoundColumn>
                            </Columns>
                       </MasterTableView>
                       <ClientSettings>
                            <ClientEvents OnRowSelected="nothing" />
                       </ClientSettings>
                    </telerik:RadGrid>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>
      <script language="javascript" type="text/javascript">
          FilterOptionsDisplayHide();
</script>
</asp:Content>