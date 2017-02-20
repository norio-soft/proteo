<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.ShuntListReport" Codebehind="ShuntListReport.aspx.cs" MasterPageFile="~/default_tableless.master" Title="Shunt List" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>Shunt List</h1>
    <h2>A list of job(s) which are due for loading (collection) for today even if these job(s) are for delivery the next day.</h2>
    <fieldset>
	    <table>
		    <tr>
			    <td class="formCellLabel">Client</td>
			    <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                        ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" Width="355px" AllowCustomText="false" Height="300px">
                    </telerik:RadComboBox>
				    <asp:RequiredFieldValidator id="rfvClient" runat="server" Display="Dynamic" ControlToValidate="cboClient" ErrorMessage="Please supply a client to report on."><img src="/images/newMasterPage/icon-warning.png" height="16" width="16" title="Please supply a client to report on." /></asp:RequiredFieldValidator>
				    <asp:CustomValidator id="cfvClient" runat="server" Display="Dynamic" EnableClientSideScript="False" ControlToValidate="cboClient" ErrorMessage="Please supply a client to report on."><img src="/images/newMasterPage/icon-warning.png" height="16" width="16" title="Please supply a client to report on." /></asp:CustomValidator>
				    <asp:Label ID="lblError" cssclass="ControlErrorMessage" EnableViewState="False" Visible="False" Runat="server"/>
			    </td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Produce Shunt List for</td>
			    <td class="formCellField">
				    <table cellpadding="0" cellspacing="0" border="0">
					    <tr>
						    <td>
							    <telerik:RadDateInput id="dteShuntListDate" runat="server" ToolTip="The date to display a shunt list for" dateformat="dd/MM/yy" Width="60px"></telerik:RadDateInput>
						    </td>
						    <td>
							    &nbsp;<asp:RequiredFieldValidator id="rfvShuntListDate" runat="server" ControlToValidate="dteShuntListDate" Display="Dynamic" ErrorMessage="Please supply a date to report on."><img src="/images/newMasterPage/icon-warning.png" height="16" width="16" title="Please supply a date to report on." /></asp:RequiredFieldValidator>
						    </td>
					    </tr>
				    </table>
			    </td>
		    </tr>
	    </table>
    </fieldset>
    <div class="buttonbar">
	    <nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>
	    <asp:Button ID="btnReset" Runat="server" Text="Reset" CausesValidation="False"></asp:Button>
    </div>
    <uc1:ReportViewer id="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>
</asp:Content>
