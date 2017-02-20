<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="ScanOrUpload.aspx.cs" Inherits="Orchestrator.WebUI.scan.wizard.ScanOrUpload" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<%--<script language="javascript" type="text/javascript">
        function RequestStart(sender, args) {
            if (hidePageShowLoading != null) {
                hidePageShowLoading();
            }
        }
        function RequestEnd() {
            if (showPageHideLoading != null) {
                showPageHideLoading();
            }
            
        }
    </script>--%>
    
<div>
    <fieldset>
        <legend> What would you like to do? </legend>
        <asp:Panel ID="optionPanel" runat="server">
            <% if (dialogMode != PODDialogMode.Manual) { %>
            <table id="tblPODScan" runat="server">
                <tr  id="ScanField" style="display: inline;" >
                    <td >
                        <asp:RadioButtonList ID="rblOperation" runat="server" AutoPostBack="false" onclick="rblOperation_click(this); ">
                            <asp:ListItem Enabled="true" Selected="True" Text="I would like to scan a document" Value="0">
                            </asp:ListItem>
                            <asp:ListItem Enabled="true" Selected="False" Text="I would like to upload a document" Value="1">
                            </asp:ListItem>
                            <asp:ListItem Enabled="true" Selected="False" Text="No document Available" Value="2">
                            </asp:ListItem>
                        </asp:RadioButtonList>    
                        <br/>            
                        <fieldset runat="server" id="AppendOrReplaceFieldSet">
                            <legend runat="server" id="appendReplaceText">A document already exists, what would you like to do?</legend>
                            <asp:RadioButtonList ID="rblAppandOReplace" runat="server">
                                <asp:ListItem Enabled="true" Selected="True" Text="I would like to replace the existing document." Value="0">
                                </asp:ListItem>
                                <asp:ListItem Enabled="true" Selected="False" Text="I would like to append to the existing document" Value="1">
                                </asp:ListItem>
                            </asp:RadioButtonList>   
                        </fieldset>
                    </td>
                </tr>
            </table>
            <% } else { %>
            <table id="tblPODReturn" runat="server">
                <tr valign="top" runat="server">
                    <td>
                        <asp:RadioButtonList ID="rblPODReceived" runat="server" AutoPostBack="false">
                            <asp:ListItem Enabled="true" Selected="True" Text="POD Received From Driver" Value="0">
                            </asp:ListItem>
                            <asp:ListItem Enabled="true" Selected="False" Text="POD Not Received" Value="1">
                            </asp:ListItem>                            
                        </asp:RadioButtonList>   
                    </td>
                </tr>
             
                <tr runat="server">
					<td >
						Notes for Return of POD
					</td>
				</tr>
				<tr runat="server">
					<td class="formCellField-Horizontal">
						<asp:TextBox ID="txtPODReturnComment" runat="server" CssClass="fieldInputBox" Width="340"
							TextMode="MultiLine" Columns="90" Rows="3" MaxLength="500"></asp:TextBox>
					</td>
				</tr>
                 <tr>
                    <td>
                        &nbsp
                    </td>
                </tr>
            </table>
            <% } %>
        </asp:Panel>
    </fieldset>
    
    <div class="buttonbar">
        <% if (dialogMode != PODDialogMode.Manual) { %>
            <input type="button" value="Cancel" onclick="window.close();" />
            <div style="float:right;">
                <asp:button runat="server" id="btnNext" text="Next >"></asp:button>&nbsp;&nbsp;
            </div>
        <% } else { %>
            <input type="button" value="Cancel" onclick="window.close();" />
            <div style="float:right;">
                <asp:button runat="server" id="btnRecordPODReturn" text="Save" OnClick="btnRecordPODReturn_Click" OnClientClick=""></asp:button>
            </div>
        <% } %>
    </div>
</div>

    <script language="javascript" type="text/javascript">
        
        var rblAppandOReplace = $("#" + "<%=rblAppandOReplace.ClientID %>");

        function rblOperation_click(sender) {
            if (rblAppandOReplace != null && rblAppandOReplace[0] != null && rblAppandOReplace.css("display") != "none") {
                var selectedRadioButton = $("#" + "<%=rblOperation.ClientID %> :checked");
            
            }
        }

    </script>

</asp:Content>