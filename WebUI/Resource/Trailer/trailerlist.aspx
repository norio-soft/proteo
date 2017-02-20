<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Trailer.TrailerList" Codebehind="TrailerList.aspx.cs" MasterPageFile="~/default_tableless.Master"   Title="Trailer List" %>

<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
    
<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Trailer List</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
</asp:Content>
    
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<h2>Please choose a trailer from the list below.</h2>		
   	<asp:Label id="lblNote" Runat="server" Text=""></asp:Label>
   	<fieldset runat="server" id="fsDateFilter" visible="false">
   	    <legend>Filter Options</legend>
   	    <table>
		    <tr>
			    <td class="formCellLabel">Start Date</td>
			    <td class="formCellField"><Telerik:RadDatePicker id="dteStartDate" runat="server" Width="100"></Telerik:RadDatePicker></td>
			    <td class="formCellField">
				    <asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify the start date." Display="Dynamic"><img src="../../images/newMasterPage/icon-warning.png" height="16" width="16" title="Please specify the start date."></asp:RequiredFieldValidator>
				    <asp:CustomValidator id="cfvStartDate" runat="server" EnableClientScript="False" ControlToValidate="dteStartDate" ErrorMessage="The start date must occur before the end date." Display="Dynamic"><img src="../../images/Error.gif" height="16" width="16" title="The start date must occur before the end date."></asp:CustomValidator>
			    </td>
			    <td class="formCellLabel">End Date</td>
			    <td class="formCellField"><Telerik:RadDatePicker id="dteEndDate" runat="server" Width="100"></Telerik:RadDatePicker></td>
			    <td class="formCellField">
				    <asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify the end date." Display="Dynamic"><img src="../../images/newMasterPage/icon-warning.png" height="16" width="16" title="Please specify the end date."></asp:RequiredFieldValidator>
			    </td>
		    </tr>
	    </table>
	    </fieldset>
	    <div class="buttonbar" style="display:<%=fsDateFilter.Visible == true ? "" : "none" %>"> 
			<asp:Button id="btnFilter" runat="server" Text="Filter" width="75"></asp:Button>
		</div>
	<asp:panel id="pnlTrailer" runat="server" visible="false">
	
	<cc1:Dialog ID="dlgAddUpdateTrailer" URL="addupdatetrailer.aspx" Width="580" Height="532"
        AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="false" Scrollbars="false">
    </cc1:Dialog>
        <div class="overlayedFilterBox" id="overlayedClearFilterBox">
        <fieldset>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Trailer Ref
                    </td>
                    <td class="formCellField">
                        <asp:TextBox runat="server" id="txtFilterTrailerRef"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="75" />
            <input type="button" id="Button2" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide()" />
        </div>
    </div>
<telerik:RadGrid PageSize="100" runat="server" ID="grdTrailers" AutoGenerateColumns="false" AllowSorting="true" EnableAJAX="true" Skin="Orchestrator" EnableViewState="false" 
    AllowPaging="False">
    <MasterTableView DataKeyNames="ResourceId" AutoGenerateColumns="False" CommandItemDisplay="Top"
            GroupLoadMode="Client" CommandItemStyle-BackColor="White">
        <RowIndicatorColumn>
            <HeaderStyle Width="20px"></HeaderStyle>
        </RowIndicatorColumn>
        <ExpandCollapseColumn>
            <HeaderStyle Width="20px"></HeaderStyle>
        </ExpandCollapseColumn>
        <CommandItemTemplate>
            <input runat="server" type="button" class="buttonClassSmall" value="Add Trailer" id="btnAddNewTrailer" />
            <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()">
                Show filter Options</div>
            <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()"
                style="display: none;">
                Close filter Options</div>
            <asp:Label ID="lblQF" runat="server" Text="Quick Filter:" />
            <div id="grdFilterHolder">
            </div>
        </CommandItemTemplate>
        <Columns>
            <telerik:GridTemplateColumn ItemStyle-Wrap="false" UniqueName="TrailerRef" HeaderText="Trailer Ref">
                <ItemTemplate>
                    <a runat="server" id="hypAddUpdateTrailer"></a>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn UniqueName="Manufacturer" DataField="Manufacturer" HeaderText="Manufacturer" />
            <telerik:GridBoundColumn UniqueName="TrailerType" DataField="TrailerType" HeaderText="Trailer Type" />
            <telerik:GridBoundColumn UniqueName="TrailerDescription" DataField="TrailerDescription" HeaderText="Trailer Description" />
            <telerik:GridBoundColumn Display="true" UniqueName="ThirdPartyIntegrationID" DataField="ThirdPartyIntegrationID" HeaderText="Third Party Integration ID" />
            <telerik:GridBoundColumn UniqueName="GPSUnitID" DataField="GPSUnitID" HeaderText="GPS Unit ID" />
            <telerik:GridBoundColumn UniqueName="DepotCode" DataField="DepotCode" HeaderText="Current Controller" />
            <telerik:GridBoundColumn UniqueName="CurrentLocation" DataField="CurrentLocation" HeaderText="Location" />
            <telerik:GridBoundColumn UniqueName="RecordedAt" DataField="RecordedAt" HeaderText="Last Called In" DataFormatString="{0:dd/MM/yy HH:mm}" />
            <telerik:GridBoundColumn UniqueName="Description" DataField="Description" HeaderText="Last Called At" />
            <telerik:GridTemplateColumn UniqueName="ResourceID" HeaderText="">
                <ItemTemplate>
                    <a href="javascript:ShowFuture('<%#((System.Data.DataRowView)Container.DataItem)["ResourceId"]%>','<%=((int)Orchestrator.eResourceType.Trailer).ToString() %>','<%= DateTime.UtcNow.AddDays(-1).ToString("ddMMyyyy")%>' + '0000');">Resource Future</a>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>
    </MasterTableView>
    <FilterMenu EnableTheming="True">
        <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
    </FilterMenu>
</telerik:RadGrid>

    <script type="text/javascript">
	<!--
	    function openInNewWindow(url)
		{
            if (url.indexOf('?') == -1)
                url = url + '?wiz=true';
            else
                url = url + '&wiz=true';

		    _showModalDialogWithDimensions(url, 532, 580);
		}
	    
	    function ShowFuture(resourceId, resourceTypeId, fromDate)
		{
		    var url = '../../Resource/Future.aspx?wiz=true&resourceId=' + resourceId + '&resourceTypeId=' + resourceTypeId + '&fromDateTime=' + fromDate;
		    openResizableDialogWithScrollbars(url, 1050, 492);
		}

	

	    // Function to display the column configure box 
	    function ColumnDisplayShow() {
	        $("#tabs").css({ 'display': 'none' });
	        $("#dvColumnDisplay").css({ 'display': 'block' });
	    }

	    // Function to hide the column configure box 
	    function ColumnDisplayHide() {
	        $("#tabs").css({ 'display': 'block' });
	        $("#dvColumnDisplay").css({ 'display': 'none' });
	    }

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

	    $(document).ready(function() {

	        FilterOptionsDisplayHide();
	        $('#<%=grdTrailers.ClientID %>_ctl00 tbody tr:not(.GroupHeader_Orchestrator)').quicksearch({
	            position: 'after',
	            labelText: '',
	            attached: '#grdFilterHolder',
	            delay: 100
	        });
	    }); 
        //-->
    </script>
	</asp:panel>
	<div style="HEIGHT:10px"></div>
	
	<!--<div class="buttonbar">
	    <input type="button" value="Add Trailer" runat="server" id="hypAddNewTrailer" />
	</div>-->
	
</asp:Content>
