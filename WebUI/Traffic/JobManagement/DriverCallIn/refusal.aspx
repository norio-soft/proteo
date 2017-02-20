<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.DriverCallIn.Refusal" CodeBehind="Refusal.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<%@ Reference Control="~/usercontrols/businessruleinfringementdisplay.ascx" %>

<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc1" TagName="callInTabStrip" Src="~/UserControls/DriverCallInTabStrip.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Record Refusal</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    
    <script language="javascript" src="/script/scripts.js" type="text/javascript" />
    <script language="javascript" src="/script/popaddress.js" type="text/javascript" />
    <script language="javascript" type="text/javascript">
        <!--
           
	        window.focus();
    		
	        var returnUrlFromPopUp = window.location;
        //-->
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    
    <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div runat="server" id="buttonBar" class="buttonbar">
                    <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
                        <table border="0" cellpadding="0" cellspacing="2" width="99%">
                            <tr>
                                <td><input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>    '+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px; display: <%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>" value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td width="100%" align="right"><iframe marginheight="0" marginwidth="0" frameborder="0" scrolling="no" width="360px" height="22px" src='/traffic/jobManagement/CallInSelector.aspx?JobId=<%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'></iframe></td>
                            </tr>
                        </table>
                    </telerik:RadCodeBlock>
                </div>
    
                <uc1:callInTabStrip ID="tabStrip1" runat="server" SelectedTab="1"></uc1:callInTabStrip>
                <div style="padding-bottom:10px;"></div>
                <fieldset>
                    <table border="0" cellpadding="1" cellspacing="0">
                        <tr>
                            <td class="formCellLabel">Order ID</td>
                            <td class="formCellField">
                                <!-- show the order/docke to attach this refusal too -->
                                <telerik:RadComboBox HighlightTemplatedItems="true" ID="cboOrder" runat="server" Width="400px" DataValueField="OrderID">
                                    <HeaderTemplate>
                                        <table>
                                            <tr>
                                                <td style="width: 90px;"><b>Order ID</b></td>
                                                <td style="width: 120px;"><b><asp:Label runat="server" ID="lblHeader"></asp:Label></b></td>
                                                <td style="width: 175px;"><b>Customer</b></td>
                                            </tr>
                                        </table>   
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <table>
                                            <tr>
                                                <td style="width: 100px;"><%#DataBinder.Eval(Container.DataItem, "OrderID") %></td>
                                                <td style="width: 130px;"><%#DataBinder.Eval(Container.DataItem, "DeliveryOrderNumber") %></td>
                                                <td style="width: 175px;"><%#DataBinder.Eval(Container.DataItem, "CustomerOrganisationName") %></td>
                                            </tr>
                                        </table>
                                    </ItemTemplate>
                                </telerik:RadComboBox>
                                <input type="hidden" id="hidInstructionId" runat="server" value='' name="hidInstructionId" />
                                <input type="hidden" id="hidRefusalId" runat="server" value='' name="hidRefusalId   " />
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Product Name</td>
                            <td class="formCellField">
                                <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
                                    <asp:TextBox ID="txtProductName" runat="server" Width="194px"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvProductName" runat="server" ErrorMessage="Please enter the product name"
                                        ControlToValidate="txtProductName" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" title="Please enter the product name" /></asp:RequiredFieldValidator>
                                </telerik:RadCodeBlock>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Product Code</td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtProductCode" runat="server" Width="194px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Receipt Number</td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtReceiptNumber" runat="server" Width="194px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Pack Size</td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtPackSize" runat="server" Width="194px"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Quantity Despatched</td>
                            <td class="formCellField">
                                <telerik:RadCodeBlock ID="RadCodeBlock4" runat="server">
                                    <asp:TextBox ID="txtQuantityOrdered" runat="server" Width="194px"></asp:TextBox>
                                    <asp:CustomValidator ID="cfvQuantityOrdered" runat="server" EnableClientScript="False" ControlToValidate="txtQuantityOrdered" ErrorMessage="Please specify a number for the quantity ordered."
                                        Display="Dynamic"><img src="/images/Error.gif" alt="" height="16" width="16" title="Please specify a number for the quantity ordered." /></asp:CustomValidator>
                                </telerik:RadCodeBlock>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Quantity Refused or Returned</td>
                            <td class="formCellField">
                                <telerik:RadCodeBlock ID="RadCodeBlock5" runat="server">
                                    <asp:TextBox ID="txtQuantityReturned" runat="server" Width="194px"></asp:TextBox>
                                    <asp:CustomValidator ID="cfvQuantityReturned" runat="server" EnableClientScript="False" ControlToValidate="txtQuantityReturned" ErrorMessage="Please specify a number for the quantity refused or returned."
                                        Display="Dynamic"><img src="/images/Error.gif" alt="" height="16" width="16" title="Please specify a number for the quantity refused or returned." /></asp:CustomValidator>
                                </telerik:RadCodeBlock>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Pallets</td>
                            <td class="formCellField">
                                <telerik:RadNumericTextBox ID="rntNoPallets" runat="server" Width="25px" MinValue="0" NumberFormat-DecimalDigits="0" Type="Number" Value="0" />
                                <asp:RequiredFieldValidator ID="rfvNoPallets" runat="server" ControlToValidate="rntNoPallets"
										Display="Dynamic" ErrorMessage="Please enter the number of pallets.">
										<img id="Img3" runat="server" src="~/images/error.png" title="Please enter the number of pallets." /></asp:RequiredFieldValidator>
								<asp:RegularExpressionValidator ID="revPallets" runat="server" ControlToValidate="rntNoPallets"
										Display="Dynamic" ValidationExpression="^[0-9]*" ErrorMessage="Please enter a valid Number.">
										<img id="Img4" runat="server" src="~/images/error.png" title="Please enter a valid number." /></asp:RegularExpressionValidator>
									<asp:CustomValidator ID="cfvNoPallets" runat="server" ControlToValidate="rntNoPallets"
										EnableClientScript="false" ErrorMessage="The Number of pallets must equal 1 for Undersized pallet spaces."
										Display="Dynamic">
                                        <img src="~/images/error.png" title="The Number of pallets must equal 1 for Undersized pallet spaces." /></asp:CustomValidator>
									
                            
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Pallet Spaces</td>
                            <td class="formCellField">
                                <telerik:RadNumericTextBox ID="rntNoPalletSpaces" runat="server" Width="25px" MinValue="0" NumberFormat-DecimalDigits="0" Type="Number" Value="0" />
                                <asp:RequiredFieldValidator ID="rfvPalletSpaces" runat="server" ControlToValidate="rntNoPalletSpaces"
										Display="Dynamic" ErrorMessage="Please enter a value for the number of Pallet Spaces.">
										<img id="Img8" runat="server" src="~/images/error.png" title="Please enter a value for the number of Pallet Spaces." /></asp:RequiredFieldValidator>
                                    <asp:CustomValidator ID="cfvPalletSpaces" runat="server" ControlToValidate="rntNoPalletSpaces" ClientValidationFunction="validateSpaces"
                                        Display="Dynamic" ErrorMessage="Please provide the number of Pallet Spaces between 0 and 9,999.">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please provide the number of Pallet Spaces between 0 and 9,999." /></asp:CustomValidator>
									
                           
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Reason</td>
                            <td class="formCellField">
                                <asp:DropDownList ID="cboGoodsReasonRefused" runat="server" DataTextField="Description" DataValueField="RefusalTypeId" Width="200px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trDeviationReason">
                            <td class="formCellLabel">Deviation Reason</td>
                            <td colspan="2" class="formCellField">
                                <asp:DropDownList ID="cboDeviationReason" runat="server" >
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Status</td>
                            <td class="formCellField">
                                <telerik:RadCodeBlock ID="RadCodeBlock6" runat="server">
                                    <asp:DropDownList ID="cboGoodsStatus" runat="server" Width="200px"></asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="rfvGoodsStatus" runat="server" ControlToValidate="cboGoodsStatus"
                                        Display="Dynamic" ErrorMessage="Please select a valid status for these goods."><img src="/images/Error.gif" height="16" width="16" title="Please select a valid status for these goods." /></asp:RequiredFieldValidator>
                                    <asp:CustomValidator ID="cfvGoodsStatus" runat="server" ControlToValidate="cboGoodsStatus"
                                        Display="Dynamic" EnableClientScript="false" ErrorMessage="Please select a valid status for these goods."><img src="/images/Error.gif" height="16" width="16" title="Please select a valid status for these goods." /></asp:CustomValidator>
                                </telerik:RadCodeBlock>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Current Location</td>
                            <td class="formCellField">
                                <asp:DropDownList ID="cboLocation" runat="server" Width="200px"></asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Return to Supplier By</td>
                            <td class="formCellField">
                                <telerik:RadCodeBlock ID="RadCodeBlock7" runat="server">
                                    <telerik:RadDateInput ID="rdiReturnDate" runat="server" Width="60px" DateFormat="dd/MM/yy" ToolTip="The date the goods must be returned to the supplier by" />
                                    <asp:RequiredFieldValidator ID="rfvReturnDate" runat="server" ErrorMessage="Please enter the date the goods must be returned to the supplier by" ControlToValidate="rdiReturnDate" Display="Dynamic" EnableClientScript="True">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please enter the date the goods must be returned to the supplier by" />
                                    </asp:RequiredFieldValidator>
                                </telerik:RadCodeBlock>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Notes</td>
                            <td class="formCellField">
                                <asp:TextBox ID="txtGoodsNotes" runat="server" TextMode="MultiLine" Columns="80" Rows="4"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                </fieldset>

                <fieldset style="padding: 0px 5px 5px 5px;">
                    <legend>Location the Goods should be Returned to</legend>
                    <telerik:RadAjaxPanel ID="raxReturnedToPoint" runat="server">
                        <p1:Point runat="server" ID="ucReturnedToPoint" ShowFullAddress="true" CanClearPoint="true" CanUpdatePoint="true" ShowPointOwner="true" Visible="true" />
                    </telerik:RadAjaxPanel>
                </fieldset>

                <fieldset>
                    <legend>Location the Goods will be Stored at</legend>
                    <telerik:RadAjaxPanel ID="raxStoredAtPoint" runat="server">
                        <telerik:RadScriptBlock  ID="RadScriptBlock1" runat="server"> 
                            <p1:Point runat="server" ID="ucStoredAtPoint" ShowFullAddress="true" CanClearPoint="true" CanUpdatePoint="true" ShowPointOwner="true" Visible="true" />
                        </telerik:RadScriptBlock>
                    </telerik:RadAjaxPanel>
                </fieldset>
    
                <div class="buttonbar">
                    <asp:Button ID="btnStoreGoods" runat="server"></asp:Button>
                    &nbsp;<asp:Button ID="btnStoreGoodsAndReset" runat="server"></asp:Button>
                    &nbsp;<asp:Button ID="btnCancelGoods" runat="server" Text="Clear Goods" CausesValidation="False">
                    </asp:Button>
                </div>
    
            </td>
        </tr>
    </table>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">         
        <script type="text/javascript" language="javascript">
        <!--

            function pageLoad() {

                var displayDeviationReason = '<%= Orchestrator.Globals.Configuration.DisplayDeviationCode %>';
                if (displayDeviationReason == 'True')
                    $("#trDeviationReason").css("display", "");
                else
                    $("#trDeviationReason").css("display", "none");
            }

	        function HideTop(processValidation)
	        {
		        if (processValidation && typeof(Page_ClientValidate) == 'function')
		        {
			        if (Page_ClientValidate())
			        {
				        var topPortion = document.getElementById("topPortion");
				        var inProgress = document.getElementById("inProgress");
        				
				        if (topPortion != null && inProgress != null)
				        {
					        topPortion.style.display = "none";
					        inProgress.style.display = "";
				        }
			        }
		        }
		        else
		        {
			        var topPortion = document.getElementById("topPortion");
			        var inProgress = document.getElementById("inProgress");
        			
			        if (topPortion != null && inProgress != null)
			        {
				        topPortion.style.display = "none";
				        inProgress.style.display = "";
			        }
		        }
	        }
         --> 
        </script>
    </telerik:RadCodeBlock>
</asp:Content>