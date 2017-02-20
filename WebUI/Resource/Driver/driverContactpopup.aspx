<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Resource.Driver.DriverContactPopUp" Codebehind="driverContactpopup.aspx.cs" %>
<%@ OutputCache Duration="3600" VaryByParam="identityId" %>
<html>
<head runat="server">
</head>
<body>
    <table runat="server" id="tblSubContractorIndividual">
        <tr><td colspan="2" style="border-bottom:solid 1pt black;"><asp:Label runat="server" id="lblIndividualFullName"></asp:Label></td></tr>
        <tr><td><b>Home:</b></td><td><asp:Label ID="lblIndiviualHomePhone" runat="server"></asp:Label></td></tr>
        <tr><td><b>Mobile:</b></td><td><b><asp:Label ID="lblIndividualMobilePhone" runat="server"></asp:Label></td></tr>
        <tr><td><b>Personal Mobile:</b></td><td><asp:Label ID="lblIndividualPersonalMobile" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr><td colspan="2"><asp:Label ID="lblIndividualAddressLine1" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblIndividualAddressLine2" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblIndividualAddressLine3" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblIndividualPostTown" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblIndividualCounty" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblIndividualPostCode" runat="server"></asp:Label></td></tr>
    </table>
    <table runat="server" id="tblSubContractorOrganisation">
        <tr><td colspan="2" style="border-bottom:solid 1pt black;"><asp:Label runat="server" id="lblOrganisationFullName"></asp:Label></td></tr>
        <tr><td><b>Main Telephone:</b></td><td><asp:Label ID="lblOrganisationMainTelephone" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2">&nbsp;</td></tr>
        <tr><td colspan="2"><asp:Label ID="lblOrganisationAddressLine1" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblOrganisationAddressLine2" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblOrganisationAddressLine3" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblOrganisationPostTown" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblOrganisationCounty" runat="server"></asp:Label></td></tr>
        <tr><td colspan="2"><asp:Label ID="lblOrganisationPostCode" runat="server"></asp:Label></td></tr>
    </table>
</body>
</html>