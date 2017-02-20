<%@ Page Language="C#" AutoEventWireup="true" Inherits="Groupage_loadorder" Codebehind="loadorder.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Load Order</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	<script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
	<script language="javascript" src="/script/gridDragAndDrop.js" type="text/javascript"></script>

    <div>
        
        <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
            <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Load Order</div>
            <span style="font-size:11px;">
                <asp:Label ID="lblAmendedBy" runat="server" ></asp:Label>
                <p>
                    <span style="background-color:#C5CDF3; width:20px; height:20px; border:solid 1pt #7C8EE4;">&#160;&#160;&#160;</span> To be Loaded at this point.
                </p>
            </span>
        </fieldset>
        
        <telerik:RadGrid runat="server" ID="grdLoadOrder" Skin="Office2007" AutoGenerateColumns="false">
            <MasterTableView DataKeyNames="OrderID">
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="selectColumn">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkPIL" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                   
                    <telerik:GridTemplateColumn HeaderText="Load Order" HeaderStyle-Width="70px" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:TextBox ID="txtOrder" runat="server" Width="20"></asp:TextBox>
                         </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Order Number" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Delivery Order Number" DataField="DeliveryOrderNumber"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Pallets" DataField="NoPallets"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Customer" DataField="CustomerOrganisationName"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Destination" DataField="DeliveryPointDescription"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Collecting Trailer" DataField="Trailer" visible="false" ></telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings  >
                <Selecting AllowRowSelect="true" />
                <ClientEvents OnRowCreated="RowCreated"></ClientEvents>
            </ClientSettings>
        </telerik:RadGrid>
        
        <div class="buttonbar">
            <asp:Button ID="btnCancel" runat="server" Text="Close" />
            <asp:Button ID="btnSave" runat="server" Text="Save" Visible="false"/>
            <asp:Button ID="btnPIL" runat="server" Text="Print PIL" Enabled="false" />
            <asp:Button ID="btnLoadSheet" runat="server" Text="Load Sheet" />
        </div>
        
    </div>
    
    <telerik:RadCodeBlock ID="rcbLoadOrder" runat="server">
        <script type="text/javascript" language="javascript">
            var ichkCount = 0;

            function ReorderRows(index1, index2) 
            {
                window["<%= RadAjaxManger1.ClientID %>"].AjaxRequestWithTarget( "<%= grdLoadOrder.UniqueID %>", "RowMoved," + index1 + "," + index2);
            }
        
            function SetPrintPILButtonState(el)
            {
                var btn = document.getElementById("<%=btnPIL.ClientID %>")
                if (el.checked) ichkCount ++;
                else ichkCount --;
                
                if(ichkCount < 0 ) ichkCount = 0;
                
                if (ichkCount    > 0)
                {
                    btn.disabled = false;
                }
                 else
                 {
                     btn.disabled = true;
                 }
            }

            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz az well) 
                return oWindow;
            }

            function CloseOnReload() {
                GetRadWindow().Close();
            }

            function RefreshParentPage() {
                GetRadWindow().BrowserWindow.location.reload();
                GetRadWindow().Close();
            }
            
        </script>
    </telerik:RadCodeBlock>
    
    <telerik:RadAjaxManager ID="RadAjaxManger1" runat="server" >
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="grdLoadOrder">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="grdLoadOrder" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
    <asp:Label ID="lblInjectScript" runat="server"></asp:Label>     
    
</asp:Content>