<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="exchangeRate.aspx.cs" Inherits="Orchestrator.WebUI.currency.exchangeRate" Title="Haulier Enterprise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server"> 
    <telerik:RadWindowManager ID="rmwExchangeRate" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="smallWindow" Height="200" Width="300" />
        </Windows>
    </telerik:RadWindowManager>
    <h1><asp:Label ID="lblTitle" runat="server" Text="Exchange Rates"></asp:Label></h1>
    <h2>All Currencies are valued against the pound.</h2>
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr>
                <td class="formCellLabel">Currency</td>
                <td class="formCellField"><telerik:RadComboBox ID="rcbCurrency" runat="server" AutoPostBack="false" Skin="WindowsXP" /></td>
            </tr>
            <tr>
                <td class="formCellLabel">Date From</td>
                <td class="formCellField">
                    <telerik:RadDatePicker ID="rdiExchange" runat="server" AutoPostBack="false" Width="100px" >
                    <DateInput runat="server"
                    DateFormat="dd/MM/yyyy">
                    </DateInput>
                    </telerik:RadDatePicker>
                </td>
            </tr>
        </table>
    </fieldset>         
    <div class="buttonbar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" />
    </div>          
    <div>
        <telerik:RadGrid ID="rgExchangeRates" runat="server" Width="500px" AllowPaging="true" AllowSorting="true" >
            <MasterTableView AutoGenerateColumns="false">
                <Columns>
                    <telerik:GridBoundColumn DataField="ExchangeRate" HeaderText="Exchange Rate" SortExpression="ExchangeRate" />
                    <telerik:GridBoundColumn DataField="EffectiveDate" HeaderText="Effective From" DataFormatString="{0:D}" SortExpression="EffectiveDate" />
                    <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created On" SortExpression="CreateDate" />
                    <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" SortExpression="CreateUserID" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>  
    <div class="buttonbar">
        <input type="button" value="Add Exchange Rate" onclick="javascript:addExchangeRate();" />
    </div>
    <div>
        <telerik:RadAjaxManager ID="ramExchange" runat="server">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="rcbCurrency">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rgExchangeRates" LoadingPanelID="LoadingPanel" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="rgExchangeRates">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="rgExchangeRates" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        
        <telerik:RadAjaxLoadingPanel ID="LoadingPanel" runat="server">
            <asp:Image ID="imgLP" runat="server" ImageUrl="~/images/Loading.gif" AlternateText="Loading" BorderWidth="0" /> 
        </telerik:RadAjaxLoadingPanel>
    </div>
    <telerik:RadCodeBlock runat="server">
        <script language="javascript" type="text/javascript">
            function addExchangeRate()
            {
                var url = "AddExchangeRate.aspx";

                var wnd = window.radopen("about:blank", "SmallWindow");
                wnd.SetUrl(url);
                //wnd.OnClientClose = "exchangeRateClose";
                wnd.add_close(exchangeRateClose);
                wnd.SetTitle("Add Exchange Rate"); 
                wnd.SetWidth("400px");
                wnd.SetHeight("300px");
            }
            
            function exchangeRateClose()
            {
                var refreshButton = document.getElementById("<%=btnRefresh.ClientID %>");
                refreshButton.click();
            }
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
