<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.SubContractorSB.InvoiceExtraPreparation" MasterPageFile="~/default_tableless.Master" Codebehind="InvoiceExtraPreparation.aspx.cs" %>

<%@ Register assembly="Orchestrator.WebUI.Dialog" namespace="Orchestrator.WebUI" tagprefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

	<telerik:RadCodeBlock runat="server">
	    <script type="text/javascript">
    	    
        function onUpdate(oldItem, newItem)
        {
             // validate ExtraState
            var extraAmount = newItem.GetMember('ForeignAmount').Value;
          
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
    </telerik:RadCodeBlock>
    <h1>Invoice Extra Preparation
    </h1>
    <h2></h2>
    <cc1:Dialog ID="dlgExtra" runat="server" Width="400" Height="320" Mode="Modal" URL="/job/addextra.aspx" 
    AutoPostBack="true" ReturnValueExpected="true" >
    </cc1:Dialog>
    <cc1:Dialog ID="dlgOrder" runat="server" Width="1180" Height="900" Mode="Normal" URL="/groupage/manageorder.aspx" >
    </cc1:Dialog>
    <input type="hidden" id="hidSelectedExtras" runat="server" value="" />
    <input type="hidden" id="hidSelectedOrders" runat="server" value="" />
    <input type="hidden" id="hidExtraCount" runat="server" value="0" />
    <input type="hidden" id="hidExtraTotal" runat="server" value="0" />
    <asp:label id="lblExtraDetails" runat="server" width="100%"></asp:label>
    <fieldset>
		<legend>Extra Filtering Options</legend>
		<table>
		    <tr>
			    <td class="formCellLabel">Sub Contractor</td>
			    <td class="formCellInput">
			       <telerik:RadComboBox ID="cboSubContractor" Overlay="true" height="400px" ZIndex="500" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" SelectOnTab="false" ></telerik:RadComboBox>
			    </td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Order Id</td>
			    <td class="formCellInput"><asp:TextBox id="txtJobId" runat="server" /></td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Extra type</td>
			    <td class="formCellInput"><asp:DropDownList ID="cboExtraType" Runat="server" /></td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Extra state</td>
			    <td class="formCellInput"><asp:DropDownList id="cboSelectExtraState" Runat="server" /></td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Date from</td>
			    <td class="formCellInput"><telerik:RadDateInput id="dteDateFrom" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Date to</td>
			    <td class="formCellInput">
				    <telerik:RadDateInput id="dteDateTo" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput>&nbsp;<asp:CustomValidator id="cfvDateTo" runat="server" ControlToValidate="dteDateTo" ErrorMessage="Date To should fall after Date From"
					    OnServerValidate="cfvDateTo_ServerValidate"><img src="../images/Error.gif" height="16" width="16" title="Date from should fall before date to." /></asp:CustomValidator>
			    </td>
		    </tr>
	    </table>
	</fieldset>
	<div class="buttonbar">
	    <asp:Button id="btnFilter" runat="server" Text="Get Extras To Invoice" />
		<asp:Button id="btnExport" visible="false" runat="server" Text="Export To CSV" />
	</div>
    <fieldset>
		<legend>
			<strong>Extras to include in invoice</strong></legend>
		<table width="100%" border="0" cellpadding="1" cellspacing="0">
			<tr>
				<td width="100%">
				    <asp:GridView ID="gvExtras" runat="server" EnableViewState="true" AllowSorting="true" gridlines="vertical" autogeneratecolumns="false" cssclass="Grid" width="100%" visible="true">
                    <headerstyle cssclass="HeadingRow" height="25" verticalalign="middle" wrap="false" />
                    <rowStyle height="20" cssclass="Row" />
                    <AlternatingRowStyle height="20" backcolor="WhiteSmoke" />
                    <SelectedRowStyle height="20" cssclass="SelectedRow" />
                    <columns>
                        <asp:templatefield headerText="Inc.">
                            <itemtemplate>
                                <asp:checkbox id="chkIncludeExtra" runat="server" ></asp:checkbox>
                            </itemtemplate>
                        </asp:templatefield>
                        <asp:boundfield headertext="Load/Docket" datafield="Load_Number" />
                        <asp:templatefield headertext="Order&nbsp;Id&nbsp;&nbsp;">
                            <itemtemplate>
                                <a id="lnkViewOwner" runat="server"></a>
                            </itemtemplate>
                        </asp:templatefield>
                        <asp:templatefield headertext="Extra Type">
                            <itemtemplate>
                                <a href="javascript:UpdateExtra('<%# Eval("JobId").ToString() %>', '<%# Eval("OrderId").ToString() %>', '<%# Eval("ExtraId").ToString() %>', '<%# Eval("InstructionId").ToString() %>');"><%# Eval("ExtraType").ToString() %></a>
                            </itemtemplate>
                        </asp:templatefield>
                        <asp:boundfield headertext="Description" datafield="CustomDescription" ItemStyle-Wrap="true" HeaderStyle-Wrap="False" />
                        <asp:boundfield headertext="Extra State" datafield="ExtraState" />
                        <asp:boundfield headertext="Client" datafield="OrganisationName" />
                        <asp:boundfield headertext="Client Contact" datafield="ClientContact" />
                        <asp:templatefield headertext="Extra Amount">
                            <itemtemplate>
                                <asp:Label id="extraAmountCurrencyLabel" runat="server" text=""></asp:Label>
                            </itemtemplate>
                        </asp:templatefield>
                        <asp:boundfield headertext="Delivery Date" datafield="DeliveryDate" htmlencode="false" DataFormatString="{0:dd/MM/yy HH:mm}" ItemStyle-Wrap="true" HeaderStyle-Wrap="False"/>
                    </columns>
                </asp:GridView>
				
					<asp:label id="lblExtrasOnHold" runat="server" visible="false" cssclass="confirmation" text=""></asp:label>
				</td>
			</tr>
		</table>
	</fieldset>
	<div class="buttonbar">
		<asp:Button id="btnCreateInvoice" runat="server" Text="Create Invoice" visible="False" />
		<asp:Label id="sameCurrencyValidationLabel" runat="server" style="color:red;"></asp:Label>
	</div>
	
<telerik:RadCodeBlock runat="server">
<script type="text/javascript">
    <!--//
	var hidSelectedExtras = document.getElementById('<%=hidSelectedExtras.ClientID %>');
	var message = "You have selected ";
    var message2 = " extra(s), and the total amount is ";
	var hidExtraCount = document.getElementById('<%=hidExtraCount.ClientID %>');
	var hidExtraTotal = document.getElementById('<%=hidExtraTotal.ClientID %>');		
	
    function setLabelText(ID, Text)
  	{
  	  	document.getElementById(ID).innerHTML = Text;
  	}
  	
    function openOrderProfile(orderID)
    {
        var qs = "oId=" + orderID;
        <%=dlgOrder.ClientID %>_Open(qs);
    }
    
    
  	function UpdateExtra(jobId, orderId, extraId, instructionId)
    {
        var qs = "jobId=" + jobId + "&orderId=" + orderId + "&extraId=" + extraId;
        if (instructionId > 0)
            qs += "&instructionId=" + instructionId;
        
        <%=dlgExtra.ClientID %>_Open(qs);
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
		setLabelText("<%=lblExtraDetails.ClientID %>", text);
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
	    
	if(<%=Page.IsPostBack.ToString().ToLower()%> || <%= Request.QueryString["edit"]=="true" ? "true" : "false" %>)
	    {
	    // hidExtraTotal.value  = Math.round(hidExtraTotal.value * 100)/100;
		var text = message + hidExtraCount.value + message2 + hidExtraTotal.value;
		setLabelText("<%=lblExtraDetails.ClientID %>", text);
	    }
        //-->
</script>
</telerik:RadCodeBlock>
</asp:Content>