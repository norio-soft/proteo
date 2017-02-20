<%@ Page Language="c#" Inherits="Orchestrator.WebUI.GoodsRefused.addupdategoodsrefused"
    CodeBehind="addupdategoodsrefused.aspx.cs" MasterPageFile="~/WizardMasterPage.Master"
    Title="Haulier Enterprise" AutoEventWireup="True" %>

<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <link rel="stylesheet" type="text/css" href="/style/newStyles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newMasterpage.css" />
</asp:Content>
<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">
    Add / Update Goods</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgRun" runat="server" Mode="Modal" Resizable="false" URL="/Job/Job.aspx"
        Width="1120" Height="800" UseCookieSessionID="true"/>
    <div style="height: 10px;">
    </div>
    <h3>
        Configure the
        <asp:Label runat="server" ID="lblReturnTypeHeading" Text="Refusal"></asp:Label>
        Information</h3>
    <asp:Label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation"
        Text="The new Trailer has been added successfully.">The Goods have been added successfully.</asp:Label>
    <div>
        <div style="float: left;">
            <table>
                <tr>
                    <td class="formCellLabel">
                        Refused on Run Id
                    </td>
                    <td colspan="2" class="formCellField">
                        <a runat="server" id="hypRefusalRunId"></a>
                    </td>
                    <td rowspan="15">
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Return Run Id(s)
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:Repeater runat="server" ID="rptReturnRunLinks">
                            <ItemTemplate>
                                <a runat="server" id="hypReturnRun"></a>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                    <td rowspan="15">
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Load No
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:Label ID="lblLoadNo" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Docket No
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:Label runat="server" ID="lblDocket"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Client
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:Label ID="lblClient" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Date Delivered
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:Label ID="lblDateDelivered" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Refusal Type
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:DropDownList ID="cboRefusalType" runat="server" AutoPostBack="true">
                        </asp:DropDownList>
                    </td>
                </tr>
                 <tr id="trDeviationReason">
                    <td class="formCellLabel">
                        Deviation Reason
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:DropDownList ID="cboDeviationReason" runat="server" >
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Product Name
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtProductName" runat="server" Width="150"></asp:TextBox>
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvProductName" runat="server" Text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
                            Display="Dynamic" ControlToValidate="txtProductName" ErrorMessage="Please enter a Product Name.">
                            <img src="/images/Error.gif" height='16' width='16' title='Please enter a product name.' alt='Field is Required'>
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Product Code
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:TextBox ID="txtProductCode" runat="server" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Quantity Ordered
                    </td>
                    <td class="formCellField" colspan="2">
                        <telerik:RadNumericTextBox Value="0" runat="server" ID="rntQuantityOrdered" Width="150"
                            NumberFormat-DecimalDigits="0" MinValue="0" Type="Number">
                        </telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        <asp:Label runat="server" ID="lblQuantityRefused" Text="Quantity Refused"></asp:Label>
                    </td>
                    <td class="formCellField" colspan="2">
                        <telerik:RadNumericTextBox Value="0" runat="server" ID="rntQuantityRefused" Width="150"
                            NumberFormat-DecimalDigits="0" MinValue="0" Type="Number">
                        </telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Pack Size
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:TextBox ID="txtPackSize" runat="server" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Receipt Number
                    </td>
                    <td colspan="2" class="formCellField">
                        <asp:TextBox ID="txtReceiptNumber" runat="server" Width="150"></asp:TextBox>
                    </td>
                </tr>
                <tr style="display: none;">
                    <td class="formCellLabel">
                        Return By
                    </td>
                    <td colspan="2" class="formCellField">
                        <telerik:RadDateInput ID="rdiTimeFrame" Width="100" runat="server" DisplayDateFormat="dd/MM/yy"
                            DateFormat="dd/MM/yy">
                        </telerik:RadDateInput>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Status
                    </td>
                    <td class="formCellField">
                        <telerik:RadCodeBlock ID="RadCodeBlock6" runat="server">
                            <asp:DropDownList ID="cboGoodsStatus" runat="server" Width="200px">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rfvGoodsStatus" runat="server" ControlToValidate="cboGoodsStatus" Display="Dynamic" ErrorMessage="Please select a valid status for these goods.">
                                <img src="/images/Error.gif" height="16" width="16" alt="" title="Please select a valid status for these goods." />
                            </asp:RequiredFieldValidator>
                        </telerik:RadCodeBlock>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        <asp:Label ID="lblCurrentLocation" runat="server">Current Location</asp:Label>
                    </td>
                    <td class="formCellField">
                        <asp:RadioButtonList ID="cboLocation" runat="server" RepeatDirection="Horizontal"
                            RepeatColumns="6">
                        </asp:RadioButtonList>
                        <asp:Label ID="lblLocation" runat="server" Text="" />
                        <br />
                        <asp:Label ID="lblAtStore" runat="server" Text="Check when At Store:" Visible="false" />
                        <asp:CheckBox ID="chkAtStore" runat="server" Checked="false" Visible="false" />
                    </td>
                    <td class="formCellField">
                        <asp:RequiredFieldValidator ID="rfvCurrentLocation" runat="server" EnableClientScript="True"
                            Display="Dynamic" ControlToValidate="cboLocation" ErrorMessage="Please select current location">
                            <img src="/images/Error.gif" height="16" width="16" title="Please select a current location." />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Refusal Notes
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtRefusalNotes" runat="server" Width="250px" Height="80px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
        <div style="float: left;">
            <table cellpadding="0" cellspacing="0">
                <tr runat="server" id="trReturnPoint">
                    <td class="formCellLabel">
                        Return To
                    </td>
                    <td colspan="2" class="formCellField">
                        <p1:Point runat="server" ID="ucReturnToPoint" ShowFullAddress="true" CanClearPoint="true"
                            CanUpdatePoint="true" ShowPointOwner="false" Visible="true" IsDepotVisible="false" />
                    </td>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Stored At
                    </td>
                    <td colspan="2" class="formCellField">
                        <p1:Point runat="server" ID="ucStoreAtPoint" ShowFullAddress="true" CanClearPoint="true"
                            CanUpdatePoint="true" ShowPointOwner="false" Visible="true" IsDepotVisible="false" />
                    </td>
                    <td>
                    </td>
                </tr>
            </table>
        </div>
        <div class="clearDiv">
        </div>
        <div style="height: 10px;">
        </div>
        <asp:Panel ID="pnlGoodsRefusedDeleted" runat="server" Visible="False">
            <fieldset>
                <legend><strong>
                    <p>
                        Are Goods Deleted</p>
                </strong></legend>
                <asp:CheckBox ID="chkDelete" runat="server" Text="These Goods are deleted."></asp:CheckBox>
            </fieldset>
        </asp:Panel>
        <asp:ValidationSummary ID="vsAddUpdateGoodsRefused" runat="server" ShowMessageBox="True"
            ShowSummary="False"></asp:ValidationSummary>
        <br />
        <div class="buttonbar" style="float: left; width: 98%">
            <asp:Button ID="btnAdd" runat="server" Text="Add New Goods Refused" />
            <asp:Button Visible="false" ID="btnReturnToRefusedGoods" runat="server" Text="Return to refused goods"
                CausesValidation="false" />
            <input type="button" value="Close" onclick="javascript: window.close();" />
        </div>
    </div>
    <div style="height: 35px;">
    </div>

    <script type="text/javascript">
        
        function pageLoad()
        {
            var displayDeviationReason = '<%= Orchestrator.Globals.Configuration.DisplayDeviationCode %>';
            if(displayDeviationReason == 'True')
                $("#trDeviationReason").css("display", "");
            else
                 $("#trDeviationReason").css("display", "none");
        }

        function OpenRunDetails(runID) 
        {
            var qs = "jobId=" + runID + "&wiz=true";
	        <%=dlgRun.ClientID %>_Open(qs);	            
        }
        
    </script>

</asp:Content>
