<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Report.GetAllForClientAndLoad" Codebehind="GetAllForClientAndLoad.aspx.cs" MasterPageFile="~/default_tableless.Master" Title="All Runs for Client and Load Date" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>All Runs for Client and Load Date</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
    <!--
        var table  = null;
        
        $(document).ready(function() {
            table = $get('orders');
        });
    
        function openJobDetailsWindow(jobId)
        {
            openDialogWithScrollbars('../Job/Job.aspx?wiz=true&jobId=' + jobId,'1220','800');
        }

        function openJobDetails(jobID) 
        {
            var vs = "wiz=true&jobId=" + jobID;

            if (sessionStorage && sessionStorage.sessionID) {
                vs += '&csid=' + sessionStorage.sessionID;
            }

            <%=dlgJobDetails.ClientID %>_Open(vs);
        }

        function openOrder(orderID) 
        {
            var vs = "wiz=true&oID=" + orderID;
            <%=dlgOrder.ClientID %>_Open(vs);
        }
        
        function toggleGroup(img, numberOfRows) {
            //  get a reference to the row and table
            var tr = img.parentNode.parentNode;
            var src = img.src;

            //  do some simple math to determine how many
            //  rows we need to hide/show
            var startIndex = tr.rowIndex + 1;
            var stopIndex = startIndex + parseInt(numberOfRows);

            //  if the img src ends with plus, then we are expanding the
            //  rows.  go ahead and remove the hidden class from the rows
            //  and update the image src
            if (src.endsWith('topItem_exp.gif')) {
                for (var i = startIndex; i < stopIndex; i++) {
                    Sys.UI.DomElement.removeCssClass(table.rows[i], 'hidden');
                }

                src = src.replace('topItem_exp.gif', 'topItem_col.gif');
            }
            else {
                for (var i = startIndex; i < stopIndex; i++) {
                    Sys.UI.DomElement.addCssClass(table.rows[i], 'hidden');
                }

                src = src.replace('topItem_col.gif', 'topItem_exp.gif');
            }

            //  update the src with the new value
            img.src = src;
        }
    //-->

        // Function to show the filter options overlay box
    function FilterOptionsDisplayShow() {
        $("#overlayedClearFilterBox").css({ 'display': 'block' });
        $("#filterOptionsDiv").css({ 'display': 'none' });
        $("#filterOptionsDivHide").css({ 'display': 'block' });
    }

    function FilterOptionsDisplayHide() {
        $("#overlayedClearFilterBox").css({ 'display': 'none' });
        $("#filterOptionsDivHide").css({ 'display': 'none' });
        $("#filterOptionsDiv").css({ 'display': 'block' });
    }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <cc1:Dialog ID="dlgJobDetails" runat="server" URL="/Job/Job.aspx" Mode="Normal" AutoPostBack="false" UseCookieSessionID="true"/>
    <cc1:Dialog ID="dlgOrder" runat="server" URL="/Groupage/ManageOrder.aspx" Mode="Normal" Width="1300"  Height="900" AutoPostBack="false" />

    <h2>Enter your criteria below</h2>
                <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <asp:button id="btnExport" runat="server" text="Export to CSV" />
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr>
                <td class="formCellLabel">Client</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" 
                            MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" Height="300px"
                            ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" AllowCustomText="false" Overlay="true" ZIndex="99">
                    </telerik:RadComboBox>
                 </td>
            </tr>
            <tr>
                <td class="formCellLabel">Start Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100" ToolTip="The earliest date to report on.">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="rdiStartDate" Display="Dynamic" ErrorMessage="Please supply a start date.">
                        <img src="/images/error.gif" width="16" height="16" alt="Please supply a start date." />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">End Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100" ToolTip="The earliest date to report on.">
                    <DateInput runat="server" 
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator id="rfvSEndDate" runat="server" ControlToValidate="rdiEndDate" Display="Dynamic" ErrorMessage="Please supply an end date.">
                        <img src="/images/error.gif" width="16" height="16" alt="Please supply an end date." />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Use Planned Times</td>
                <td class="formCellField">
                    <asp:CheckBox ID="chkUserPlannedTimes" runat="server" Checked="true" />
                </td>
            </tr>
        </table>
    </fieldset>
        <div class="buttonbar">
        <asp:button id="btnGetJobs" runat="server" text="Get Jobs" />
    </div>
    </div>

    
    <asp:panel id="pnlGrid" runat="server" visible="false">
        <ComponentArt:Grid id="dgJobs" 
                RunningMode="Client" 
                EnableViewState="false"
                CssClass="Grid" 
                HeaderCssClass="GridHeader" 
                FooterCssClass="GridFooter"
                GroupByTextCssClass="GroupByText"
                GroupingNotificationTextCssClass="GridHeaderText"
                GroupBySortAscendingImageUrl="group_asc.gif"
                GroupBySortDescendingImageUrl="group_desc.gif"
                GroupBySortImageWidth="10"
                GroupBySortImageHeight="10"
                GroupingPageSize = "25"
                ShowHeader="true"
                ShowSearchBox="True"
                AllowTextSelection="True"
                SearchOnKeyPress="True"
                PageSize="1000" 
                ShowFooter="false"
                ScrollBar ="auto"
                ScrollTopBottomImagesEnabled="true"
                ScrollImagesFolderUrl="~/images/scroller"
                ScrollBarCssClass="ScrollBar"
                ScrollGripCssClass="ScrollGrip"
                ScrollPopupClientTemplateId="ScrollPopupTemplate"
                PagerTextCssClass="GridFooterText"
                PagerButtonWidth="41"
                PagerButtonHeight="22"
                SliderHeight="20"
                PreExpandOnGroup="false"
                SliderWidth="150" 
                SliderGripWidth="9" 
                SliderPopupOffsetX="20"
                ImagesBaseUrl="~/images/" 
                PagerImagesFolderUrl="~/images/pager/"
                TreeLineImagesFolderUrl="~/images/lines/" 
                TreeLineImageWidth="22" 
                TreeLineImageHeight="19" 
                FillContainer="true"
                Width="100%" runat="server"
                KeyboardEnabled ="true">

                <Levels>
                    <ComponentArt:GridLevel 
                         DataKeyField="JobId"
                         HeadingCellCssClass="HeadingCell" 
                         HeadingRowCssClass="HeadingRow" 
                         HeadingTextCssClass="HeadingCellText"
                         DataCellCssClass="DataCell" 
                         RowCssClass="Row" 
                         SelectedRowCssClass="SelectedRow"
                         SortAscendingImageUrl="asc.gif" 
                         SortDescendingImageUrl="desc.gif" 
                         SortImageWidth="10"
                         SortImageHeight="10"
                         GroupHeadingCssClass="GroupHeading" >
                         <Columns>
                            <ComponentArt:GridColumn DataField="Client" HeadingText="Client" IsSearchable="true" visible="false"/>
                            <ComponentArt:GridColumn DataField="JobId" HeadingText="ID" FixedWidth="True" IsSearchable="true" DataCellClientTemplateId="JobTemplate" Width="40" />
                            <ComponentArt:GridColumn DataField="LoadNo" HeadingText="Load No" IsSearchable="true" />
                            <ComponentArt:GridColumn DataField="ChargeAmount" HeadingText="Rate" />
                            <ComponentArt:GridColumn DataField="DocketNumber" HeadingText="DocketNo" />
                            <ComponentArt:GridColumn DataField="OrganisationName" HeadingText="Drop Customer" formatstring="dd/MM/yy HH:mm" Width="100" />
                            <ComponentArt:GridColumn DataField="Description" HeadingText="Drop Point" />
                            <ComponentArt:GridColumn DataField="PostTown" HeadingText="Drop Town" />
                            <ComponentArt:GridColumn DataField="Cases Despatched" HeadingText="Despatched"/>
                            <ComponentArt:GridColumn DataField="Cases Delivered" HeadingText="Delivered"/>                                
                            <ComponentArt:GridColumn DataField="Shorts" HeadingText="Shorts"/>
                            <ComponentArt:GridColumn DataField="Weight" HeadingText="Weight"/>
                            <ComponentArt:GridColumn DataField="Pallets Despatched" HeadingText="Pallets Desp"/>
                            <ComponentArt:GridColumn DataField="Pallets Delivered" HeadingText="Pallets Del"/>
                            <ComponentArt:GridColumn DataField="Pallets Returned" HeadingText="Pallets Ret"/>
                            <ComponentArt:GridColumn DataField="QuantityRefused" HeadingText="Quantity Refused"/>
                            <ComponentArt:GridColumn DataField="ProductCode" HeadingText="Product Code"/>
                            <ComponentArt:GridColumn DataField="RefusalType" HeadingText="Refusal Type"/>
                            <ComponentArt:GridColumn DataField="RefusalNotes" HeadingText="Refusal Notes"/>
                            <ComponentArt:GridColumn DataField="Planned Arrival Date & Time" HeadingText="Planned"/>
                            <ComponentArt:GridColumn DataField="Actual Arrival Date & Time" HeadingText="Actual"/>
                            <ComponentArt:GridColumn DataField="Departure Date & Time" HeadingText="Departed"/>
                            <ComponentArt:GridColumn DataField="Demurrage (2 Hour)" HeadingText="Demurrage (2 Hours)"/>
                         </Columns>
                           
                    </ComponentArt:GridLevel>
                </Levels>
              
                <ClientTemplates>
                    <ComponentArt:ClientTemplate ID="JobTemplate">
                        <a href='javascript:openJobDetailsWindow(## DataItem.GetMember("JobId").Value ##);' title="Open this job">## DataItem.GetMember("JobId").Value ##</a>
                    </ComponentArt:ClientTemplate>
                           
                    <ComponentArt:ClientTemplate Id="LoadingFeedbackTemplate">
                        <table cellspacing="0" cellpadding="0" border="0">
                            <tr>
                                <td style="font-size:10px;font-family:Verdana;">Loading...&nbsp;</td>
                                <td><img src="../../images/spinner.gif" width="16" height="16" border="0"></td>
                            </tr>
                        </table>
                    </ComponentArt:ClientTemplate>
                </ClientTemplates>
            </ComponentArt:grid>
    </asp:panel>
    
    <asp:ListView ID="lvReturnedOrders" runat="server" EnableViewState="false">
        <LayoutTemplate>
            <div class="listViewGrid">
                <table id="orders" cellpadding="0" cellspacing="0" style="width:95%;">
                    <thead>
                        <tr class="HeadingRow">
                            <th class="first"></th>
                            <th>RunID</th>
                            <th>LoadNo</th>
                            <th>Rate</th>
                            <th>Docket No</th>
                            <th>Drop Customer</th>
                            <th>Despatched</th>
                            <th>Delivered</th>
                            <th>Shorts</th>
                            <th>Weight</th>
                            <th>Pallets Desp</th>
                            <th>Pallets Del</th>
                            <th>Pallets Ret</th>
                            <th>Voucher No</th>
                            <th>No Of Pallets</th>
                            <th>Quantity Refused</th>
                            <th>Product Code</th>
                            <th>Refusal Type</th>
                            <th>Refusal Notes</th>
                            <th>Planned</th>
                            <th>Actual</th>
                            <th>Departed</th>
                            <th>Demurrage (2 Hours)</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr id="itemPlaceHolder" runat="server" />
                    </tbody>
                </table>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="row" runat="server" class="group">
                <th class="first">
                    <img src="/images/topItem_exp.gif" alt='Group: <%# Eval("Destination.Client")%>-<%# Eval("Destination.Description")%>' onclick="toggleGroup(this, '<%# Eval("Orders") %>');" />
                </th>
                <th colspan="20"><%# Eval("Destination.Client")%> : <%# Eval("Destination.Description")%></th>
                <th colspan="2">(<%# Eval("Orders")%>Orders)</th>
            </tr>
            <asp:ListView ID="lvItems" runat="server" DataSource='<%# Eval("Items") %>' >
                <LayoutTemplate>
                    <tr runat="server" id="itemPlaceHolder" />
                </LayoutTemplate>
                <ItemTemplate>
                    <tr id="row" runat="server" class="Row hidden">
                        <td class="first"></td>
                        <td><a id="jobRef" href='javascript:openJobDetails(<%# ((System.Data.DataRow)Container.DataItem)["JobID"].ToString()%>);' > <%# ((System.Data.DataRow)Container.DataItem)["JobID"].ToString()%></a></td>
                        <td><%# ((System.Data.DataRow)Container.DataItem)["LoadNo"].ToString()%></td>
                        <td><%# ((decimal)((System.Data.DataRow)Container.DataItem)["Rate"]).ToString("C")%></td>
                        <td><a id="orderRef" href='javascript:openOrder(<%# ((System.Data.DataRow)Container.DataItem)["OrderId"].ToString()%>);'> <%# ((System.Data.DataRow)Container.DataItem)["Docket No"].ToString() %></a></td>
                        <td><%# ((System.Data.DataRow)Container.DataItem)["OrganisationName"].ToString()%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Cases Despatched"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["Cases Despatched"].ToString()) %></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Cases Delivered"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["Cases Delivered"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Shorts"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["Shorts"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Weight"] == DBNull.Value ? "&nbsp;" : ((decimal)((System.Data.DataRow)Container.DataItem)["Weight"]).ToString("F2"))%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Pallets Despatched"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["Pallets Despatched"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Pallets Delivered"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["Pallets Delivered"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Pallets Returned"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["Pallets Returned"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["VoucherNo"] != DBNull.Value ? ((bool)((System.Data.DataRow)Container.DataItem)["IsUploaded"]) != false ? "<a id=\"pcvRef\" href=\"" + ((System.Data.DataRow)Container.DataItem)["ScannedFormPDF"].ToString() + "\">" + ((System.Data.DataRow)Container.DataItem)["VoucherNo"].ToString() + "</a>" : ((System.Data.DataRow)Container.DataItem)["VoucherNo"].ToString() : "&nbsp;" )%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["NoOfPallets"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["NoOfPallets"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["QuantityRefused"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["QuantityRefused"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["ProductCode"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["ProductCode"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["RefusalType"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["RefusalType"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["RefusalNotes"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["RefusalNotes"].ToString())%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Planned Arrival Date & Time"] == DBNull.Value ? "&nbsp;" : ((DateTime)((System.Data.DataRow)Container.DataItem)["Planned Arrival Date & Time"]).ToString("dd/MM/yy"))%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Actual Arrival Date & Time"] == DBNull.Value ? "&nbsp;" : ((DateTime)((System.Data.DataRow)Container.DataItem)["Actual Arrival Date & Time"]).ToString("dd/MM/yy"))%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Departure Date & Time"] == DBNull.Value ? "&nbsp;" : ((DateTime)((System.Data.DataRow)Container.DataItem)["Departure Date & Time"]).ToString("dd/MM/yy"))%></td>
                        <td><%# string.Format("{0}", ((System.Data.DataRow)Container.DataItem)["Demurrage (2 Hour)"] == DBNull.Value ? "&nbsp;" : ((System.Data.DataRow)Container.DataItem)["Demurrage (2 Hour)"].ToString())%></td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </ItemTemplate>
        <EmptyDataTemplate>
            <div>
                There are no records to display
            </div>
        </EmptyDataTemplate>
    </asp:ListView>
        <script language="javascript" type="text/javascript">
            FilterOptionsDisplayHide();
</script>    
</asp:Content>