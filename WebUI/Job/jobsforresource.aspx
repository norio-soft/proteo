<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job_jobsforresource" Title="Sub-contracted Jobs" Codebehind="jobsforresource.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript">
        function openJob(jobID) {

            var url = "job.aspx?jobId=" + jobID + getCSID();

            window.open(url);
        }

        function openOrderProfile(orderID) {
            window.open("../groupage/ManageOrder.aspx?OID=" + orderID);
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Runs For Resource</h1>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>From here you can find any runs that you have currently got for a resource and a date.</h2>
    
    <fieldset>
        <table>
            <tr>
                <td class="formCellLabel">Driver or Sub-contractor</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true" AllowCustomText="False" ShowMoreResultsBox="false" Skin="Orchestrator" Width="355px" Height="300px"></telerik:RadComboBox> 
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Date</td>
                <td class="formCellField">
                    <table cellpadding="0" cellspacing="0" border="0">
						<tr>
							<td nowrap="nowrap">
							    <telerik:RadDatePicker id="dteDate" runat="server" Width="100" ToolTip="The Start Date">
                                <DateInput runat="server"
                                dateformat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
							</td>
							<td>
							    <asp:requiredfieldvalidator id="rfvBookedDate" runat="server" ControlToValidate="dteDate" Display="Dynamic" ErrorMessage="Please enter a Booked Date and Time.">									
							        <img id="Img1" runat="server" src="~/images/newMasterPage/icon-warning.png" title="Please enter a Date ." alt="" />
							    </asp:requiredfieldvalidator>
						    </td>
						</tr>
					</table>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <div class="buttonBar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
    </div>
    
    <asp:ObjectDataSource runat="server" TypeName="Orchestrator.Facade.Job" SelectMethod="GetForResourceAndDate" ID="odsJobs">
        <SelectParameters>
            <asp:ControlParameter ControlID="cboSubContractor" Name="identityID" PropertyName="SelectedValue" />
            <asp:ControlParameter ControlID="dteDate" Name="startDate" PropertyName="SelectedDate" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <telerik:RadGrid ID="grdJobs" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" DataSourceID="odsJobs" EnableAJAX="True">
        <MasterTableView DataKeyNames="JobId" >
            <DetailTables>
                <telerik:GridTableView Name="Orders">
                    <Columns>
                        <telerik:GridHyperLinkColumn HeaderText="Order Id" DataNavigateUrlFormatString="javascript:openOrderProfile({0})" DataNavigateUrlFields="OrderId" DataTextField="OrderId"></telerik:GridHyperLinkColumn>
                        <telerik:GridTemplateColumn HeaderText="Collect From" >
                            <ItemTemplate>
                                 <span id="spnCollectionPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Deliver To" >
                            <ItemTemplate>
                                 <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </telerik:GridTableView>
            </DetailTables>
            <Columns>
                <telerik:GridHyperLinkColumn DataNavigateUrlFormatString="javascript:openJob({0})" DataNavigateUrlFields="JobID" DataTextField="JobID" HeaderText="Run Id"></telerik:GridHyperLinkColumn>
                <telerik:GridBoundColumn HeaderText="Current Number of Orders" DataField="OrderCount"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Created By" DataField="CreateUserId"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Created Date" DataField="CreateDate"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Last Updated By" DataField="LastUpdateUserID"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Last Update Date" DataField="LastUpdateDate"></telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>