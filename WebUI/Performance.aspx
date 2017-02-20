<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default.master" CodeBehind="Performance.aspx.cs" Inherits="Orchestrator.WebUI.Performance" %>
<%@ Register TagPrefix="igtxt" Namespace="Infragistics.WebUI.WebDataInput" Assembly="Infragistics.WebUI.WebDataInput.v1.1, Version=1.1.20042.26, Culture=neutral, PublicKeyToken=7dd5c3163f2cd0cb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
    <fieldset>
        <legend>Performance Filter</legend>
        <telerik:RadAjaxPanel Width="100%" ID="AllPagesPerformancePanel" runat="server">
        <table width="100%">
          <tr>
            <td width="80">Date From</td>
            <td align="left">
                <igtxt:webdatetimeedit id="dteAllPagesStartDate" runat="server" 
                EditModeFormat="dd/MM/yy" Width="60"></igtxt:webdatetimeedit>
                <asp:RequiredFieldValidator ID="rfvStartDate" ValidationGroup="AllPageValidation" runat="server" Display="Dynamic" EnableClientScript="true" ControlToValidate="dteAllPagesStartDate" ErrorMessage="Please enter a Start Date"></asp:RequiredFieldValidator>
            </td>
          </tr>
          <tr>
            <td align="left" width="80">Date To</td>
            <td align="left">
                <igtxt:webdatetimeedit id="dteAllPagesEndDate" runat="server" 
                    EditModeFormat="dd/MM/yy" Width="60"></igtxt:webdatetimeedit>
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ValidationGroup="AllPageValidation" Display="Dynamic" EnableClientScript="true" ControlToValidate="dteAllPagesEndDate" ErrorMessage="Please enter a To Date"></asp:RequiredFieldValidator>
            </td>
          </tr>
          <tr>
            <td align="left" width="80">User</td>
            <td align="left" colspan="3">
                <telerik:RadComboBox ID="cboUsers" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="True"
                                    ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px"></telerik:RadComboBox>
            </td>
            <td width="75" align="right">
                <asp:Button ID="btnAllPagesRefresh" runat="server" Text="Refresh" 
                    onclick="btnAllPagesRefresh_Click" Width="75" ValidationGroup="AllPageValidation" />
            </td>
            <td width="75" align="right">
                <asp:Button ID="btnExportAll" runat="server" Text="Export" Width="75" 
                    onclick="btnExportAll_Click" ValidationGroup="AllPageValidation" />
            </td>
          </tr>
        </table>
        <div>
            <telerik:RadChart ID="radAllPagesChart" runat="server" Width="700" Height="450"
                            AlternateText="" CatalogIconImageUrl="" Description="" Margins-Bottom="12%" 
                Margins-Left="10%" Title="" TitleIconImageUrl="" TitleUrl="" Visible="false" >
            
                <XAxis MaxValue="5" MinValue="1" Step="1" >
                </XAxis>
                <Gridlines>
                    <VerticalGridlines Visible="False" />
                </Gridlines>
                
                <Title1 HorPadding="10" Text="Performance chart of all Pages between {0} and {1}">
                    <Background BorderColor="199, 199, 199" FillStyle="Solid" MainColor="White" />
                </Title1>

                <YAxis Step="20">
                </YAxis>
                <Legend>
                    <Background BorderColor="227, 227, 227" FillStyle="Solid" MainColor="White" />
                </Legend>
            </telerik:RadChart>
        </div>
        </telerik:RadAjaxPanel>
    </fieldset>
    <br />
    <fieldset>
        <legend>Single Page Performance</legend>
        <telerik:RadAjaxPanel Width="100%" ID="SinglePagePerformancePanel" runat="server">
        <table width="100%">
          <tr>
            <td width="80">Date From</td>
            <td colspan="2" align="left">
                <igtxt:webdatetimeedit id="dteSinglePageStartDate" runat="server" EditModeFormat="dd/MM/yy" Width="60"></igtxt:webdatetimeedit>
                <asp:RequiredFieldValidator ID="rfvdteSinglePageStartDate" ValidationGroup="SinglePageValidation" runat="server" EnableClientScript="true" ControlToValidate="dteSinglePageStartDate" Display="Dynamic" ErrorMessage="Please enter a Start Date"></asp:RequiredFieldValidator>
            </td>
          </tr>
          <tr>
            <td width="80">Date To</td>
            <td align="left" colspan="2">
                <igtxt:webdatetimeedit id="dteSinglePageEndDate" runat="server" EditModeFormat="dd/MM/yy" Width="60"></igtxt:webdatetimeedit>
                <asp:RequiredFieldValidator ID="rfvdteSinglePageEndDate" runat="server" ValidationGroup="SinglePageValidation" EnableClientScript="true" ControlToValidate="dteSinglePageEndDate" Display="Dynamic" ErrorMessage="Please enter a To Date"></asp:RequiredFieldValidator>
            </td>
          </tr>
          <tr>
          <td align="left" width="80">User</td>
            <td align="left" colspan="3">
                <telerik:RadComboBox ID="cboSinglePageUser" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="True"
                                    ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px"></telerik:RadComboBox>
            </td>
          </tr>
          <tr>
            <td width="80">Page</td>
            <td align="left">
                <telerik:RadComboBox ID="cboPage" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                    ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px"></telerik:RadComboBox>
            </td>
            <td width="75" align="right">
                <asp:Button ID="btnSinglePageRefresh" runat="server" Text="Refresh" Width="75" 
                    onclick="btnSinglePageRefresh_Click" ValidationGroup="SinglePageValidation" />
            </td>
            <td width="75" align="right">
                <asp:Button ID="btnExportSingle" runat="server" Text="Export" Width="75" 
                    onclick="btnExportSingle_Click" ValidationGroup="SinglePageValidation" />
            </td>
          </tr>
        </table>
        <div style="width:100%">
            <telerik:RadChart ID="radSinglePageChart" runat="server" Width="500px" Height="450"
                AlternateText="" CatalogIconImageUrl="" Description="" Margins-Bottom="12%" 
                Margins-Left="10%" Title="" TitleIconImageUrl="" TitleUrl="" Visible="false">
                <Gridlines>
                    <VerticalGridlines Visible="False" />
                </Gridlines>
                <Title1 HorPadding="10" Text="">
                    <Background BorderColor="199, 199, 199" FillStyle="Solid" MainColor="White" />
                </Title1>
                <Legend>
                    <Background BorderColor="227, 227, 227" FillStyle="Solid" MainColor="White" />
                </Legend>
            </telerik:RadChart>
        </div>
        </telerik:RadAjaxPanel>
    </fieldset>
    </div>

</asp:Content>