<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Reference Control="~/job/wizard/usercontrols/jobtype.ascx" %>
<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.Details" Codebehind="Details.ascx.cs" %>
<script language="javascript" src="../../../script/popAddress.js"></script>
<table width="100%" height="672" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td colspan="2" bgcolor="white" align="right" style="PADDING-RIGHT:10px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Job Details
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Please review the job details displayed below.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="505" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:10px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
			width="100%" colspan="2">
			<div>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr id="trFailed" runat="server" visible="false">
						<td colspan="2">
							<span class="warning">The job could not be created.</span>
						</td>
					</tr>
					<tr>
						<td width="100">
							<span><b>Job Id</b></span>
						</td>
						<td>
							<asp:Label ID="lblJobId" runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td>
							<span><b>Client</b></span>
						</td>
						<td>
							<asp:Label ID="lblClient" runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td>
							<span><b>Business Type</b></span>
						</td>
						<td>
							<asp:Label ID="lblBusinessType" runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="HEIGHT:10px"></div>
						</td>
					</tr>
					<tr>
						<td>
							<span><b>Job Type</b></span>
						</td>
						<td>
							<asp:Label ID="lblJobType" runat="server"></asp:Label>
						</td>
					</tr>
					<TR>
						<TD><STRONG>Job Stock Movement</STRONG></TD>
						<TD>
							<asp:Label id="lblJobMovement" runat="server"></asp:Label></TD>
					</TR>
					<tr>
						<td>
							<span><b>Job Charge Type</b></span>
						</td>
						<td>
							<asp:Label ID="lblJobChargeType" Runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td>
							<span><b>Job Charge Amount</b></span>
						</td>
						<td>
							<asp:Label ID="lblJobChargeAmount" Runat="server"></asp:Label>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div align="right">
								<asp:Button ID="btnAlterJobTypeAndCharging" Runat="server" CausesValidation="false" Text="Alter" Width="75px"></asp:Button>
							</div>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="HEIGHT:10px"></div>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<fieldset>
								<legend>
									<strong>Client References</strong></legend>
								<table width="100%">
									<tr>
										<td valign="top" align="left" width="30%">
											<span>
													<asp:Label id="lblLoadNumberText" Runat="server" Text="Load Number"></asp:Label></span>
										</td>
										<td valign="top" align="left">
											<asp:Label ID="lblLoadNumber" runat="server"></asp:Label>
										</td>
									</tr>
									<asp:Repeater ID="repJobReferences" Runat="server">
										<HeaderTemplate>
										</HeaderTemplate>
										<ItemTemplate>
											<tr>
												<td valign="top" align="left"><%# DataBinder.Eval(Container.DataItem, "OrganisationReference.Description") %></td>
												<td valign="top" align="left"><%# DataBinder.Eval(Container.DataItem, "Value") %></td>
											</tr>
										</ItemTemplate>
									</asp:Repeater>
								</table>
								<div align="right">
									<asp:Button ID="btnAlterReferences" Runat="server" CausesValidation="false" Text="Alter" Width="75px"></asp:Button>
								</div>
							</fieldset>
						</td>
					</tr>
					<tr>
						<td colspan="2">
							<div class="spacer" style="HEIGHT:10px"></div>
						</td>
					</tr>
					<asp:Panel ID="pnlNormalJob" runat="server">
					    <tr>
						    <td colspan="2">
							    <fieldset>
								    <legend>
									    <strong>Collections</strong></legend>
								    <asp:Repeater ID="repCollections" Runat="server">
									    <HeaderTemplate>
										    <table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
											    <thead bgcolor="#ECE9D8">
												    <tr>
													    <th valign="top">Client</th>
													    <th valign="top" colspan="3">Collection Town</th>
													    <th valign="top" colspan="3">Collection Point</th>
													    <th>&nbsp;</th>
												    </tr>
											    </thead>
											    <tbody bgcolor="#FFFFFF">
									    </HeaderTemplate>
									    <ItemTemplate>
										    <tr>
											    <td valign="top" rowspan="4"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.OrganisationName %><input type="hidden" id="hidInstructionId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionID %>"></td>
											    <td valign="top" colspan="3"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.PostTown.TownName %></td>
											    <td valign="top" colspan="3"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.Description %></td>
											    <td valign="top" align="right" nowrap="nowrap">
											        <asp:Button ID="btnDown" Runat="server" CausesValidation="false" CommandName="Down" Text=" &#9660; " Height="24px"></asp:Button>
											        <asp:Button ID="btnUp" Runat="server" CausesValidation="false" CommandName="Up" Text=" &#9650; " Height="24px"></asp:Button>
											        <asp:Button ID="btnAlter" Runat="server" CausesValidation="false" CommandName="Alter" Text="&#8230;" ToolTip="Edit collection details" Height="24px"></asp:Button>
											        <asp:Button ID="btnDelete" Runat="server" CausesValidation="false" CommandName="Delete" Text=" x " Height="24px"></asp:Button>
                                                </td>
										    </tr>
										    <tr>
										        <td colspan="7">
										            <table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
											            <thead bgcolor="#ECE9D8">
												            <tr>
													            <th valign="top">Date/Time</th>
													            <th valign="top">Total Qty&nbsp;(cases)</th>
													            <th valign="top">Total Pallets</th>
													            <th valign="top">Total Weight</th>
												            </tr>
											            </thead>
											            <tbody bgcolor="#FFFFFF">
											                <tr>
											                    <td valign="top"><asp:Label ID="lblBookedDateTime" Runat="server"></asp:Label></td>
											                    <td valign="top" align="right"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalQuantityCases.ToString() %></td>
											                    <td valign="top" align="right"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalPallets.ToString() %></td>
											                    <td valign="top" align="right"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalWeight.ToString("N2") %></td>
											                </tr>
											            </tbody>
											        </table>
									            </td>
										    </tr>
										    <tr>
										        <td colspan="8" valign="top">
										            <table border="0" cellpadding="0" cellspacing="0">
										                <tr id="rowInstructionNote" runat="server">
											                <td>
											                    <b>NB:</b>&nbsp;<%# ((Orchestrator.Entities.Instruction) Container.DataItem).Note %>
											                </td>
										                </tr>
										            </table>
										        </td>
										    </tr>
										    <tr>
											    <td colspan="7">
												    <hr noshade>
												    <asp:Repeater ID="repCollectDrops" Runat="server" EnableViewState="False" DataSource="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).CollectDrops %>">
													    <HeaderTemplate>
														    <table width="100%" border="1" cellpadding="1" cellspacing="0" bgcolor="#ECE9D8" bordercolor="#FFFFFF">
															    <thead bgcolor="#ECE9D8">
																    <tr>
																	    <th valign="top">
																		    <%=(m_client == null ? "Docket" : m_client.DocketNumberText) %>
																	    </th>
																	    <th valign="top">
																	        Goods Type
																	    </th>
																	    <th valign="top">
																		    Qty (cases)</th>
																	    <th valign="top">
																		    Pallets</th>
																	    <th valign="top">
																		    Weight</th>
																    </tr>
															    </thead>
															    <tbody bgcolor="#FFFFFF">
													    </HeaderTemplate>
													    <ItemTemplate>
																    <tr>
																	    <td><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Docket %></td>
																	    <td><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsTypeDescription %></td>
																	    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoCases %></td>
																	    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoPallets %></td>
																	    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Weight.ToString("N2") %></td>
																    </tr>
													    </ItemTemplate>
													    <FooterTemplate>
															    </tbody>
														    </table>
													    </FooterTemplate>
												    </asp:Repeater>
											    </td>
										    </tr>
									    </ItemTemplate>
									    <SeparatorTemplate>
										    <tr>
											    <td colspan="8"><hr noshade>
											    </td>
										    </tr>
									    </SeparatorTemplate>
									    <FooterTemplate>
										    </table>
									    </FooterTemplate>
								    </asp:Repeater>
								    <div align="right">
									    <asp:Button ID="btnAddCollection" Runat="server" CausesValidation="false" Text="Add Collection"></asp:Button>
								    </div>
							    </FIELDSET>
						    </TD>
					    </TR>
					    <tr>
						    <td colspan="2">
							    <div class="spacer" style="HEIGHT:10px"></div>
						    </td>
					    </tr>
					    <tr>
						    <td colspan="2">
							    <fieldset>
								    <legend>
									    <strong>Deliveries</strong></legend>
									    <asp:Repeater ID="repDeliveries" Runat="server">
										    <HeaderTemplate>
											    <table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
												    <thead bgcolor="#ECE9D8">
													    <tr>
														    <th valign="top">Client's Customer</th>
														    <th valign="top" colspan="3">Delivery Town</th>
														    <th valign="top" colspan="3">Delivery Point</th>
														    <th>&nbsp;</th>
													    </tr>
												    </thead>
												    <tbody bgcolor="#FFFFFF">
										    </HeaderTemplate>
										    <ItemTemplate>
											    <tr>
												    <td valign="top" rowspan="4"><asp:Label ID="lblClientsCustomer" Runat="server"></asp:Label><input type="hidden" id="hidInstructionId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionID %>" NAME="Hidden1"></td>
												    <td valign="top" colspan="3"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.PostTown.TownName %></td>
												    <td valign="top" colspan="3"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.Description %></td>
												    <td valign="top" align="right" nowrap="nowrap">
												        <asp:Button ID="btnDown" Runat="server" CausesValidation="false" CommandName="Down" Text=" &#9660; " Height="24px"></asp:Button>
												        <asp:Button ID="btnUp" Runat="server" CausesValidation="false" CommandName="Up" Text=" &#9650; " Height="24px"></asp:Button>
												        <asp:Button ID="btnAlter" Runat="server" CausesValidation="false" CommandName="Alter" Text="&#8230;" ToolTip="Edit delivery details" Height="24px"></asp:Button>
												        <asp:Button ID="btnDelete" Runat="server" CausesValidation="false" CommandName="Delete" Text=" x " Height="24px"></asp:Button>
												    </td>
											    </tr>
											    <tr>
    										        <td colspan="7">
										                <table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
											                <thead bgcolor="#ECE9D8">
												                <tr>
													                <th valign="top">Date/Time</th>
													                <th valign="top">Total Qty&nbsp;(cases)</th>
													                <th valign="top">Total Pallets</th>
													                <th valign="top">Total Weight</th>
												                </tr>
											                </thead>
											                <tbody bgcolor="#FFFFFF">
											                    <tr>
												                    <td valign="top"><asp:Label ID="lblBookedDateTime" Runat="server"></asp:Label></td>
												                    <td valign="top" align="right"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalQuantityCases.ToString() %></td>
												                    <td valign="top" align="right"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalPallets.ToString() %></td>
												                    <td valign="top" align="right"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).TotalWeight.ToString("N2") %></td>
											                    </tr>
										                    </tbody>
                                                        </table>
							                        </td>
											    </tr>
										        <tr>
										            <td colspan="8" valign="top">
										                <table border="0" cellpadding="0" cellspacing="0">
											                <tr id="rowInstructionNote" runat="server">
												                <td valign="top"><b>NB:</b>&nbsp;<%# ((Orchestrator.Entities.Instruction) Container.DataItem).Note %></td>
											                </tr>
										                </table>
									                </td>
								                </tr>
											    <tr>
												    <td colspan="7">
													    <hr noshade>
													    <asp:Repeater ID="repCollectDrops" Runat="server" EnableViewState="False" DataSource="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).CollectDrops %>">
														    <HeaderTemplate>
															    <table width="100%" border="1" cellpadding="1" cellspacing="0" bgcolor="#ECE9D8" bordercolor="#FFFFFF">
																    <thead bgcolor="#ECE9D8">
																	    <tr>
																		    <th valign="top"><%=(m_client == null ? "Docket" : m_client.DocketNumberText) %></th>
																	        <th valign="top">Goods Type</th>
																	        <th valign="top">Qty (cases)</th>
																		    <th valign="top">Pallets</th>
																		    <th valign="top">Weight</th>
																	    </tr>
																    </thead>
																    <tbody bgcolor="#FFFFFF">
														    </HeaderTemplate>
														    <ItemTemplate>
															    <tr>
																    <td><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Docket %></td>
															        <td><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsTypeDescription %></td>
																    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoCases %></td>
																    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoPallets %></td>
																    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Weight.ToString("N2") %></td>
															    </tr>
														    </ItemTemplate>
														    <FooterTemplate>
															    </tbody> </table>
														    </FooterTemplate>
													    </asp:Repeater>
												    </td>
											    </tr>
										    </ItemTemplate>
										    <SeparatorTemplate>
											    <tr>
												    <td colspan="8"><hr noshade="noshade"></td>
											    </tr>
										    </SeparatorTemplate>
										    <FooterTemplate>
											    </tbody> </table>
										    </FooterTemplate>
									    </asp:Repeater>
									    <div align="right">
										    <asp:Button ID="btnAddDelivery" Runat="server" CausesValidation="false" Text="Add Delivery"></asp:Button>
									    </div>
								    </fieldset>
							    </td>
						    </tr>
                        </asp:Panel>
                        <asp:Panel ID="pnlGroupageJob" runat="server">
						    <td colspan="2">
							    <fieldset>
								    <legend>
									    <strong>Order Handling</strong></legend>
									    <asp:Repeater ID="repOrderHandling" Runat="server">
										    <HeaderTemplate>
											    <table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
												    <thead bgcolor="#ECE9D8">
													    <tr>
														    <th valign="top">Location Owner</th>
														    <th valign="top" colspan="3">Town</th>
														    <th valign="top" colspan="3">Point</th>
														    <th>&nbsp;</th>
													    </tr>
												    </thead>
												    <tbody bgcolor="#FFFFFF">
										    </HeaderTemplate>
										    <ItemTemplate>
											    <tr>
												    <td valign="top"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.OrganisationName %><input type="hidden" id="hidInstructionId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionID %>"><input type="hidden" id="hidInstructionTypeId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionTypeId %>"></td>
												    <td valign="top" colspan="3"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.PostTown.TownName %></td>
												    <td valign="top" colspan="3"><%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.Description %></td>
												    <td valign="top" align="right" nowrap="nowrap">
												        <asp:Button ID="btnDown" Runat="server" CausesValidation="false" CommandName="Down" Text=" &#9660; " Tooltip="Move this order handling instruction down" Height="24px"></asp:Button>
												        <asp:Button ID="btnUp" Runat="server" CausesValidation="false" CommandName="Up" Text=" &#9650; " Tooltip="Move this order handling instruction up" Height="24px"></asp:Button>
												        <asp:Button ID="btnMerge" runat="server" CausesValidation="false" CommandName="Merge" Text=" ? " Tooltip="Merge this order handling instruction with the following instruction" Height="24px" />
												        <asp:Button ID="btnAlter" Runat="server" CausesValidation="false" CommandName="Alter" Text="&#8230;" ToolTip="Edit order handling" Height="24px"></asp:Button>
												    </td>
											    </tr>
											    <tr>
											        <td valign="top">
										                <table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
											                <tr>
												                <th bgcolor="#ECE9D8" valign="top">Perform</th>
												                <td bgcolor="#FFFFFF"><asp:Label ID="lblInstructionType" Runat="server" Font-Bold="true"></asp:Label></td>
											                </tr>
											                <tr>
												                <th bgcolor="#ECE9D8" valign="top">Date/Time</th>
												                <td bgcolor="#FFFFFF"><asp:Label ID="lblBookedDateTime" Runat="server"></asp:Label></td>
											                </tr>
											                <tr>
												                <th bgcolor="#ECE9D8" valign="top">Pallets On</th>
												                <td bgcolor="#FFFFFF"><asp:Label ID="lblPalletsOn" Runat="server"></asp:Label></td>
											                </tr>
                                                        </table>
											        </td>
												    <td colspan="7" valign="top">
													    <asp:Repeater ID="repOrderCollectDrops" Runat="server" EnableViewState="False" DataSource="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).CollectDrops %>" OnItemDataBound="repOrderCollectDrops_ItemDataBound">
														    <HeaderTemplate>
															    <table width="100%" border="1" cellpadding="1" cellspacing="0" bgcolor="#ECE9D8" bordercolor="#FFFFFF">
																    <thead bgcolor="#ECE9D8">
																	    <tr>
																	        <th valign="top">Action</th>
																		    <th valign="top">Customer Ord No</th>
																	        <th valign="top">Goods Type</th>
																		    <th valign="top">Pallets</th>
																		    <th valign="top">Weight</th>
																	    </tr>
																    </thead>
																    <tbody bgcolor="#FFFFFF">
														    </HeaderTemplate>
														    <ItemTemplate>
															    <tr>
																    <td><asp:Label ID="lblOrderAction" runat="server" /></td>
																    <td><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.CustomerOrderNumber %> [<a href="../../groupage/ManageOrder.aspx?oID=<%#((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.OrderID.ToString()  %>" target="_blank"><%#((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.OrderID.ToString()  %></a>] </td>
															        <td><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsTypeDescription %></td>
																    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoPallets %></td>
																    <td align="right"><%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Weight.ToString("N2") %><asp:Label ID="lblWeightCode" runat="server" /></td>
															    </tr>
														    </ItemTemplate>
														    <FooterTemplate>
															    </tbody> </table>
														    </FooterTemplate>
													    </asp:Repeater>
												    </td>
											    </tr>
										    </ItemTemplate>
										    <SeparatorTemplate>
											    <tr>
												    <td colspan="8"><hr noshade="noshade"></td>
											    </tr>
										    </SeparatorTemplate>
										    <FooterTemplate>
											    </tbody> </table>
										    </FooterTemplate>
									    </asp:Repeater>
									    <div align="right">
										    <asp:Button ID="btnAddGroupageCollection" Runat="server" CausesValidation="false" Text="Add Collection"></asp:Button>
									    </div>
								    </fieldset>
							    </td>
					    </asp:Panel>
					    <tr>
						    <td colspan="2">
							    <div class="spacer" style="HEIGHT:10px"></div>
						    </td>
					    </tr>
					    <tr id="trRateAnalysis" runat="server">
					        <td colspan="2">
					            <fieldset>
					                <legend><strong>Rate Analysis</strong></legend>
					                <asp:Label ID="lblRateAnalysis" runat="server"></asp:Label><img id="imgRatesRequired" runat="server" src="../../../images/error.gif" alt="Please configure the rates, or check the checkbox below." />
					                <br />
					                <asp:Repeater ID="repRates" runat="server">
					                    <HeaderTemplate>
											<table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
												<thead bgcolor="#ECE9D8">
													<tr>
														<th valign="top">Collection Point</th>
														<th valign="top">Delivery Point</th>
														<th valign="top">&nbsp;</th>
													</tr>
												</thead>
												<tbody bgcolor="#FFFFFF">
					                    </HeaderTemplate>
					                    <ItemTemplate>
					                                <tr>
					                                    <td valign="top">
					                                        <span id="spnCollectionPoint" runat="server"><%# DataBinder.Eval(Container.DataItem, "CollectionPoint") %></span>
					                                        <input type="hidden" runat="server" id="hidCollectionPointId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "CollectionPointId") %>' />
					                                        <input type="hidden" runat="server" id="hidDeliveryPointId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "DeliveryPointId") %>' />
					                                    </td>
					                                    <td valign="top">
					                                        <span id="spnDeliveryPoint" runat="server"><%# DataBinder.Eval(Container.DataItem, "DeliveryPoint") %></span>
					                                    </td>
					                                    <td valign="top" align="center" width="80">
    					                                    <asp:Button ID="btnSetRate" runat="server" CommandName="setRate" CausesValidation="False" Text="Set Rate" Width="75px" Height="24px" />
					                                    </td>
					                                </tr>
					                    </ItemTemplate>
					                    <FooterTemplate>
					                            </tbody>
                                            </table>
					                    </FooterTemplate>
					                </asp:Repeater>
					                <asp:CheckBox ID="chkManualRateEntry" runat="server" Text="Ignore rate requirements" TextAlign="Right" />
					            </fieldset>
					        </td>
					    </tr>
						<tr>
							<td colspan="2">
								<fieldset>
									<legend><strong>Control Area</strong></legend>
									Specify Control Area: <asp:DropDownList id="cboControlArea" Runat="server" DataValueField="ControlAreaId" DataTextField="Description"></asp:DropDownList>
								</fieldset>
							</td>
						</tr>
						<tr id="trDuplicateJobs" runat="server">
						    <td colspan="2">
						        How many copies of this job do you want? <asp:TextBox ID="txtDuplicateJobCount" runat="server" Width="60px"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDuplicateJobCount" runat="server" ControlToValidate="txtDuplicateJobCount" Display="dynamic" ErrorMessage="Please supply how many copies of the job you would like."><img src="../../images/error.gif" alt="Please supply how many copies of the job you would like." /></asp:RequiredFieldValidator><asp:CustomValidator ID="cfvDuplicateJobCount" runat="server" ControlToValidate="txtDuplicateJobCount" Display="dynamic" EnableClientScript="false" ErrorMessage="Please supply how many copies of the job you would like."><img src="../../images/error.gif" alt="Please supply how many copies of the job you would like." /></asp:CustomValidator>
						    </td>
						</tr>
						<tr>
							<td colspan="2">
								<div class="spacer" style="HEIGHT:10px"></div>
							</td>
                        </tr>
					</TBODY>
				</TABLE>
			</DIV>
		</TD>
	</TR>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td align="left" height="46" style="PADDING-LEFT:10px">
			<asp:Button ID="btnAddAnotherJob" Runat="server" CausesValidation="false" Text="Add Another Job"></asp:Button>
			<asp:Button ID="btnResourceThis" runat="server" CausesValidation="false" Text="Resource This Job" />
		</td>
		<td align="right" height="46" style="PADDING-RIGHT:10px">
			<asp:Button ID="btnFinish" Runat="server" Text="Finish" Width="75px"></asp:Button>
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="false" Text="Cancel" Width="75px"></asp:Button>
			<asp:Button ID="btnClose" Runat="server" CausesValidation="false" Text="Close" Width="75px"></asp:Button>
		</td>
	</tr>
</TABLE>
<script language="javascript" type="text/javascript">
<!--
	function HidePage()
	{
		document.getElementById("tblMain").style.display = "none";
		document.getElementById("tblRotate").style.display = "block";
	}

	height = 850;
//-->
</script>
<table width="596" height="672" cellpadding="0" cellspacing="0" border="0" id="tblRotate"
	style="DISPLAY: none">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Processing
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Your actions are being processed, please wait.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="505" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
			width="100%"></td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px"></td>
	</tr>
</table>
<div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
	<table style="background-color: white; border:solid 1pt black; " cellpadding="2">
		<tr>
			<td><span id="spnPointAddress"></span></td>
		</tr>
	</table>
</div>