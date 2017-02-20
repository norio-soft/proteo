<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage_DeliveryJob" EnableViewStateMac="false" MasterPageFile="~/default_tableless.master" Title="Create Delivery Run" Codebehind="DeliveryJob.aspx.cs" %>
<%@ PreviousPageType VirtualPath="~/Groupage/Deliveries.aspx" %>

<%@ Register TagPrefix="uc" TagName="Point" Src="~/UserControls/Point.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <style type="text/css">
        .masterpage_layout 
        {
        	width: 1700px;
        }
    </style>
    <asp:HiddenField ID="hidIsUpdatePoint" runat="server" Value='' />                  
    <asp:HiddenField ID="hidIsOrderChange" runat="server" Value='' /> 
    <script type="text/javascript" language="javascript" src="../script/tooltippopups.js"></script>
    <h1>Delivery Job Details</h1>       
    <h2>For the orders that you previously selected you can now choose where these will be collect froom , the initial collection point has been put in for you but you can change this for 1 or more of the orders as required. If you want to change the collection point and/or time, <ul><li>Enter the date and time and then choose the Point that you want</li><li>Tick each of the orders that you want to change</li><li>Click Update Collection Point</li></ul></h2>
    <div class="layoutDefault-boxOuter" style="width: 480px;">
        <div class="layoutDefault-boxInner">
            <div class="layoutDefault-boxHeader" onmousedown="javascript:keyHandler(this);">
                <h1>Collect From</h1>
            </div>
            <div class="layoutDefault-boxContent">
                <uc:Point runat="server" id="ucCollectionPoint" />
            </div>
        </div>
     </div>
     <div class="layoutDefault-boxOuter">
        <div class="layoutDefault-boxInner">
            <div class="layoutDefault-boxHeader">
                <h1>When</h1>
            </div>
            <div class="layoutDefault-boxContent">
                <table cellpadding="0" cellspacing="0" border="0" style="margin-top: 10px;">
                    <tr>
	                    <td nowrap=nowrap><telerik:RadDateInput id="dteCollectionDate" Width="75" runat="server" dateformat="dd/MM/yy" ToolTip="The Booked Date"></telerik:RadDateInput><asp:CustomValidator ID="CustomValidator1" Runat="server" ControlToValidate="dteCollectionDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="The Date must be no more than 1 day before today and no more than 3 days ahead of today."><img src="../../images/error.png" Title="The Date must be no more than 1 day before today and no more than 3 days ahead of today."></asp:CustomValidator></td>
	                    <td>&#160;</td>
	                    <td><telerik:RadDateInput id="dteCollectionTime" runat="server" Width="75" dateformat="t" DataMode="EditModeText" NullText="AnyTime"
			                    Nullable="True"></telerik:RadDateInput></td>
	                    <td>
		                    <asp:requiredfieldvalidator id="rfvBookedDate" runat="server" ControlToValidate="dteCollectionDate" Display="Dynamic"
			                    ErrorMessage="Please enter a Booked Date and Time.">
		                        <img id="Img1" runat="server" src="~/images/error.png" Title="Please enter a Booked Date and Time."/></asp:requiredfieldvalidator>
		                    <asp:CustomValidator ID="cfvBookedDate" Runat="server" ControlToValidate="dteCollectionDate" Display="Dynamic"
			                    EnableClientScript="False" ErrorMessage="Booked Dates for collections must occur before the first delivery, and for deliveries the booked date must occur after the last collection."></asp:CustomValidator>
                			
	                    </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>         
    <div class="clearDiv"></div>      
    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false" EnableViewState="false">
        <div class="MessagePanel" style="vertical-align:middle;">
            <table><tr><td><asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /></td><td><asp:Label cssclass="ControlErrorMessage" id="lblNote" runat="server" /></td></tr></table>
        </div>
    </asp:Panel>
    <asp:label id="lblError" runat="server" cssclass="Error" visible="false"></asp:label>
    <asp:CustomValidator ID="cvLoadOrder" runat="server" ErrorMessage="The Load order is incorrect please check." Enabled="true" ValidationGroup="ReOrder"></asp:CustomValidator>    
     <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
        <MasterTableView Width="100%" DataKeyNames="OrderID" >
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                 <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <img name="moveUpButton" src="/images/newMasterPage/icon-arrow-up.png" onclick= "javascript:up(this, <%#Eval("OrderId") %>);" />&nbsp;
                        <img name="moveDownButton" src="/images/newMasterPage/icon-arrow-down.png" onclick="javascript:down(this, <%#Eval("OrderId") %>);" />&nbsp;
                        <input name="moveTextbox" type="text" style="width: 15px;" maxlength="2" onkeypress="javascript:return validateReorderKey();" onblur="javascript:reorder(this, <%#Eval("OrderId") %>);" value="<%# (Container as Telerik.Web.UI.GridDataItem).ItemIndex + 1 %>" />
                        <input type="hidden" runat="server" id="hidJobId" value='<%#Eval("OrderId") %>' />
                        <input type="hidden" runat="server" id="hidJobOrder" value="" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign=Left HeaderStyle-Width="40" HeaderText="" ></telerik:GridClientSelectColumn>                
                <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                    <ItemTemplate>
                        <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>                                                     
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Town" SortExpression="Delivery Town">
                    <ItemTemplate><b><%#(String)((System.Data.DataRowView)Container.DataItem)["Delivery Town"]%></b></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false"  >
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Plan To Collect From">
                    <ItemTemplate>
                             <span id="spnPlannedCollectionPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["CollectFromPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectFromPoint"].ToString()%></b></span>                                                     
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At (Planned)" SortExpression="CollectAtDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectAtDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectAtAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect From">
                    <ItemTemplate>
                             <span id="spnCollectFrom" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>                                                     
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Order ID" SortExpression="OrderID" DataField="OrderID" HeaderStyle-Width="30" />
                <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="Rate" HeaderStyle-Width="60" UniqueName="Rate" ItemStyle-HorizontalAlign="Right" FooterStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["Rate"].ToString()).ToString("C")%>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblRateTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Pallets" SortExpression="NoPallets" HeaderStyle-Width="60" UniqueName="NoPallets" ItemStyle-HorizontalAlign="right" FooterStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# ((int)((System.Data.DataRowView)Container.DataItem)["NoPallets"]).ToString()%>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblPalletTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80" ItemStyle-HorizontalAlign="right" FooterStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["Weight"].ToString()).ToString("F0")%>
                        <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:Label ID="lblWeightTotal" runat="server"></asp:Label>
                    </FooterTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridButtonColumn HeaderText="" SortExpression="" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" ButtonType="PushButton" CommandName="remove" Text="Remove"></telerik:GridButtonColumn>
                <telerik:GridTemplateColumn HeaderText="References">
                    <ItemTemplate>
                        <asp:Literal ID="litReferences" runat="server"></asp:Literal>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Notes" DataField="DeliveryNotes" ItemStyle-Wrap="true" HeaderStyle-Width="200" />
                <telerik:GridBoundColumn HeaderText="Collection Notes" DataField="CollectionNotes" ItemStyle-Wrap="true" HeaderStyle-Width="200" />
                <telerik:GridTemplateColumn HeaderText="Delivery Address">
                    <ItemTemplate>
                        <asp:Literal ID="litAddress" runat="server"></asp:Literal>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" >
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        </telerik:RadGrid>
                    
        <div class="buttonbar">
            <asp:Button ID="btnResetCollectionPoint" runat="server" Text="Reset Collection Point" CausesValidation="false" ToolTip="This will reset the job to be a load and go." />
            <asp:Button id="btnChangePlannedColletionPoint" runat="server" Text="Update Collection Point" />
            <asp:DropDownList ID="cboBusinessType" runat="server" />
            <asp:button id="btnCreateJob" runat="server" text="Create Job" ValidationGroup="CreateJob" OnClientClick="if(!ConfirmCreation()) {return false;} else { this.style.display = 'none';}" />
            <span style="width:100px; display:inline;">&nbsp;</span><asp:button id="btnSaveGridSettings" runat="server" text="Save Grid Layout"  onclick="btnSaveGridSettings_Click" />
        </div>
       <input type="hidden" runat="server" id="hidJobOrderChanged" value="false" />
        <script type="text/javascript">
        
            document.getElementsByAttribute=function(attrN,attrV,multi)
            {
                attrV=attrV.replace(/\|/g,'\\|').replace(/\[/g,'\\[').replace(/\(/g,'\\(').replace(/\+/g,'\\+').replace(/\./g,'\\.').replace(/\*/g,'\\*').replace(/\?/g,'\\?').replace(/\//g,'\\/');
                var
                    multi=typeof multi!='undefined'?
                        multi:
                        false,
                    cIterate=document.getElementsByTagName('*'),
                    aResponse=[],
                    attr,
                    re=new RegExp(multi?'\\b'+attrV+'\\b':'^'+attrV+'$'),
                    i=0,
                    elm;
                while((elm=cIterate.item(i++)))
                {
                    attr=elm.getAttributeNode(attrN);
                    if(attr &&
                        attr.specified &&
                        re.test(attr.value)
                    )
                        aResponse.push(elm);
                }
                return aResponse;
            }
            
            function OpenJob(jobId)
            {
                openDialogWithScrollbars('../traffic/JobManagement.aspx?wiz=true&jobId=' + jobId + getCSID(),'600','400');
            }
            
            if (<%=OpenJob %> == true)
            {
                OpenJob(<%=JobID %>);

                var url = "deliveries.aspx"+ getCSIDSingle();

                location.href = url;

            }
            
            function ConfirmCreation()
            {
                var answer = true;
                hidIsUpdatePoint = document.getElementById("<%=hidIsUpdatePoint.ClientID%>");
                hidIsOrderChange = document.getElementById("<%=hidIsOrderChange.ClientID%>")

                if(hidIsUpdatePoint.value == "true" || hidIsOrderChange.value == "true")
                answer = confirm("Changes have been made but not Updated, are you sure you wish to proceed?")
                
                
                return answer;
            }
            
            function keyHandler(e)
            {
                var pressedKey;
                if (document.all)	{ e = window.event; }
                
                if(e.type == "mousedown")
                {
                    hidIsUpdatePoint = document.getElementById("<%=hidIsUpdatePoint.ClientID%>")
                    hidIsUpdatePoint.value = "true";
                }
                else
                {
                    hidIsOrderChange = document.getElementById("<%=hidIsOrderChange.ClientID%>")
                    hidIsOrderChange.value = "true";
                }
                
            }
            var currentDOValue = -1;
            function setCurrentDO(el)
            {
                currentDOValue = el.options[el.selectedIndex].value;
               
            }
            
            function orderDeliveries(currentDropDown)
            {
                // Get All of the Dropdowns we are interested in
                var dropDowns = document.getElementsByAttribute("p1", "DeliveryOrder");
                
                // Check the Value of the item we are adjusting
                var currentValue = currentDropDown.options[currentDropDown.selectedIndex].value;
                
                // find the dropdown with the value that we are setting and reverse it
                for(var i = 0; i <dropDowns.length; i ++)
                {
                    if (dropDowns[i] != currentDropDown)
                        if(dropDowns[i].options[dropDowns[i].selectedIndex].value == currentValue)
                            dropDowns[i].options[currentDOValue - 1].selected = true;
                }
                
            }
            
            
           
            document.onkeypress = keyHandler;
            
               showHideMoveButtons();
     

        function showHideMoveButtons() {

            // Show all buttons
            $('table[id*=grdOrders] img[name*=move]').show();
            
            // Hide up button for first row 
            $('table[id*=grdOrders] tr:eq(2) img[name*=moveUpButton]').hide();

            // Hide down button for last row
            $('table[id*=grdOrders] tr:last img[name*=moveDownButton]').hide();
        }
        
        function validateReorderKey(e) {
            var key = 0;
            if (navigator.appName == 'Microsoft Internet Explorer')
                key = window.event.keyCode;
            else
                key = e.which;

            if (key < 48 || key > 57)
                return false;
            return true; 
        }

        function reorder(textbox, orderID) {
            var desiredOrder = parseInt(textbox.value);
            var currentRow = $(textbox).parent().parent();
            
            var rowCount = $('table[id*=grdOrder] tr').length;
            
            if (desiredOrder >= (rowCount - 2))
                desiredOrder = rowCount - 1;
            else if (desiredOrder == 0)
                desiredOrder = 2; // positions 0 and 1 are header and footer respectively.
            else
                desiredOrder = desiredOrder + 1;
                
            var targetRow = $('table[id*=grdOrder] tr:eq(' + desiredOrder + ')');
                
            if (currentRow != null && targetRow != null) {
                var hidCurrentJobOrder = $(currentRow).find('input[id*=hidJobOrder]');
                var hidTargetJobOrder = $(targetRow).find('input[id*=hidJobOrder]');
                
                if (desiredOrder == 2)
                    currentRow.insertBefore(targetRow);
                else
                    currentRow.insertAfter(targetRow);
                                   
                // All the orders and textboxes need to be correctly set for items between the rows.
                var currentJobOrder = parseInt(hidCurrentJobOrder.val());
                var targetJobOrder = parseInt(hidTargetJobOrder.val());
                
                if (currentJobOrder > targetJobOrder)
                {
                    // Swap them around.
                    var temp = targetJobOrder;
                    targetJobOrder = currentJobOrder;
                    currentJobOrder = temp;
                }
                
                for (var i = currentJobOrder + 2; i <= targetJobOrder + 2; i++)
                {
                    var row = $('table[id*=grdOrder] tr:eq(' + i + ')');
                    
                    $(row).find('input[id*=hidJobOrder]').val(i - 2);
                    $(row).find('input[id*=moveTextbox]').value = i - 1;
                }
                    
                $('input[id*=hidJobOrderChanged]').val('true');
            }

            showHideMoveButtons();
        }
                
        function up(butt, orderID) {
            var currentRow = $(butt).parent().parent();
            var previousRow = $(butt).parent().parent().prev();
         
            if (previousRow != null) {
                currentRow.insertBefore(previousRow);

                $('input[id*=hidJobOrderChanged]').val('true');
                var hidPreviousJobOrder = $(previousRow).find('input[id*=hidJobOrder]');
                var previousJobOrder = parseInt(hidPreviousJobOrder.val());
                previousJobOrder++;
                hidPreviousJobOrder.val(previousJobOrder);

                var hidCurrentJobOrder = $(currentRow).find('input[id*=hidJobOrder]');
                var currentJobOrder = parseInt(hidCurrentJobOrder.val());
                currentJobOrder--;
                hidCurrentJobOrder.val(currentJobOrder);
            }

            showHideMoveButtons();
        }

        function down(butt, orderID) {

            var currentRow = $(butt).parent().parent();
            var nextRow = $(butt).parent().parent().next();

            if (nextRow != null) {
                currentRow.insertAfter(nextRow);

                $('input[id*=hidJobOrderChanged]').val('true');

                var hidNextJobOrder = $(nextRow).find('input[id*=hidJobOrder]');
                var nextJobOrder = parseInt(hidNextJobOrder.val());
                nextJobOrder--;
                hidNextJobOrder.val(nextJobOrder);

                var hidCurrentJobOrder = $(currentRow).find('input[id*=hidJobOrder]');
                var currentJobOrder = parseInt(hidCurrentJobOrder.val());
                currentJobOrder++;
                hidCurrentJobOrder.val(currentJobOrder);
            }

            showHideMoveButtons();
        }

    </script>
</asp:Content>
