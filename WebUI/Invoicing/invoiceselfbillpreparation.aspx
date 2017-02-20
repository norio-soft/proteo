<%@ Page Language="c#" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.Invoicing.InvoiceSelfBillPreparation" Codebehind="InvoiceSelfBillPreparation.aspx.cs" %>

<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>
<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Self Bill Invoice Preparation</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript">
        function checkAll(el)
  	    {
      	    
  	        var col = document.getElementsByName("INPUT");
  	        //alert(col.length);
          	
      	    if (el.checked)
      	        {
     	               for (i = 0; i < col.length; i++) 
     	               {
     	                //alert(col.name);
      	                if (col[i].name.indexOf("dvJobs")> -1)
                            col[i].checked = true;
                      }
     	        }
      	        else
      	        {
      	            for (i = 0; i < col.length; i++) 
      	               {
      	                if (col[i].name.indexOf("dvJobs")> -1)
                            col[i].checked = false;
                       }
     	        }
  	    }
      	
        function onUpdate(oldItem, newItem)
        {
             // validate ExtraState
            var extraAmount = newItem.GetMember('ExtraAmount').Value;
          
            if(isNaN(parseFloat(extraAmount)))
            {
              // failed validation, return to editing
              alert('Extra Amount must be a number.');
              return 2;
            }
            else
            {
                return 1; 
                
            }
        }
      
        function editGrid(rowId)
        {
            dgExtras.Edit(dgExtras.GetRowFromClientId(rowId)); 
        }

        function editRow()
        {
            dgExtras.EditComplete();     
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    
    <h2>The Invoice preparation summary is below</h2>
    <asp:label id="lblJobCount" runat="server" width="100%" font-size="Medium"></asp:label>
    <asp:label id="lblDetails" runat="server" width="100%"></asp:label>
    <asp:label id="lblExtraDetails" runat="server" width="100%"></asp:label>
    <input type="hidden" id="hidJobCount" runat="server" value="0" />
    <input type="hidden" id="hidJobTotal" runat="server" value="0" />
    <input type="hidden" id="hidExtraCount" runat="server" value="0" />
    <input type="hidden" id="hidExtraTotal" runat="server" value="0" />
    <input type="hidden" id="hidSelectedJobs" runat="server" value="" />
    <input type="hidden" id="hidSelectedExtras" runat="server" value="" />
    
    <fieldset>
        <legend>Main Filter</legend>
        <table width="100%" border="0" cellpadding="0" cellspacing="0">
            <tr>
                <td class="formCellLabel"><asp:label id="lblClient" text="Client" runat="server"></asp:label></td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" Overlay="true" 
                        runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" AllowCustomText="false"
                        ShowMoreResultsBox="false" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" SelectOnTab="false"
                        Width="355px" Height="300px" TabIndex="1">
                    </telerik:RadComboBox>
                </td>
                <td class="formCellField" style="vertical-align: top;" rowspan="99" width="40%">
                    <asp:panel id="pnlFilterOptions" runat="server" visible="true">
		                <fieldset>
			                <legend>Filter Options</legend>
				            <asp:label id="lblSaveProgressNotification" runat="server" Visible="true"></asp:label>
				            <nfvc:noformvalbutton id="btnLoadFilter" runat="server" visible="true" text="Load Filter" ServerClick="btnLoadFilter_Click" NoFormValList="rfvClient" CssClass="buttonClass"></nfvc:noformvalbutton>
				            <nfvc:noformvalbutton id="btnSaveFilter" runat="server" visible="false" text="Save Current Filter" ServerClick="btnSaveFilter_Click" CssClass="buttonClass"></nfvc:noformvalbutton>
				            <nfvc:noformvalbutton id="btnClearFilter" runat="server" visible="false" text="Clear Filter" ServerClick="btnClear_Click" CssClass="buttonClass"></nfvc:noformvalbutton>
    	                </fieldset>
	                </asp:panel>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Date From</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="dteStartDate" runat="server" Width="100" tabindex="2"> 
                    <DateInput runat="server"
                    dateformat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" Display="Dynamic" ErrorMessage="Please supply a start date.">
                        <img src="/images/error.gif" width="16" height="16" alt="Please supply a start date." />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel"><asp:label id="lblJobState" text="Client" runat="server" visible="false"></asp:label></td>
                <td class="formCellField">
                    <asp:dropdownlist id="cboJobState" runat="server" visible="false"></asp:dropdownlist>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Date To</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="dteEndDate" runat="server" Width="100" tabindex="3"> 
                    <DateInput runat="server"
                    dateformat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                    <asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" Display="Dynamic" ErrorMessage="Please supply an end date.">
                        <img src="/images/error.gif" width="16" height="16" alt="Please supply an end date." />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel"><asp:label id="lblSelfOnHold" runat="server" visible="false" cssclass="confirmation" text=""></asp:label></td>
            </tr>
        </table>
    </fieldset>
    
    <div class="buttonbar" align="left">
        <nfvc:NoFormValButton ID="btnFilter2" runat="server" Text="Generate Runs To Invoice"
            ServerClick="btnFilter_Click"></nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnClear2" runat="server" Text="Clear" ServerClick="btnClear_Click">
        </nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnCreateInvoice2" runat="server" Visible="false" Text="Create Invoice"
            ServerClick="btnCreateInvoice_Click"></nfvc:NoFormValButton>
        <nfvc:noformvalbutton id="btnExport" visible="false" runat="server" text="Export to CSV" ServerClick="btnExport_Click"></nfvc:noformvalbutton>
    </div>
    <fieldset>
	    <legend>Self Bill Runs To Invoice</legend>
		<table width="100%">
		    <tr valign="top">
		        <td align="right">
		            <p><asp:CheckBox id="chkSelfMarkAll" runat="server" visible="false" Text="Un/Mark All" AutoPostBack="True" TextAlign="Left" Font-Bold="True"></asp:CheckBox></p>
                </td>
	            <td align="left">
                    <p><asp:Checkbox id="chkOnlyShowTicked" runat="server" Text="Only Ticked Jobs" visible="False" TextAlign="Left" AutoPostBack="True" Font-Bold="True"></asp:Checkbox></p>
                </td>
	        </tr>
        </table>
	    <asp:GridView ID="dvJobs" runat="server" AllowSorting="true" gridlines="vertical" autogeneratecolumns="false" cssclass="Grid" width="100%" visible="true">
            <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle"/>
            <rowStyle height="20" cssclass="Row" />
            <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
            <SelectedRowStyle height="20" cssclass="SelectedRow" />
            <columns>
                <asp:templatefield headerText="">
                    <HeaderTemplate>
                        <input id="chkAll" onclick="javascript:SelectAllCheckboxes(this);" runat="server" type="checkbox" />
                    </HeaderTemplate>
                    <itemtemplate>
                        <asp:checkbox id="chkSelect" runat="server" ></asp:checkbox>
                    </itemtemplate>
                </asp:templatefield>
                <asp:boundfield datafield="OrganisationName" HeaderText="Customer" visible="true" />
                <asp:boundfield datafield="IdentityId" HeaderText="ID" visible="false"/>
                <asp:boundfield datafield="LoadNo" HeaderText="Load" sortexpression="LoadNo"/>
                <asp:boundfield datafield="DocketNumbers" HeaderText="Docket" SortExpression="DocketNumbers" />
                <asp:boundfield datafield="ChargeAmount" HeaderText="Charge" DataFormatString="{0:C}" htmlencode="false" />
                <asp:templatefield headertext="Delivery">
                    <itemtemplate>
                        <%# Eval("Customers").ToString().Replace(Environment.NewLine, "<br/>") %>    
                    </itemtemplate>
                </asp:templatefield>
                <asp:templatefield headertext="Job Id">
                    <itemtemplate>
                        <INPUT id="hidJobId" type=hidden value='<%# DataBinder.Eval(Container.DataItem, "JobId") %> ' name=hidJobId runat="server">
                         <a href="javascript:openDialogWithScrollbars('~/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%# Eval("JobId").ToString() %>'+ getCSID(), '800','600');">
				    <%# Eval("JobId").ToString() %>
			    </a>
                    </itemtemplate>
                </asp:templatefield>
                <asp:boundfield datafield="CollectionPoint" HeaderText="Collection" />
                <asp:boundfield datafield="CompleteDate" HeaderText="Date Completed" DataFormatString="{0:dd/MM HH:mm}"  htmlencode="false" itemstyle-width="100" />
                <asp:boundfield datafield="ChargeType" HeaderText="Type" />
                <asp:boundfield datafield="References" HeaderText="References" />
            </columns>
        </asp:GridView>         
        <!-- NOTE: repDeliveryPoints ... Could be firstDeliveryPoint in main dataset if MR T is not happy with this one --> 
	</fieldset>       
    <!-- EXTRAS FOR SELF BILL INVOICE -->
    <asp:panel id="pnlExtraFilter" runat="server" visible="False">
		<fieldset>
		<legend><strong>Extras</strong></legend>
		<table>
		    <tr>
                <td valign="top" >
                    <fieldset>
	                    <legend><strong>Extra Filter</strong></legend>
	                    <table>
		                    <tr>
			                    <td class="formCellLabel">Job Id</td>
			                    <td class="formCellField"><asp:TextBox id="txtExtraJobId" runat="server"/></td>
			                    <td class="formCellLabel">Date from</td>
			                    <td class="formCellField"><telerik:RadDatePicker id="dteExtraDateFrom" runat="server" Width="100">
                                <DateInput runat="server"
                                dateformat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker></td>
		                    </tr>
		                    <tr>
			                    <td class="formCellLabel">Extra type</td>
			                    <td class="formCellField"><asp:DropDownList ID="cboExtraType" Runat="server"/></td>
			                    <td class="formCellLabel">Date to</td>
			                    <td class="formCellField"><telerik:RadDatePicker id="dteExtraDateTo" runat="server" Width="100">
                                <DateInput runat="server"
                                dateformat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker></td>
		                    </tr>
		                    <tr>
			                    <td class="formCellLabel">Extra state</td>
			                    <td class="formCellField"><asp:DropDownList id="cboSelectExtraState" Runat="server"/></td>
			                    <td colspan="2" align="right">
				                    <asp:Button id="btnFilterExtras" runat="server" Text="Filter"/>
			                    </td>
		                    </tr>
	                    </table>
                    </fieldset>
			    </td>
		        <td align="right" valign="top" >
		            <asp:Panel id="pnlExtras" runat="server" Visible="False">
		                <fieldset>
		                    <legend>Include extras to substitute for jobs</legend>		
		                    <table>
		                        <tr>
        	                        <td align =right>
                                      <cc1:Dialog ID="dlgExtra" runat="server" Width="400" Height="320" Mode="Modal" URL="/job/addextra.aspx" 
                                        AutoPostBack="true" ReturnValueExpected="true" >
                                        </cc1:Dialog>
                                        <asp:GridView ID="dvExtras" runat="server" AllowSorting="true" gridlines="vertical" autogeneratecolumns="false" cssclass="Grid" width="100%" visible="true">
                                            <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle"/>
                                            <rowStyle height="20" cssclass="Row" />
                                            <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
                                            <SelectedRowStyle height="20" cssclass="SelectedRow" />
                                            <columns>
                                                <asp:templatefield headerText="Inc.">
                                                    <itemtemplate>
                                                        <asp:checkbox id="chkIncludeExtra" runat="server" ></asp:checkbox>
                                                    </itemtemplate>
                                                </asp:templatefield>
                                                <asp:templatefield headertext="Job Id">
                                                    <itemtemplate>
                                                        <a href="javascript:openDialogWithScrollbars('~/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%# Eval("JobId").ToString() %>'+ getCSID(), '800','600');"><%# Eval("JobId").ToString() %></a>
                                                    </itemtemplate>
                                                </asp:templatefield>
                                                <asp:templatefield headertext="Extra Type">
                                                    <itemtemplate>
                                                        <a href="javascript:UpdateExtra('<%# Eval("JobId").ToString() %>', '<%# Eval("ExtraId").ToString() %>', '<%# Eval("InstructionId").ToString() %>');"><%# Eval("ExtraType").ToString() %></a>
                                                    </itemtemplate>
                                                </asp:templatefield>
                                                <asp:boundfield headertext="Extra Type" datafield="ExtraType" />
                                                <asp:boundfield headertext="Description" datafield="CustomDescription" />
                                                <asp:boundfield headertext="Extra State" datafield="ExtraState" />
                                                <asp:boundfield headertext="Client Contact" datafield="ClientContact" />
                                                <asp:boundfield headertext="Extra Amount" datafield="ExtraAmount" htmlencode="false" DataFormatString="{0:C}" />
                                                <asp:boundfield headertext="Load/Docket" datafield="Load_Number" />
                                                <asp:boundfield headertext="Delivery Date" datafield="DeliveryDate" htmlencode="false" DataFormatString="{0:dd/MM/yy HH:mm} " />
                                            </columns>
                                        </asp:GridView>
			                        </td>
			                    </tr>
		                    </table>
		                </fieldset> 	
                    </asp:Panel>
                </td> 
            </tr> 
        </table> 
    </fieldset>
	</asp:panel>
    <!-- END OF THE EXTRAS SECTION -->
    <div class="buttonbar">
        <nfvc:NoFormValButton ID="btnFilter" runat="server" Text="Generate Runs To Invoice"
            ServerClick="btnFilter_Click"></nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnClear" runat="server" Text="Clear" ServerClick="btnClear_Click">
        </nfvc:NoFormValButton>
        <nfvc:NoFormValButton ID="btnCreateInvoice" runat="server" Visible="false" Text="Create Invoice"
            ServerClick="btnCreateInvoice_Click"></nfvc:NoFormValButton>
    </div>
<telerik:RadCodeBlock runat="server">
<script language="javascript">
<!--
	var message = "You have selected ";
	var message1 = " job(s), and the total amount is £";
	var message2 = " extra(s), and the total amount is £";
	var hidSelectedJobs = document.getElementById('<%=hidSelectedJobs.ClientID %>');
	var hidSelectedExtras = document.getElementById('<%=hidSelectedExtras.ClientID %>');
    var hidJobCount = document.getElementById('<%=hidJobCount.ClientID %>');
	var hidJobTotal = document.getElementById('<%=hidJobTotal.ClientID %>');
    var hidExtraCount = document.getElementById('<%=hidExtraCount.ClientID %>');
	var hidExtraTotal = document.getElementById('<%=hidExtraTotal.ClientID %>');

   function GetCheckedItems(gridItem, index, checkBoxElement)
    {
        amount = gridItem.Cells[6].Value;
        jobId = gridItem.Cells[8].Value;
        // Check the current CheckBox that we have just dealt with 
        if (checkBoxElement.checked)
        {
            // We've selected this item
            hidJobCount.value++;
	        hidJobTotal.value = parseFloat(hidJobTotal.value) + parseFloat(amount, 10);
            hidSelectedJobs.value = hidSelectedJobs.value + jobId + ",";
        }
        else
        {
            // We've deselected this item
            hidJobCount.value--;
			hidJobTotal.value = parseFloat(hidJobTotal.value) - parseFloat(amount);
			
		
			if (hidSelectedJobs.value.substr(0, (new String(jobId).length) + 1) == jobId + ",")
			{
			    if (hidSelectedJobs.value == (jobId + ","))
			    {
			        hidSelectedJobs.value = "";
			    }
			    else
			    {
			        hidSelectedJobs.value = hidSelectedJobs.value.substr((new String(jobId).length) + 1);
			    }
		    }
			else
			{
			    var location = hidSelectedJobs.value.indexOf("," + jobId + ",");
                hidSelectedJobs.value = hidSelectedJobs.value.substr(0, location + 1) + hidSelectedJobs.value.substr(location + ("," + jobId + ",").length);
			}
        }
        hidJobTotal.value  = Math.round(hidJobTotal.value * 100)/100;

		var text = message + hidJobCount.value + message1 + hidJobTotal.value;
	
		setLabelText("lblDetails", text);

        return true;
    }

	function GetCheckedItemsForExtras(gridItem, index, checkBoxElement)
	{
        extraId = gridItem.Cells[1].Value;
    	hidSelectedExtras.value = "";
	 
	    if (checkBoxElement.checked)
	    {
	       hidSelectedExtras.value = hidSelectedExtras.value + extraId + ",";
	    }
	    
        var gridItem1;
        var itemIndex = 0;
        
        while(gridItem1 = dgExtras.Table.GetRow(itemIndex))
        {
            checked = gridItem1.Cells[0].Value;
            extraId = gridItem1.Cells[1].Value;
            
            if(checked) // If checked
            {
	            hidSelectedExtras.value = hidSelectedExtras.value + extraId + ",";
            }
           
           itemIndex++;
        }
        
        return true;
	}
    
    function setLabelText(ID, Text)
  	{
  	  	document.getElementById(ID).innerHTML = Text;
  	}
  	
  	function showMenu(e)
  	{
  	    if (event.button == 2)
  	        alert("right click menu");
  	}
  	
  	function selectItem(jobId, chargeAmount, el)
  	{
  	
  	    if (el.checked)
  	    {
  	         // We've selected this item
            hidJobCount.value++;
	        hidJobTotal.value = parseFloat(hidJobTotal.value) + parseFloat(chargeAmount, 10);
            hidSelectedJobs.value = hidSelectedJobs.value + jobId + ",";
  	    }
  	    else
  	    {
  	        // We've deselected this item
            hidJobCount.value--;
			hidJobTotal.value = parseFloat(hidJobTotal.value) - parseFloat(chargeAmount, 10);
						
			if (hidSelectedJobs.value.substr(0, (new String(jobId).length) + 1) == jobId + ",")
			{
			    if (hidSelectedJobs.value == (jobId + ","))
			    {
			        hidSelectedJobs.value = "";
			    }
			    else
			    {
			        hidSelectedJobs.value = hidSelectedJobs.value.substr((new String(jobId).length) + 1);
			    }
		    }
			else
			{
			    var location = hidSelectedJobs.value.indexOf("," + jobId + ",");
                hidSelectedJobs.value = hidSelectedJobs.value.substr(0, location + 1) + hidSelectedJobs.value.substr(location + ("," + jobId + ",").length);
			}
//			var str = hidSelectedJobs.value;
//			str = str.replace(jobId, "");
//			str = str.replace(",,", ",");
//			
//			hidSelectedJobs.value = str;
  	    }
  	    hidJobTotal.value  = Math.round(hidJobTotal.value * 100)/100;

		var text = message + hidJobCount.value + message1 + hidJobTotal.value;
	
		setLabelText("lblDetails", text);
  	}

  	function countSelectedExtras(el, extraId, extraAmount)
  	{
  	    if (el.checked)
  	    {
  	         // We've selected this item
            hidExtraCount.value++;
	        hidExtraTotal.value = parseFloat(hidExtraTotal.value) + parseFloat(extraAmount, 10);
            hidSelectedExtras.value = hidSelectedExtras.value + extraId + ",";
  	    }
  	    else
  	    {
  	        // We've deselected this item
            hidExtraCount.value--;
			hidExtraTotal.value = parseFloat(hidExtraTotal.value) - parseFloat(extraAmount, 10);
						
			if (hidSelectedExtras.value.substr(0, (new String(extraId).length) + 1) == extraId + ",")
			{
			    if (hidSelectedExtras.value == (extraId + ","))
			    {
			        hidSelectedExtras.value = "";
			    }
			    else
			    {
			        hidSelectedExtras.value = hidSelectedExtras.value.substr((new String(extraId).length) + 1);
			    }
		    }
			else
			{
			    var location = hidSelectedExtras.value.indexOf("," + extraId + ",");
                hidSelectedExtras.value = hidSelectedExtras.value.substr(0, location + 1) + hidSelectedExtras.value.substr(location + ("," + extraId + ",").length);
			}
  	    }
  	    hidExtraTotal.value  = Math.round(hidExtraTotal.value * 100)/100;

		var text = message + hidExtraCount.value + message2 + hidExtraTotal.value;
	
		setLabelText("lblExtraDetails", text);
  	}  	
  	
 function SelectAllCheckboxes(spanChk)
 {

   var oItem = spanChk.children;
   var theBox= (spanChk.type=="checkbox") ? spanChk : spanChk.children.item[0];
   xState=theBox.checked;
   
   elm=theBox.form.elements;

   for(i=0;i<elm.length;i++)
     if(elm[i].type=="checkbox" && elm[i].id!=theBox.id)
     {
        if (elm[i].name.indexOf("dvJobs")> -1)
        {
           //elm[i].click();
           if(elm[i].checked!=xState)
             elm[i].click();
           //elm[i].checked=xState;
        }
     }
 }
 	var lastHighlightedRow = "";
	var lastHighlightedRowColour = "";
	
 	function HighlightRow(row)
	{
		var rowElement;
		
		if (lastHighlightedRow != "")
		{
			rowElement = document.getElementById(lastHighlightedRow);
			rowElement.style.backgroundColor = lastHighlightedRowColour;
		}

		rowElement = document.getElementById(row);
		lastHighlightedRow = row;
		lastHighlightedRowColour = rowElement.style.backgroundColor;
		rowElement.style.backgroundColor = 'yellow';
	}
	
     //Window Code
//    function _showModalDialog(url, width, height, windowTitle)
//    {
//        MyClientSideAnchor.WindowHeight= height + "px";
//        MyClientSideAnchor.WindowWidth= width + "px";
//        
//        MyClientSideAnchor.URL = url;
//        MyClientSideAnchor.Title = windowTitle;
//        var returnvalue = MyClientSideAnchor.Open();
//        if (returnvalue == true)
//        {
//            document.all.Form1.submit();
//	    }
//        return true;	        
//    }
    
    
    function UpdateExtra(jobId, orderId, extraId, instructionId)
    {
        var qs = "jobId=" + jobId + "&orderId=" + orderId + "&extraId=" + extraId;
        if (instructionId > 0)
            qs += "&instructionId=" + instructionId;
        
        <%=dlgExtra.ClientID %>_Open(qs);
    }
    
    -->
</script>
</telerik:RadCodeBlock>
</asp:Content>

