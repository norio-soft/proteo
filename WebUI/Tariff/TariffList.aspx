<%@ Page Title="Tariff List" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="TariffList.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.TariffList" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Tariffs</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset runat="server" class="invisiableFieldset" id="fsTariffs">
        <telerik:RadGrid ID="grdTariffs" runat="server" AutoGenerateColumns="false">
            <MasterTableView DataKeyNames="TariffID">
                <Columns>
                    <telerik:GridHyperLinkColumn HeaderText="Tariff" DataTextField="TariffDescription"
                        DataNavigateUrlFormatString="EditTariff.aspx?tariffID={0}" DataNavigateUrlFields="TariffID">
                    </telerik:GridHyperLinkColumn>
                    <telerik:GridCheckBoxColumn DataField="IsForSubContractor" HeaderText="Used For Sub Contractors"
                        ItemStyle-Width="100px" ItemStyle-HorizontalAlign="Center">
                    </telerik:GridCheckBoxColumn>
                    <telerik:GridBoundColumn DataField="VersionDescription" HeaderText="Latest Version">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="StartDate" HeaderText="Start Date" ItemStyle-Width="80px"
                        DataFormatString="{0:dd/MM/yy}">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="FinishDate" HeaderText="Finish Date" ItemStyle-Width="80px"
                        DataFormatString="{0:dd/MM/yy}">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="ZoneMap" HeaderText="Zone Map">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="Scale" HeaderText="Scale">
                    </telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>

        <div class="buttonbar">
            <asp:Button ID="btnAdd" runat="server" Text="Add Tariff" />
        </div>
    </fieldset>
</asp:Content>
