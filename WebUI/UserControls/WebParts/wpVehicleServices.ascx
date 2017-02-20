<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpVehicleServices" Codebehind="wpVehicleServices.ascx.cs" %>
<asp:sqldatasource id="sqlVehiclesToService" runat="server" DataSourceMode="DataSet" selectCommand="spVehicle_DueService" ConnectionString="<%$ConnectionStrings:Orchestrator %>" cacheexpirationpolicy="Absolute" enablecaching="true" cacheduration="1440">
</asp:sqldatasource>
            				                
<asp:gridview id="gvJobsReadyToInvoice" DataSourceID="sqlVehiclesToService" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showfooter="false" showheader="false" >
    <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <footerstyle cssclass="Row" />
    <columns>
            <asp:boundfield datafield="RegNo" HeaderText="Reg" ItemStyle-Width="80" />
            <asp:boundfield datafield="VehicleServiceDueDate" HeaderText="Date" DataFormatString="{0:dd/MM/yy}" HtmlEncode="false" />
            <asp:TemplateField HeaderText="Type">
                <ItemTemplate>
                    <%# ((System.Data.DataRowView)Container.DataItem)["IsMOT"].ToString() == "1" ? "MOT" : "Service" %>
                </ItemTemplate>
            </asp:TemplateField>
    </columns>
 </asp:gridview>