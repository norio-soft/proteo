<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="listShortsAndOvers.aspx.cs" Inherits="Orchestrator.WebUI.GoodsRefused.listShortsAndOvers" MasterPageFile="~/default_tableless.master" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.fixedheader.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js" ></script>
    <script type="text/javascript" language="javascript" src="listShortsAndOvers.aspx.js"></script>

    <style type="text/css">
        h3
        {
            clear: left;
            font-size: 12px;
            font-weight: normal;
            padding: 0 0 1em;
            margin: 0;
        }
        
        /*.masterpage_layout {width: 1700px;}*/.RadGrid_Orchestrator *
        {
            font-family: Verdana !important;
            font-size: 10px !important;
        }
        
        .overlayedDataBox
        {
            width: 330px !important;
        }
    </style>

    <script type="text/javascript">
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
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>List Shorts and Overs</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <cc1:Dialog ID="dlgRun" runat="server" Mode="Modal" Resizable="false" URL="/Job/Job.aspx" Width="1200" Height="900" UseCookieSessionID="true"/>
    <cc1:Dialog ID="dlgOrder" URL="/Groupage/ManageOrder.aspx" Height="1400" Width="1000" AutoPostBack="true" runat="server" Mode="Modal" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgRefusal" ReturnValueExpected="true" AutoPostBack="true" runat="server" Mode="Modal" Resizable="false" URL="Addupdategoodsrefused.aspx" Width="900" Height="760" />

    <table width="100%">
        <tr valign="top">
            <td valign="top">
                            <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <asp:Button ID="btnSave" runat="server" Text="Save" Width="75px" />
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
                <fieldset>
                    <legend>Filter Options</legend>
                    <table>
                        <tr>
                            <td class="formCellLabel"><asp:label id="lblClient" runat="server">Client</asp:label></td>
                            <td class="formCellField" colspan="3">
                                <telerik:RadComboBox ID="cboClient"  runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    AutoPostBack="true" MarkFirstMatch="true" Height="300px" Overlay="true" CausesValidation="False"
                                    ShowMoreResultsBox="false" Width="350px" AllowCustomText="True" OnClientItemsRequesting="cboClient_itemsRequesting" >
                                    <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                                </telerik:RadComboBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Collection Point</td>
                            <td>
                            <p1:Point runat="server" ID="ucCollectionPoint" ShowFullAddress="true" CanClearPoint="true"
                                CanUpdatePoint="false" ShowPointOwner="true" Visible="true" IsDepotVisible="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Date From</td>
                            <td class="formCellField">
                                <telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100">
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                        </tr>    
                        <tr>
                            <td class="formCellLabel">Date To</td>
                            <td class="formCellField">
                                <telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100">
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Show Shorts and Overs already checked?</td>
                            <td class="formCellField">
                                <asp:CheckBox ID="chkAlreadyChecked" runat="server" Checked="false" />
                            </td>
                        </tr>
                    </table>
                </fieldset>
                    <div class="buttonbar" style="margin-bottom:5px;">
	    <asp:Button ID="btnSearch" runat="server" Text="Search" Width="75px" />
        
    </div>
                </div>
            </td>
        </tr>
    </table>



    <div>
        <telerik:RadGrid runat="server" ID="grdRefusals" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false">
            <MasterTableView Width="100%" ClientDataKeyNames="RefusalId" DataKeyNames="RefusalId">
                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                <Columns>
                    <telerik:GridTemplateColumn HeaderStyle-Width="30" HeaderText="">
                        <HeaderTemplate>
                            <input type="checkbox" onclick="javascript:HandleSelectAll(this);" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelectShort" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn headertext="Goods ID" HeaderStyle-Width="75">
                        <itemtemplate>
                            <a href="javascript:OpenGoodsRefusedForEdit(<%# DataBinder.Eval(Container.DataItem, "InstructionId") %>,<%# DataBinder.Eval(Container.DataItem, "RefusalId") %>)"><%# DataBinder.Eval(Container.DataItem, "RefusalId") %></a>
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn datafield="OrganisationName" headertext="Client" />
                    <telerik:GridBoundColumn datafield="CollectionPoint" headertext="CollectionPoint" />
                    <telerik:GridBoundColumn datafield="ProductName" headertext="Name" />
                    <telerik:GridBoundColumn datafield="LoadNo" headertext="Load No" />
                    <telerik:GridBoundColumn datafield="DocketNumber" headertext="Docket No" />
                    <telerik:GridBoundColumn datafield="RefusalType" headertext="Type" />
                    <telerik:GridTemplateColumn HeaderText="Order">
                        <ItemTemplate>
                            <a runat="server" id="hypOriginalOrder"></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn headertext="Run Id">
                        <itemtemplate>
                            <a href="javascript:OpenRunDetails(<%# DataBinder.Eval(Container.DataItem, "JobId") %>);"><%# DataBinder.Eval(Container.DataItem, "JobId") %></a>
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn datafield="CollectDropDateTime" headertext="Date Recorded" dataformatstring="{0:dd/MM/yy}" />
                </Columns>
            </MasterTableView>
            <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                <Resizing AllowColumnResize="true" AllowRowResize="false" />
            </ClientSettings>
        </telerik:RadGrid>
    </div>
	
	<div class="buttonbar" align="left">
		<asp:Button ID="btnSearchBottom" runat="server" Text="Search" Width="75px" />
        <asp:Button ID="btnSaveBottom" runat="server" Text="Save" Width="75px" />
	</div>

    <script language="javascript" type="text/javascript">
        function cboClient_itemsRequesting(sender, eventArgs)
        {
            try
            {
                var context = eventArgs.get_context();
                context["DisplaySuspended"] = false;
            }
            catch (err) { }
        }

        function OpenGoodsRefusedForEdit(instructionId, refusalId)
        {
	        var qs = "InstructionId=" + instructionId + "&RefusalId=" + refusalId + "&isWindowed=1" + "&showRP=1";
	        <%=dlgRefusal.ClientID %>_Open(qs);	        
        }

        function OpenRunDetails(runID) 
        {
            var qs = "jobId=" + runID + "&wiz=true";
	        <%=dlgRun.ClientID %>_Open(qs);	            
        }
        FilterOptionsDisplayHide();
    </script>

</asp:Content>
