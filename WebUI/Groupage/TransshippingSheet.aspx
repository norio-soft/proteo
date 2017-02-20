<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.TransshippingSheet" MasterPageFile="~/WizardMasterPage.master" Title="Trans-shipping Sheet" Codebehind="TransshippingSheet.aspx.cs" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="uc" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
        <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Trans-shipping Sheet</div>
        <div style="height: 630px; width: 100%; overflow: auto;">
            <table width="979" border="0" cellpadding="2" cellspacing="0">
	            <tr>
	                <td align="left" valign="top" width="150">Instruction: </td>
	                <td align="left" valign="top">
	                    <asp:TextBox ID="txtInstruction" runat="server" TextMode="Multiline" Rows="6" Columns="60" />
	                </td>
	            </tr>
		        <tr>
		            <td colspan="2">
		                <br />
                        <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;">
                            <asp:Button ID="btnProduceReport" runat="server" Text="Produce Report" CausesValidation="false" />
                        </div>
                    </td>
		        </tr>
		        <tr>
		            <td colspan="2">
                        <uc:ReportViewer id="reportViewer" runat="server" EnableViewState="False"></uc:ReportViewer>
		            </td>
		        </tr>
            </table>
        </div>
    </fieldset>

    <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false"></cs:WebModalWindowHelper>
</asp:Content>