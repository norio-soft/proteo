<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.JobManagement.Pricing2"
    CodeBehind="pricing2.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
    <title>Orchestrator - Run Pricing</title>

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>

    <script language="javascript" src="/script/popaddress.js" type="text/javascript"></script>

    <script language="javascript" src="/script/NumberFormat154.js" type="text/javascript"></script>

    <link rel="stylesheet" type="text/css" href="../../../style/styles.css" />
    <link href="../../../style/helpTip.css" rel="stylesheet" type="text/css" />
    <link rel="stylesheet" type="text/css" href="../../../style/newStyles.css" />
    <link rel="stylesheet" type="text/css" href="../../../style/newMasterpage.css" />
</head>
<body leftmargin="0" topmargin="0" bottommargin="0" rightmargin="0">
    <form id="Form1" runat="server">

    <script language="javascript" type="text/javascript">
	
	    moveTo((screen.width - 1220) / 2, (screen.height - 870) / 2);
		resizeTo(1220, 870);
		window.focus();
		
		var returnUrlFromPopUp = window.location;

	     //Window Code
        function _showModalDialog(url, width, height, windowTitle)
        {
            MyClientSideAnchor.WindowHeight= height + "px";
            MyClientSideAnchor.WindowWidth= width + "px";
            
            MyClientSideAnchor.URL = url;
            MyClientSideAnchor.Title = windowTitle;
	        var returnvalue = MyClientSideAnchor.Open();
	        if (returnvalue == true)
	        {
	            document.all.Form1.submit();
		    }
	        return true;	        
        }

        function AddExtra()
        {
            var url = "../../job/AddExtra.aspx?jobId=" + <%# Request.QueryString["jobId"]%>;
            _showModalDialog(url, 400,320, 'Add Extra');
            
        }
		
        function UpdateExtra(extraId)
        {
            var url = "../../job/AddExtra.aspx?jobId=" + <%# Request.QueryString["jobId"]%> + "&extraId=" + extraId;
            _showModalDialog(url, 400,320, 'Update Extra');
            
        }

              
        function updateCost(el)
        {
            var cost        = new NumberFormat(el.value) 

            cost.setCurrencyPrefix('£');
            cost.setCurrency(true);
            el.value = cost.toFormatted();
            setTotalCost(el);
        }
        
         function updateRate(el)
        {
            var cost = new NumberFormat(el.value) 

            cost.setCurrencyPrefix('£');
            cost.setCurrency(true);
            el.value = cost.toFormatted();
        }
        
        function updateCharge(el)
        {
            var charge        = new NumberFormat(el.value) 
            
            charge.setCurrencyPrefix('£');
            charge.setCurrency(true);
            el.value = charge.toFormatted();
            setTotalCharge(el);
        }
        
        function updateRefusalCharge(el)
        {
            
            var charge        = new NumberFormat(el.value) 
            
            charge.setCurrencyPrefix('£');
            charge.setCurrency(true);
            el.value = charge.toFormatted();
            setTotalRefusalCharge(el);
        }
        
        function setTotalCost(el)
        {
           elm=el.form.elements;
            
           var lblTotal    = document.getElementById('<%# gvLegs.FooterRow.FindControl("lblTotalCost").ClientID %>');
           var runningTotal = 0;
           for(i=0;i<elm.length;i++)
           {
             if(elm[i].type=="text")
             {
                if (elm[i].name.indexOf("gvLegs") > -1 && elm[i].name.indexOf("txtCost")> -1)
                {
                   runningTotal = runningTotal + new NumberFormat(elm[i].value).toUnformatted();
                }
             }
            } 
           
           var ret = new NumberFormat(runningTotal);
           ret.setCurrencyPrefix('£');
           ret.setCurrency(true);
           lblTotal.innerText = ret.toFormatted();
           
         }
         
        function setTotalCharge(el)
        {
           elm=el.form.elements;
            
           var lblTotal    = document.getElementById('<%# gvLegs.FooterRow.FindControl("lblTotalCharge").ClientID %>');
           var runningTotal = 0;
           for(i=0;i<elm.length;i++)
           {
             if(elm[i].type=="text")
             {
                if (elm[i].name.indexOf("gvLegs") > -1 && elm[i].name.indexOf("txtCharge")> -1)
                {
                   runningTotal = runningTotal + new NumberFormat(elm[i].value).toUnformatted();
                }
             }
            } 
           
           var ret = new NumberFormat(runningTotal);
           ret.setCurrencyPrefix('£');
           ret.setCurrency(true);
           lblTotal.innerText = ret.toFormatted();
           
         }
         
        function setTotalRefusalCharge(el)
        {
               elm=el.form.elements;
           
               var lblTotal    = document.getElementById('<%# gvRefusals.Rows.Count > 0 ? gvRefusals.FooterRow.FindControl("lblTotalRefusalCharge").ClientID : ""   %>');
               if (lblTotal ==  null)
                return;
                
               var runningTotal = 0;
               for(i=0;i<elm.length;i++)
               {
                 if(elm[i].type=="text")
                 {
                    if (elm[i].name.indexOf("gvRefusals") > -1 && elm[i].name.indexOf("txtRefusalCharge")> -1)
                    {
                       runningTotal = runningTotal + new NumberFormat(elm[i].value).toUnformatted();
                    }
                 }
                } 
               
               var ret = new NumberFormat(runningTotal);
               ret.setCurrencyPrefix('£');
               ret.setCurrency(true);
               lblTotal.innerText = ret.toFormatted();
           
         }
	
    </script>

    <cs:WebModalAnchor ID="MyClientSideAnchor" Title="Sub-Contract Run" runat="server"
        ClientSideSupport="true" WindowWidth="580" WindowHeight="532" Scrolling="false"
        URL="addupdatedriver.aspx" HandledEvent="onclick" LinkedControlID="lblJobId">
    </cs:WebModalAnchor>
    <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="layoutWizzardHeaderOuter">
                <div class="layoutWizzardHeaderInner">
                    <p>
                        &nbsp;|&nbsp;</p>
                    <asp:Label ID="lblWizardTitle" runat="server">Pricing</asp:Label><div class="clearDiv">
                    </div>
                </div>
            </td>
        </tr>
        <tr>
            <td class="layoutContentTop">
                &nbsp;
            </td>
        </tr>
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div class="layoutContentMiddleInner">
                    <div class="buttonbar" style="text-align: left;">
                        <table border="0" cellpadding="0" cellspacing="2" width="100%">
                            <tr>
                                <td>
                                    <input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=<%# Request.QueryString["jobId"]%>'+ getCSID();" />
                                </td>
                                <td>
                                    <input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=<%# Request.QueryString["jobId"]%>'+ getCSID();" />
                                </td>
                                <td>
                                    <input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/Callin.aspx?wiz=true&jobId=<%# Request.QueryString["jobId"]%>'+ getCSID();" />
                                </td>
                                <td>
                                    <input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=<%# Request.QueryString["jobId"]%>'+ getCSID();" />
                                </td>
                                <td>
                                    <input type="button" style="width: 75px; display: <%# m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>"
                                        value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%# Request.QueryString["jobId"]%>'+ getCSID();" />
                                </td>
                                <td width="100%" align="right">
                                    <iframe marginheight="0" marginwidth="0" frameborder="no" scrolling="no" width="300px"
                                        height="22px" style="visibility: visible;" src='/traffic/jobManagement/CallInSelector.aspx?JobId=<%# Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'>
                                    </iframe>
                                </td>
                            </tr>
                        </table>
                    </div>
                    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
                        <div class="MessagePanel">
                            <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" />
                            <asp:Label ID="lblConfirmation" runat="server" Text="" CssClass="confirmation"></asp:Label>
                        </div>
                    </asp:Panel>
                    <uc1:infringementDisplay ID="infringementDisplay" runat="server" Visible="false">
                    </uc1:infringementDisplay>
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td style="vertical-align: top;">
                                <fieldset>
                                    <legend>Run Details</legend>
                                    <table>
                                        <tr>
                                            <td class="formCellLabel">
                                                Run Id
                                            </td>
                                            <td class="formCellField">
                                                <asp:Label ID="lblJobId" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">
                                                Customer
                                            </td>
                                            <td class="formCellField">
                                                <asp:Label ID="lblCustomer" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">
                                                Run Charge Type
                                            </td>
                                            <td class="formCellField">
                                                <asp:Label ID="lblJobType" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">
                                                Run Rate
                                            </td>
                                            <td class="formCellField">
                                                <asp:TextBox ID="txtRate" runat="server" CssClass="fieldInputBoxHighlight" onblur="updateRate(this);"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvRate" runat="server" ControlToValidate="txtRate"
                                                    ErrorMessage="Please supply a run rate."><img src="../../images/Error.gif" height="16" width="16" title="Please supply a run rate." /></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="cvRate" runat="server" Display="Dynamic" ControlToValidate="txtRate"
                                                    EnableClientScript="False" ErrorMessage="The run's pricing cannot be updated, as both the instruction charges and leg charges must both sum to the overall run charge."><img src="../../images/Error.gif" height="16" width="16" title="The run's pricing cannot be updated, as both the instruction charges and leg charges must both sum to the overall run charge." /></asp:CustomValidator>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:CheckBox ID="chkIsPriced" runat="server" Text="Set Run As Priced" />
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </td>
                            <td>
                            </td>
                            <td style="vertical-align: top; padding-left: 10px;">
                                <asp:Panel ID="pnlExtra" runat="server" Visible="True">
                                    <fieldset style="height: 151px; width: 280px;">
                                        <legend>Run Extras</legend>
                                        <table>
                                            <tr>
                                                <td>
                                                    <input type="button" class="buttonClass" onclick="javascript:AddExtra();" value="Add Extra"
                                                        id="btnAddExtra" runat="server" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td style="padding-top: 5px">
                                                    <asp:Label ID="lblNoExtras" runat="server" Text="There are no Extras" Visible="false"></asp:Label>
                                                    <asp:DataGrid ID="dgExtras" runat="server" AllowPaging="False" CssClass="Grid" CellPadding="3"
                                                        CellSpacing="0" GridLines="Both" AutoGenerateColumns="False" PagerStyle-HorizontalAlign="Right"
                                                        PagerStyle-Mode="NumericPages" PageSize="5" ShowFooter="false">
                                                        <SelectedItemStyle CssClass="SelectedRow"></SelectedItemStyle>
                                                        <AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
                                                        <ItemStyle ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ItemStyle>
                                                        <HeaderStyle CssClass="HeadingRow" Height="22" VerticalAlign="middle"></HeaderStyle>
                                                        <Columns>
                                                            <asp:TemplateColumn HeaderText="Type">
                                                                <ItemTemplate>
                                                                    <a href="javascript:UpdateExtra(<%# DataBinder.Eval(Container.DataItem, "ExtraId") %>)">
                                                                        <asp:Label ID="lblExtraType" runat="server" Text='<%# Orchestrator.WebUI.Utilities.UnCamelCase(DataBinder.Eval(Container.DataItem, "ExtraType").ToString()) %>'></asp:Label></a>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn Visible="False" HeaderText="Custom Description">
                                                                <ItemTemplate>
                                                                    <asp:Label Visible="False" ID="lblCustomDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomDescription") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="State">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblExtraState" runat="server" Text='<%# Orchestrator.WebUI.Utilities.UnCamelCase(DataBinder.Eval(Container.DataItem, "ExtraState").ToString()) %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:TemplateColumn HeaderText="Contact">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="lblClientContact" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ClientContact") %>'></asp:Label>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:BoundColumn HeaderText="Amount" DataField="ExtraAmount" DataFormatString="{0:C}">
                                                            </asp:BoundColumn>
                                                        </Columns>
                                                    </asp:DataGrid>
                                                </td>
                                            </tr>
                                        </table>
                                    </fieldset>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="3">
                            </td>
                        </tr>
                        <tr valign="top">
                            <td colspan="3" style="vertical-align: top;">
                                <fieldset>
                                    <legend>Price Legs</legend>
                                    <asp:GridView ID="gvLegs" runat="server" CssClass="Grid" CellPadding="3" CellSpacing="0"
                                        AllowSorting="false" AutoGenerateColumns="false" ShowFooter="true">
                                        <HeaderStyle CssClass="HeadingRowLite" Height="22" VerticalAlign="middle" />
                                        <FooterStyle Height="22" />
                                        <RowStyle CssClass="Row" />
                                        <AlternatingRowStyle BackColor="WhiteSmoke" />
                                        <SelectedRowStyle CssClass="SelectedRow" />
                                        <Columns>
                                            <asp:TemplateField HeaderText="Start Time">
                                                <ItemTemplate>
                                                    <%# ((Orchestrator.Entities.LegView) Container.DataItem).StartLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="End Time">
                                                <ItemTemplate>
                                                    <%# ((Orchestrator.Entities.LegView) Container.DataItem).EndLegPoint.PlannedDateTime.ToString("dd/MM/yy HH:mm") %>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Driver">
                                                <ItemTemplate>
                                                    <%# m_job.HasBeenSubContracted == true ? "Subby" : ((Orchestrator.Entities.LegView)Container.DataItem).Driver.Individual.FullName%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Vehicle">
                                                <ItemTemplate>
                                                    <%# m_job.HasBeenSubContracted == true ? "Subby" : ((Orchestrator.Entities.LegView)Container.DataItem).Vehicle.RegNo%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Trailer">
                                                <ItemTemplate>
                                                    <%# m_job.HasBeenSubContracted == true ? "Subby" : ((Orchestrator.Entities.LegView)Container.DataItem).Trailer != null ? ((Orchestrator.Entities.LegView)Container.DataItem).Trailer.TrailerRef : string.Empty%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="From">
                                                <ItemTemplate>
                                                    <%# ((Orchestrator.Entities.LegView)Container.DataItem).StartLegPoint.Point.Description%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="To">
                                                <ItemTemplate>
                                                    <%# ((Orchestrator.Entities.LegView)Container.DataItem).EndLegPoint.Point.Description%>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Cost">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtCost" runat="server" CssClass="fieldInputBoxHighlight" Width="75px"
                                                        Text='<%# ((Orchestrator.Entities.LegView)Container.DataItem).EndLegPoint.Instruction.Cost.ToString("C") %>'></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfvCost" runat="server" Display="Dynamic" ControlToValidate="txtCost"
                                                        ErrorMessage="Please supply a cost."><img src="../../images/Error.gif" height="16" width="16" title="Please supply a cost." /></asp:RequiredFieldValidator>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label ID="lblTotalCost" runat="server" Text="£0.00"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Charge">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtCharge" runat="server" CssClass="fieldInputBoxHighlight" Width="75px"
                                                        Text='<%# ((Orchestrator.Entities.LegView)Container.DataItem).EndLegPoint.Instruction.Charge.ToString("C") %>'></asp:TextBox>
                                                    <asp:RequiredFieldValidator ID="rfcCharge" runat="server" Display="Dynamic" ControlToValidate="txtCharge"
                                                        ErrorMessage="Please supply a cost."><img src="../../images/Error.gif" height="16" width="16" title="Please supply a cost." /></asp:RequiredFieldValidator>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label ID="lblTotalCharge" runat="server" Text="£0.00"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </fieldset>
                            </td>
                            <td style="vertical-align: top; padding-left: 10px;">
                                <fieldset runat="server" id="fsRefusals">
                                    <legend>Refusals</legend>
                                    <asp:ObjectDataSource ID="odsRefusals" runat="server" SelectMethod="GetRefusalsForReturnJob"
                                        TypeName="Orchestrator.Facade.GoodsRefusal"></asp:ObjectDataSource>
                                    <asp:GridView ID="gvRefusals" runat="server" CssClass="Grid" CellPadding="3" CellSpacing="0"
                                        AllowSorting="false" AutoGenerateColumns="false" ShowFooter="true" DataSourceID="odsRefusals">
                                        <HeaderStyle CssClass="HeadingRowLite" Height="22" VerticalAlign="middle" />
                                        <FooterStyle Height="22" />
                                        <RowStyle CssClass="Row" />
                                        <AlternatingRowStyle BackColor="WhiteSmoke" />
                                        <SelectedRowStyle CssClass="SelectedRow" />
                                        <Columns>
                                            <asp:BoundField DataField="ProductName" HeaderText="Product" />
                                            <asp:BoundField DataField="PackSize" HeaderText="Size" />
                                            <asp:BoundField DataField="ProductCode" HeaderText="Code" />
                                            <asp:BoundField DataField="RefusalReceiptNumber" HeaderText="Receipt" />
                                            <asp:BoundField DataField="RefusalNotes" HeaderText="Notes" />
                                            <asp:TemplateField HeaderText="Charge">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtRefusalCharge" runat="server" Width="75px" Text='<%# decimal.Parse(DataBinder.Eval(Container.DataItem, "RefusalCharge").ToString()).ToString("C") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                    <asp:Label ID="lblTotalRefusalCharge" runat="server" Width="75px" Text="£0.00"></asp:Label>
                                                </FooterTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </fieldset>
                            </td>
                        </tr>
                    </table>
                    <div class="buttonbar">
                        <asp:Button ID="btnUpdate" runat="server" Text="Update" Width="75"></asp:Button>
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
                    </div>
                </div>
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
