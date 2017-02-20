<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/WizardMasterPage.master" Inherits="Orchestrator.WebUI.Job.ChangeJobClient" Codebehind="ChangeJobClient.aspx.cs" Title="Change Job Client" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change Job Client</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
        <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td colspan="2" width="100%">
                    <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%" >
                        <tr>
                            <td colspan="2" class="pageHeadingDefault myTitle" style="border-right: medium none;
                                border-top: medium none; border-left: medium none; border-bottom: medium none">
                               Change Job's Client
                            </td>
                        </tr>
                        <tr>
                            <td class="greyText" width="100%" valign="top" height="330">
                                <table border="0" width="100%">
                                    <tr valign=top>
                                        <td valign="middle" width="50">Client:</td>
                                        <td valign="top">
                                            <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" 
                                                MarkFirstMatch="true" Height="300px" Overlay="true" 
                                                ShowMoreResultsBox="false" Width="355px" AllowCustomText="False" AutoPostBack="true">
                                            </telerik:RadComboBox>
                                            <asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" Display="Dynamic" ErrorMessage="Please select the client this job is for."><img src="../images/error.png"  Title="Please select the client this job is for."></asp:RequiredFieldValidator></td>
                                    </tr>
                                    <tr>
                                        <td valign="top">&nbsp;</td>
                                        <td>
                                            <asp:Repeater ID="repExistingReferences" runat="server">
                                                <HeaderTemplate>
                                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                        <thead>
                                                            <tr>
                                                                <th width="50%">Reference</th>
                                                                <th width="50%">Value</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                            <tr>
                                                                <td valign="top"><%#((Orchestrator.Entities.JobReference)Container.DataItem).OrganisationReference.Description %></td>
                                                                <td valign="top"><%#((Orchestrator.Entities.JobReference)Container.DataItem).Value %></td>
                                                            </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                        </tbody>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="2"><hr noshade="noshade" /></td>
                                    </tr>
                                    <tr>
                                        <td valign="top">&nbsp;</td>
                                        <td>
                                            <asp:Repeater ID="repReferences" runat="server">
                                                <HeaderTemplate>
                                                    <table width="100%" border="0" cellpadding="0" cellspacing="0">
                                                        <thead>
                                                            <tr>
                                                                <th width="50%">Reference</th>
                                                                <th width="50%">Value</th>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                                </HeaderTemplate>
                                                <ItemTemplate>
							                                <tr>
								                                <td width="150" valign="top">
									                                <span><%# DataBinder.Eval(Container.DataItem, "Description") %></span>
									                                <input type="hidden" id="hidOrganisationReferenceId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "OrganisationReferenceId") %>'>
								                                </td>
								                                <td valign="top">
									                                <asp:PlaceHolder ID="plcHolder" Runat="server">
										                                <asp:TextBox ID="txtReferenceValue" Runat="server"></asp:TextBox>
										                                <asp:RequiredFieldValidator id="rfvReferenceValue" runat="server" ControlToValidate="txtReferenceValue" EnableClientScript="False" Display="Dynamic" ErrorMessage='Please supply a <%# DataBinder.Eval(Container.DataItem, "Description") %>.'><img src="../images/error.png"  Title='Please supply a <%# DataBinder.Eval(Container.DataItem, "Description") %>.'></asp:RequiredFieldValidator>
									                                </asp:PlaceHolder>
								                                </td>
							                                </tr>
                                                </ItemTemplate>
                                                <FooterTemplate>
                                                        </tbody>
                                                    </table>
                                                </FooterTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                </table>

                                <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false">
                                </cs:WebModalWindowHelper>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="buttonbar">
                        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75"></asp:Button>
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
                    </div>
                </td>
            </tr>
        </table>
</asp:Content>