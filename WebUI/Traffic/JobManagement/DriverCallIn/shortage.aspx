<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.DriverCallIn.Shortage"
    CodeBehind="Shortage.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<%@ Reference Control="~/usercontrols/businessruleinfringementdisplay.ascx" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc1" TagName="callInTabStrip" Src="~/UserControls/DriverCallInTabStrip.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">
    Record Shortage</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div class="layoutContentMiddleInner">
                    <div runat="server" id="buttonBar" class="buttonbar" style="text-align: left;">
                        <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
                            <table width="99%" border="0" cellpadding="0" cellspacing="2">
                                <tr>
                                    <td>
                                        <input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px; display: <%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>"
                                            value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td width="100%" align="right">
                                        <iframe marginheight="0" marginwidth="0" frameborder="0" scrolling="no" width="360px"
                                            height="22px" src='/traffic/jobManagement/CallInSelector.aspx?JobId=<%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'>
                                        </iframe>
                                    </td>
                                </tr>
                            </table>
                        </telerik:RadCodeBlock>
                    </div>
                    <uc1:callInTabStrip ID="CallInTabStrip1" runat="server" SelectedTab="2"></uc1:callInTabStrip>
                    <div style="padding-bottom: 10px;">
                    </div>
                    <fieldset>
                        <table border="0" cellpadding="1" cellspacing="0">
                            <tr>
                                <td class="formCellLabel">
                                    Docket
                                </td>
                                <td class="formCellField">
                                    <telerik:RadComboBox HighlightTemplatedItems="true" ID="cboOrder" runat="server"
                                        Width="400px" DataValueField="OrderID">
                                        <HeaderTemplate>
                                            <table>
                                                <tr>
                                                    <td style="width: 90px;">
                                                        <b>Order ID</b>
                                                    </td>
                                                    <td style="width: 120px;">
                                                        <b>
                                                            <asp:Label runat="server" ID="lblHeader"></asp:Label></b>
                                                    </td>
                                                    <td style="width: 175px;">
                                                        <b>Customer</b>
                                                    </td>
                                                </tr>
                                            </table>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <table>
                                                <tr>
                                                    <td style="width: 100px;">
                                                        <%#DataBinder.Eval(Container.DataItem, "OrderID") %>
                                                    </td>
                                                    <td style="width: 130px;">
                                                        <%#DataBinder.Eval(Container.DataItem, "DeliveryOrderNumber") %>
                                                    </td>
                                                    <td style="width: 175px;">
                                                        <%#DataBinder.Eval(Container.DataItem, "CustomerOrganisationName") %>
                                                    </td>
                                                </tr>
                                            </table>
                                        </ItemTemplate>
                                    </telerik:RadComboBox>
                                    <asp:DropDownList ID="cboShortageDocket" runat="server" Width="200px" Visible="false" />
                                    <input type="hidden" id="hidShortageId" runat="server" value="0" name="hidShortageId" />
                                    <input type="hidden" id="hidInstructionId" runat="server" value='<%=m_instruction.ToString()%>'
                                        name="hidInstructionId" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Product Name
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtShortageProductName" runat="server" Width="194px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Product Code
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtShortageProductCode" runat="server" Width="194px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Pack Size
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtShortagePackSize" runat="server" Width="194px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Quantity Despatched
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtShortageQtyDespatched" runat="server" Width="194px"></asp:TextBox>
                                    <asp:CustomValidator ID="cfvShortageQtyDespatched" runat="server" EnableClientScript="False"
                                        ControlToValidate="txtShortageQtyDespatched" ErrorMessage="Please specify a number for the quantity ordered."
                                        Display="Dynamic"><img src="../../../images/Error.gif" height="16" width="16" title="Please specify a number for the quantity ordered." /></asp:CustomValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Quantity Short or Over
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtShortageQty" runat="server" Width="194px"></asp:TextBox>
                                    <asp:CustomValidator ID="cfvShortageQty" runat="server" EnableClientScript="False"
                                        ControlToValidate="txtShortageQty" ErrorMessage="Please specify a number for the quantity refused or returned."
                                        Display="Dynamic"><img src="../../../images/Error.gif" height="16" width="16" title="Please specify a number for the quantity refused or returned." /></asp:CustomValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Reason
                                </td>
                                <td class="formCellField">
                                    <asp:DropDownList ID="cboShortageReason" runat="server" DataTextField="Description"
                                        DataValueField="RefusalTypeId" Width="200px">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" valign="top">
                                    Notes
                                </td>
                                <td class="formCellField">
                                    <asp:TextBox ID="txtShortageNotes" runat="server" TextMode="MultiLine" Columns="80"
                                        Rows="4"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                            </tr>
                        </table>
                    </fieldset>
                    <div class="buttonBar">
                        <asp:Button ID="btnStoreShortage" runat="server"></asp:Button>
                        <asp:Button ID="btnClearShortage" runat="server" Text="Clear Shortage" CausesValidation="False">
                        </asp:Button>
                    </div>
                </div>
            </td>
        </tr>
    </table>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script language="javascript" type="text/javascript">
    <!--
            function HideTop(processValidation) {
                if (processValidation && typeof (Page_ClientValidate) == 'function') {
                    if (Page_ClientValidate()) {
                        var topPortion = document.getElementById("topPortion");
                        var inProgress = document.getElementById("inProgress");

                        if (topPortion != null && inProgress != null) {
                            topPortion.style.display = "none";
                            inProgress.style.display = "";
                        }
                    }
                }
                else {
                    var topPortion = document.getElementById("topPortion");
                    var inProgress = document.getElementById("inProgress");

                    if (topPortion != null && inProgress != null) {
                        topPortion.style.display = "none";
                        inProgress.style.display = "";
                    }
                }
            }
    //-->
        </script>

    </telerik:RadCodeBlock>
</asp:Content>
