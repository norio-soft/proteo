<%@ Register TagPrefix="igtbl" Namespace="Infragistics.WebUI.UltraWebGrid" Assembly="Infragistics.WebUI.UltraWebGrid.v3.1, Version=3.1.20042.26, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.ShuntList" Codebehind="shuntlist.aspx.cs" %>
<%@ Register TagPrefix="igtxt" Namespace="Infragistics.WebUI.WebDataInput" Assembly="Infragistics.WebUI.WebDataInput.v1.1, Version=1.1.20042.26, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register Assembly="RadCombobox.Net2" Namespace="Telerik.WebControls" TagPrefix="radC" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<P><uc1:header id="Header1" title="Job Details" SubTitle="Please enter the Job Details below."
		XMLPath="JobContextMenu.xml" runat="server"></uc1:header></P>
<P>
	<form id="Form1" runat="server">
</P>
<fieldset>
	<legend>Filter 
Details</legend>
	<TABLE height="16" width="448">
		<tr>
			<td width="83">Client</td>
			<td>
                <radC:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                    ShowMoreResultsBox="true" Skin="WindowsXP" Width="355px">
                </radC:RadComboBox>    
			</td>
		</tr>
	</TABLE>
	<asp:button id="btnFilter" runat="server" Text="Produce Daily Shunt List" onclick="btnFilter_Click"></asp:button>
</fieldset>&nbsp;
<br>
<fieldset>
	<P>
		<legend>Daily 
Shunt List</legend></P>
	<P><EM>A list of job(s) which are due for loading (collection) for today even if these 
			job(s) are for delivery the next day.</EM></P>
	<P><igtbl:ultrawebgrid id="grdDailyShuntList" runat="server" Height="250px" Width="100%" DataMember="Table"
			Name="grdDeliveryPoints">
			<DisplayLayout AllowDeleteDefault="Yes" JavaScriptFileName="/ig_common/webgrid3/ig_webgrid.js"
				AllowAddNewDefault="Yes" AllowSortingDefault="Yes" RowHeightDefault="20px" Version="3.00" SelectTypeRowDefault="Single"
				AllowColumnMovingDefault="OnServer" BorderCollapseDefault="Separate" AllowColSizingDefault="Free"
				Name="grdCancelledJobs" CellClickActionDefault="RowSelect" AllowUpdateDefault="Yes">
				<AddNewBox ButtonConnectorStyle="Ridge">
					<Style BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">

<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
</BorderDetails>

</Style>
					<ButtonStyle Width="120px" Cursor="Hand" BackColor="Control" Height="18px">
						<BorderDetails ColorTop="77, 74, 70" WidthLeft="1px" ColorBottom="77, 74, 70" WidthTop="1px" ColorRight="77, 74, 70"
							WidthRight="1px" WidthBottom="1px" ColorLeft="77, 74, 70"></BorderDetails>
					</ButtonStyle>
				</AddNewBox>
				<Pager>
					<Style BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray">

<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White">
</BorderDetails>

</Style>
				</Pager>
				<HeaderStyleDefault VerticalAlign="Middle" BorderWidth="1px" BorderStyle="Solid" HorizontalAlign="Left"
					BackColor="LightGray" Height="26px">
					<Padding Left="3px" Right="3px"></Padding>
					<BorderDetails ColorTop="White" WidthLeft="1px" ColorBottom="Gray" WidthTop="1px" ColorRight="Gray"
						ColorLeft="White"></BorderDetails>
				</HeaderStyleDefault>
				<FrameStyle Width="100%" BorderWidth="1px" Font-Size="8pt" Font-Names="Verdana" BorderStyle="Solid"
					Height="250px"></FrameStyle>
				<FooterStyleDefault BorderWidth="1px" BorderStyle="Solid" BackColor="LightGray" Height="26px">
					<Padding Left="3px" Right="3px"></Padding>
					<BorderDetails ColorTop="White" WidthLeft="1px" WidthTop="1px" ColorLeft="White"></BorderDetails>
				</FooterStyleDefault>
				<ClientSideEvents AfterExitEditModeHandler="AfterExitEditMode" AfterRowInsertHandler="AfterRowInsert"></ClientSideEvents>
				<EditCellStyleDefault BorderWidth="0px" BorderStyle="None"></EditCellStyleDefault>
				<SelectedRowStyleDefault ForeColor="White" BackColor="#D4B883"></SelectedRowStyleDefault>
				<RowAlternateStyleDefault BackColor="#C5CADB"></RowAlternateStyleDefault>
				<RowStyleDefault BorderWidth="1px" BorderColor="Gray" BorderStyle="Solid">
					<Padding Left="3px"></Padding>
					<BorderDetails WidthLeft="0px" WidthTop="0px"></BorderDetails>
				</RowStyleDefault>
				<ImageUrls CurrentRowImage="ig_tblTri_Black3d.gif"></ImageUrls>
			</DisplayLayout>
			<Bands>
				<igtbl:UltraGridBand BaseTableName="Table" RowSelectors="No">
					<Columns>
						<igtbl:UltraGridColumn HeaderText="Job Id" Key="JobId" IsBound="True" Width="50px" BaseColumnName="JobId"></igtbl:UltraGridColumn>
						<igtbl:UltraGridColumn HeaderText="Job Date" Key="JobDate" IsBound="True" Width="80px" BaseColumnName="JobDate" Format="dd/MM/yy HH:mm"></igtbl:UltraGridColumn>
						<igtbl:UltraGridColumn HeaderText="Client Id" Key="ClientIdentityId" IsBound="True" Hidden="True" BaseColumnName="ClientIdentityId"></igtbl:UltraGridColumn>
						<igtbl:UltraGridColumn HeaderText="Client" Key="Client" IsBound="True" Width="100px" BaseColumnName="OrganisationName"></igtbl:UltraGridColumn>
					</Columns>
					<RowTemplateStyle BorderColor="White" BorderStyle="Ridge" BackColor="White">
						<BorderDetails WidthLeft="3px" WidthTop="3px" WidthRight="3px" WidthBottom="3px"></BorderDetails>
					</RowTemplateStyle>
				</igtbl:UltraGridBand>
			</Bands>
		</igtbl:ultrawebgrid></P>
</fieldset>

<P>
	<asp:Button id="btnFax" runat="server" Text="Fax List to Client" onclick="btnFax_Click"></asp:Button></P>
<P>
	<asp:Button id="btnEMail" runat="server" Text="E-Mail List to Client" onclick="btnEMail_Click"></asp:Button></P>
<P>
	<uc1:footer id="Footer1" runat="server"></uc1:footer></P>
</FORM>
