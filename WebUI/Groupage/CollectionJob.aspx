<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage_CollectionJob" MasterPageFile="~/default_tableless.master" Title="Groupage - Collection Job" Codebehind="CollectionJob.aspx.cs" %>
<%@ PreviousPageType VirtualPath="~/Groupage/Collections.aspx" %>

<%@Register TagPrefix="uc" TagName="Point" Src="~/UserControls/Point.ascx" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hidIsUpdatePoint" runat="server" Value='' />                  
    <asp:HiddenField ID="hidIsOrderChange" runat="server" Value='' />
    <script type="text/javascript" language="javascript" src="../script/tooltippopups.js"></script>
    <h1>Collection job details</h1>
    <h2>You can choose what you would like to do with the collections on this job. You need to tick 1 or more orders and then set the "what, where and when" details and click <b>Update Delivery Point</b>. When you have finished making your changes click <b>Create Job</b>.</h2>
    <div class="layoutDefault-boxOuter" style="width:170px;" >
            <div class="layoutDefault-boxInner">
                <div class="layoutDefault-boxHeader">
                    <h1>What</h1>
                </div>
                <div class="layoutDefault-boxContent">
                    <asp:radiobuttonlist id="rblOrderAction" runat="server"/>
                </div>
            </div>
     </div>
      <div class="layoutDefault-boxOuter" style="width:420px;">
            <div class="layoutDefault-boxInner">
                <div class="layoutDefault-boxHeader">
                    <h1>Where</h1>
                </div>
                <div class="layoutDefault-boxContent">
                    <uc:Point runat="server" ID="ucDeliveryPoint"  />
                </div>
            </div>
     </div>
      <div class="layoutDefault-boxOuter" style="width:170px;" >
            <div class="layoutDefault-boxInner">
                <div class="layoutDefault-boxHeader">
                    <h1>When</h1>
                </div>
                <div class="layoutDefault-boxContent">
                     <table cellpadding="0" cellspacing="0" border="0">
						    <tr>
							    <td nowrap=nowrap><telerik:RadDateInput id="dteDeliveryDate" width="70" runat="server" dateformat="dd/MM/yy" ToolTip="The Delivery Date"></telerik:RadDateInput><asp:CustomValidator ID="CustomValidator1" Runat="server" ControlToValidate="dteDeliveryDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="The Date must be no more than 1 day before today and no more than 3 days ahead of today."><img src="../../images/error.png" Title="The Date must be no more than 1 day before today and no more than 3 days ahead of today."></asp:CustomValidator></td>
							    <td>&#160;</td>
							    <td><telerik:RadDateInput id="dteDeliveryTime" runat="server" width="70" dateformat="t" DataMode="EditModeText" NullText="AnyTime"
									    Nullable="True"></telerik:RadDateInput></td>
							    <td>
								    <asp:requiredfieldvalidator id="rfvBookedDate" runat="server" ControlToValidate="dteDeliveryDate" Display="Dynamic"
									    ErrorMessage="Please enter a Booked Date and Time.">
									    <img id="Img1" runat="server" src="~/images/error.png" Title="Please enter a Booked Date and Time."></asp:requiredfieldvalidator>
								    <asp:CustomValidator ID="cfvBookedDate" Runat="server" ControlToValidate="dteDeliveryDate" Display="Dynamic"
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
    <asp:CustomValidator ID="cvDeliveryPoint" runat="server" ErrorMessage="The Delivery Point cannot be the same as the collection point" Display="Dynamic" ValidationGroup="Buttons">
        <div style="background-color:White; padding:5px; vertical-align:middle;"><img src="~/images/ico_critical_Small.gif" runat="server" /><span style="padding-left:10px;">The <b>Planned Delivery Point</b> cannot be the same as the <b>collection point</b></span></div>
    </asp:CustomValidator>
    <asp:label  id="lblError" runat="server" cssclass="Error" visible="false"></asp:label>
    <asp:CustomValidator ID="cvLoadOrder" runat="server" ErrorMessage="The Load order is incorrect please check." Enabled="true" ValidationGroup="ReOrder">
        <div style="background-color:White; padding:5px; vertical-align:middle;"><img id="Img2" src="~/images/ico_critical_Small.gif" runat="server" /><span style="padding-left:10px;">The Load order cannot be applied as 1 or more numbers have been entered incorrectly.</span></div>
    </asp:CustomValidator>    
    <asp:CustomValidator ID="cvCrossDockLocationError" runat="server" ErrorMessage="You cannot <b>Cross Dock</b> and order at the same Point as its delivery Point." Enabled="true" ValidationGroup="Buttons">
        <div style="background-color:White; padding:5px; vertical-align:middle;"><img id="Img3" src="~/images/ico_critical_Small.gif" runat="server" /><span style="padding-left:10px;">You cannot <b>Cross Dock</b> an Order to the same place as it is to be delivered.</span></div>
    </asp:CustomValidator>    
     <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
        <MasterTableView Width="100%" ClientDataKeyNames="OrderID" DataKeyNames="OrderID" >
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <DetailTables>
                <telerik:GridTableView ClientDataKeyNames="OrderID" DataKeyNames="OrderID"  AutoGenerateColumns="false"  >
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="OrderID" MasterKeyField="OrderID" />
                    </ParentTableRelation>
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                        <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription" />
                        <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                        <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="false"/>
                    </Columns>
                </telerik:GridTableView>       
            </DetailTables>
            <Columns>
                <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign=Left HeaderStyle-Width="40" HeaderText="" ></telerik:GridClientSelectColumn>
                <telerik:GridTemplateColumn HeaderText="" HeaderStyle-Width="20">
                    <ItemTemplate>
                        <asp:Textbox id="txtLoadOrder" runat="server" width="20"></asp:Textbox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Collect From">
                    <ItemTemplate>
                         <span id="spnCollectionPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Collect Town" SortExpression="Collection Town" DataField="Collection Town" ItemStyle-Wrap="false" ItemStyle-Font-Bold="true"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionRunDeliveryDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                    <ItemTemplate>
                         <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliverAtPlanned" ItemStyle-Width="150" ItemStyle-Wrap="false"  >
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver To (Planned)" SortExpression="DeliverToPoint">
                    <ItemTemplate>
                         <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliverToPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><%#((System.Data.DataRowView)Container.DataItem)["DeliverToPoint"].ToString()%></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At (Planned)">
                    <ItemTemplate>
                       <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliverAtDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliverAtAnyTime"])%></ItemTemplate>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Action">
                    <ItemTemplate>
                        <ItemTemplate><%#GetOrderAction((int)((System.Data.DataRowView)Container.DataItem)["OrderActionID"])%></ItemTemplate>
                    </ItemTemplate>
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
                <telerik:GridButtonColumn HeaderText="" SortExpression="" HeaderStyle-Width="30" ItemStyle-HorizontalAlign="Center" ButtonType="ImageButton" CommandName="remove" ImageUrl="/images/newmasterpage/icon-cross.png"></telerik:GridButtonColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" AllowExpandCollapse="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" EnableDragToSelectRows="true" />
        </ClientSettings>
        </telerik:RadGrid>
        
         <script language="javascript">
            function OpenJob(jobId)
            {
                openDialogWithScrollbars('../traffic/JobManagement.aspx?wiz=true&jobId=' + jobId + getCSID(),'600','400');
            }
            
            if (<%=OpenJob %> == true)
            {
                var BT = '<% = Request.QueryString["BT"] == null ? "" : Request.QueryString["BT"].ToString()  %>';
                OpenJob(<%=JobID %>);
                if (BT != "")
                    location.href="collections.aspx?BT=" + BT;
                else
                    location.href="collections.aspx";
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
            
            document.onkeypress = keyHandler;
        </script>
            
        <div  class="buttonBar">
            <asp:Button id="btnChangePlannedDeliveryPoint" runat="server" Text="Update Delivery Point" ValidationGroup="Buttons" />
            <asp:Button ID="btnReOrder" Text="Re-Order" runat="server" ValidationGroup="ReOrder" />
            <asp:DropDownList ID="cboBusinessType" runat="server" />
            <asp:button id="btnCreateJob" runat="server" text="Create Job" ValidationGroup="CreateJob" OnClientClick="if(!ConfirmCreation()) { return false; } else { this.style.display = 'none';}" />
            
        </div>
    </asp:Content>
