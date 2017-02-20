<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubContract.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.SubContract" MasterPageFile="~/WizardMasterPage.master" Title="Sub Contract" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Sub Contract</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <table cellspacing="2" width="100%">
        <tr>
            <td class="greyText" width="100%" valign="top">
                <div style="width: 100%; ">
                    <asp:Panel ID="pnlSubContract" runat="server" >
                        <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
                        <table width="99%">
                            
                            <tr>
                                <td width="25%">
                                    Sub-Contract to:
                                </td>
                                <td>
                                   <telerik:RadComboBox ID="cboSubContractor" runat="server" AutoPostBack="true"
                                        OnSelectedIndexChanged="cboSubContractor_SelectedIndexChanged"
                                        EnableLoadOnDemand="true" ShowMoreResultsBox="false" MarkFirstMatch="true"
                                        AllowCustomText="false" ItemRequestTimeout="500" Width="155px">
                                    </telerik:RadComboBox>
                                    <asp:RequiredFieldValidator ID="rfvSubContractor" runat="server" ControlToValidate="cboSubContractor"
                                        ErrorMessage="<img src='../images/error.png' Title='Please select a Sub-Contractor.'>"
                                        EnableClientScript="True"></asp:RequiredFieldValidator>
                                   
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2" >
                                    <asp:Label ID="lblMissingDocumentsAlert" runat="server" ForeColor="Red" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Sub-Contract Rate
                                </td>
                                <td>
                                    <telerik:RadNumericTextBox ID="rntSubContractRate" runat="server" Type="Currency" />
                                    <asp:RequiredFieldValidator ID="rfvSubContractorRate" runat="server" ControlToValidate="rntSubContractRate"
                                        ErrorMessage="Please supply a rate."><img src="../images/Error.gif" height="16" width="16" title="Please supply a rate." /></asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="revSubContractorRate" runat="server" ControlToValidate="rntSubContractRate"
                                        ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$"
                                        ErrorMessage="Please enter a valid currency value for the rate."><img src="../../images/Error.gif" height="16" width="16" title="Please enter a valid currency value for the rate." /></asp:RegularExpressionValidator>
                                </td>
                            </tr>
                            
                            <tr>
                                <td nowrap="nowrap">
                                    Use Sub-Contractor's Trailer
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkUseSubContractorTrailer" runat="server" Checked="True" AutoPostBack="True">
                                    </asp:CheckBox>
                                </td>
                            </tr>
                            <tr>
                                <td nowrap="nowrap">
                                    <asp:Label ID="lblShowAsCommunicated" runat="server" Visible="false" >Show as Communicated</asp:Label>  
                                </td>
                                <td>
                                    <asp:CheckBox ID="chkShowAsCommunicated" runat="server" Checked="False" Visible="false" AutoPostBack="False" >
                                    </asp:CheckBox>
                                </td>
                            </tr>
                            <asp:Panel ID="pnlTrailer" runat="server" Visible="false">
                                <tr>
                                    <td nowrap="nowrap">
                                        Use your Trailer
                                    </td>
                                    <td>
                                        <telerik:RadComboBox ID="cboTrailer" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="false"
                                            MarkFirstMatch="true" OnClientItemsRequesting="trailerRequesting" ItemRequestTimeout="500" Width="155px"
                                            Height="100px" AllowCustomText="false">
                                            <WebServiceSettings Path="../ws/combostreamers.asmx" Method="GetTrailers" />
                                        </telerik:RadComboBox>
                                        <asp:RequiredFieldValidator ID="rfvTrailer" runat="server" ControlToValidate="cboTrailer"
                                            ErrorMessage="<img src='../images/error.png' Title='Please select a Trailer.'>"
                                            EnableClientScript="True"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </asp:Panel>
                        </table>
                        <hr />
                        Pricing Mechanism:
                        <br />
                        <table width="100%">
                            <tr>
                                <td>
                                    <asp:RadioButtonList ID="rdoSubContractMethod" runat="server" AutoPostBack="true"
                                        RepeatDirection="horizontal">
                                        <asp:ListItem Value="0" Text="Whole Job" />
                                        <asp:ListItem Value="1" Text="Specific Legs" />
                                        <asp:ListItem Value="2" Text="Per Order" />
                                    </asp:RadioButtonList><asp:RequiredFieldValidator runat="server" ControlToValidate="rdoSubContractMethod" ErrorMessage="<img src='../images/error.png' Title='Please select a subcontract method.'>"></asp:RequiredFieldValidator>
                                </td>
                                <td style="text-align: right;">
                                    <asp:CheckBox runat="server" ID="chkForceRemoveResources" Text="Force removal of resources" />
                                </td>
                            </tr>
                        </table>
                        <telerik:RadGrid runat="server" ID="grdLegs" AllowPaging="false" AllowMultiRowSelection="true"
                            AllowSorting="true" AutoGenerateColumns="false"
                            Width="630px" >
                            <MasterTableView Width="610px" DataKeyNames="InstructionID" >
                                <RowIndicatorColumn Display="false">
                                </RowIndicatorColumn>
                                <Columns>
                                    <telerik:GridClientSelectColumn UniqueName="checkboxSelectColumn" HeaderStyle-HorizontalAlign="Left"
                                        HeaderStyle-Width="40" HeaderText="">
                                    </telerik:GridClientSelectColumn>
                                    <telerik:GridTemplateColumn HeaderText="OrderIds">
                                        <ItemTemplate>
                                            <asp:HiddenField runat="server" id="hidInstructionId" />
                                            <asp:Label ID="lblOrderIDs" runat="server" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="From">
                                        <ItemTemplate>
                                            <b><span runat="server" id="spnCollectionPoint"></span></b>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="To">
                                        <ItemTemplate>
                                            <b><span runat="server" id="spnDestinationPoint"></span></b>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Assigned To">
                                        <ItemTemplate>
                                            <span runat="server" id="spnDriver"></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Trailer">
                                        <ItemTemplate>
                                            <span runat="server" id="spnTrailer"></span>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings >
                                <Scrolling AllowScroll="true" ScrollHeight="300px" />
                                <Selecting AllowRowSelect="true" />
                            </ClientSettings>
                        </telerik:RadGrid>
                    </asp:Panel>
                    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
                        <div class="MessagePanel">
                            <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /><asp:Label
                                ID="lblMessage" runat="server">The job has been subcontracted</asp:Label>
                        </div>
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>
    <div class="buttonbar">
        <asp:Button ID="btnConfirmSubContract" runat="server" Text="Confirm" Style="width: 75px" />
        <input type="button" onclick="window.close();" value="Cancel" />
    </div>
    
    <script type="text/javascript">

        function trailerRequesting(sender, eventArgs)
        {
            var cstring ="<%=Request.QueryString["CA"] %>:<%=Request.QueryString["TA"] %>";

            var context = eventArgs.get_context();
            context["FilterString"] = cstring;
        }  

//        function GetRadWindow()
//        {
//            var oWindow = null;
//            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
//            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well) 
//            return oWindow;
//        }

//        function CloseOnReload()
//        {
//            var oWnd = GetRadWindow();
//            
//            if(oWnd != null)
//            {
//                GetRadWindow().Close();
//            }
//        }
//        
//        function RefreshParentPage()
//        {
//            var oWnd = GetRadWindow();
//            
//            if(oWnd != null)
//            {
//                GetRadWindow().BrowserWindow.location.href = GetRadWindow().BrowserWindow.location.href;
//                GetRadWindow().Close();
//            }
//        }
    </script>
    
    <asp:Label ID="InjectScript" runat="server"></asp:Label>
</asp:Content>
