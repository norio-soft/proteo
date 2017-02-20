<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.footer" Codebehind="footer.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="alerter" Src="alerter.ascx" %>
                                        <!-- End Content --> 
                                    </div>  
                                </td>
                            </tr>
                            <tr>
                                <td class="layoutContentBottom">&nbsp;</td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <asp:Panel ID="__pnlFooter" cssclass="layoutFooter" runat="server">
        <tr style="display:<%=IsWizard%>;padding-top:2px;">
            <td style="display:<%=IsWizard%>;visibility:hidden;">
                © 2000-2004 P1 Technology Partners Ltd, All rights reserved.&nbsp;<asp:Label ID="lblUser" runat="server"></asp:Label>
            </td>
        </tr>
    </asp:Panel>
</table>
<script>var frm = document.getElementById("frmNews"); if (frm != null) { frm.src=frm.src; }</script>
</body>
</HTML>
