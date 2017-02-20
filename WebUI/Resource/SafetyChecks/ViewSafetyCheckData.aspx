<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewSafetyCheckData.aspx.cs" Inherits="Orchestrator.WebUI.Resource.SafetyChecks.View" MasterPageFile="~/WizardMasterPage.master" %>

<asp:content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <style>
        h2 {
            font-size: 18px !important;
            font-weight: bold !important;
            margin-left: -25px !important;
            text-align: center;
        }

        .dataTableWrapper, .dataTable, .listTable { width:100% }

        .resultsOverview { margin: 10px auto; }

        .scPass, .scDiscPass, .scFail  { font-weight: bold; }
        .scPass { color: #0c0; }
        .scDiscPass { color: #ca0; }
        .scFail { color: #c00; }
        .scLabel {
            font-weight: bold;
            text-align: right;
            padding-right: 10px;
        }
        .centered {
            text-align: center;
            padding: 1px;
        }
        .scData {
            text-align: left;
            padding-left: 10px;
        }

        .sigHolder {
            width: 460px;
            height: 200px;
            border: 1px solid #e9e9e9;
        }

        .dataTable tr td {
            width:50%;
            display: table-cell;
        }
        .dataTable tr td span {
            display:table-cell;
        }

        .listTable {
            margin: 0 auto;
            max-width: 946px;
        }

        .listTable thead tr {
            line-height: 28px;
            background-color: #000;
        }
        .listTable thead tr th {
            font-weight: normal;
            color: #fff;
            padding-left: 8px;
            border-right: 1px solid #363636;
            border-bottom: 1px solid #363636;
        }

        .listTable tbody tr {
            line-height: 20px;
        }

        tr.odd {
            background-color: #f5f5f5;
        }

        .listTable tbody tr td {
            padding: 3px;
            border-left: 1px solid #e9e9e9;
            border-right: 1px solid #e9e9e9;
            border-bottom: 1px solid #e9e9e9;
        }
    </style>
    <script type="text/javascript">
        function openNewPopupWindow(winName, url) {
            var newWindow = window.open(url, winName, 'height=650,width=430,toolbar=no,menu=no,scrollbars=yes');
            if (window.focus) newWindow.focus();
        }
    </script>
</asp:content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <h2>Safety Check Results</h2>

    <table id="safetyCheckResultsTable" class="resultsOverview">
        <tr>
            <td class="scLabel">Performed By</td>
            <td><asp:Label CssClass="scData" runat="server" id="lblDriverTitle"></asp:Label></td>
        </tr>
        <tr>
            <td>
                <div class="dataTableWrapper" id="vehicleWrapper">
                    <table class="dataTable" id="vehicleSafetyCheckData" runat="server">
                        <tr>
                            <td class="scLabel">Vehicle</td>
                            <td><asp:Label CssClass="scData" runat="server" id="lblVehicleTitle"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel">Vehicle Profile</td>
                            <td><asp:Label CssClass="scData" runat="server" id="lblVehicleProfile"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel">When</td>
                            <td><asp:Label CssClass="scData" runat="server" id="lblVehicleCheckDate"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel">Status</td>
                            <td><asp:Label CssClass="scData" runat="server" id="lblVehicleStatus"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel centered" colspan="2">Signature (if applicable)</td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="sigHolder" id="divVehicleSig">
                                    <telerik:RadBinaryImage id="vehicleSigImg" runat="server" width="460" height="200" ImageUrl="~/images/signaturena_460x220.png" ResizeMode="Fit" AutoAdjustImageControlSize="False"/>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
            <td>
                <div class="dataTableWrapper" id="trailerWrapper">
                    <table class="dataTable" id="trailerSafetyCheckData" runat="server">
                        <tr>
                            <td class="scLabel">Trailer</td>
                            <td class="scData"><asp:Label runat="server" id="lblTrailerTitle"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel">Trailer Profile</td>
                            <td class="scData"><asp:Label runat="server" id="lblTrailerProfile"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel">When</td>
                            <td class="scData"><asp:Label runat="server" id="lblTrailerCheckDate"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel">Status</td>
                            <td><asp:Label runat="server" id="lblTrailerStatus"></asp:Label></td>
                        </tr>
                        <tr>
                            <td class="scLabel centered" colspan="2">Signature (if applicable)</td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div class="sigHolder" id="divTrailerSig">
                                    <telerik:RadBinaryImage id="trailerSigImg" runat="server" width="460" height="200" ImageUrl="~/images/signaturena_460x220.png" ResizeMode="Fit" AutoAdjustImageControlSize="False"/>
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
            </td>
        </tr>
    </table>
        
    <asp:Table CssClass="listTable" id="safetyCheckFaultListTable" runat="server" CellSpacing="0" CellPadding="1">
        <asp:TableHeaderRow id="safetyCheckFaultListHeader" runat="server">
            <asp:TableHeaderCell id="safetyCheckFaultListProfileHeader" runat="server" width="20%">Profile</asp:TableHeaderCell>
            <asp:TableHeaderCell id="safetyCheckFaultListCheckHeader" runat="server" width="20%">Check</asp:TableHeaderCell>
            <asp:TableHeaderCell id="safetyCheckFaultListStatusHeader" runat="server" width="15%">Status</asp:TableHeaderCell>
            <asp:TableHeaderCell id="safetyCheckFaultListCommentHeader" runat="server" width="35%">Comment</asp:TableHeaderCell>
            <asp:TableHeaderCell id="safetyCheckFaultListPhotoHeader" runat="server" width="10%">Photo(s)</asp:TableHeaderCell>
        </asp:TableHeaderRow>
    </asp:Table>

</asp:Content>
