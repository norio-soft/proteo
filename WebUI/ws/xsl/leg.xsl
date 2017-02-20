<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://xsltsl.org/date-time">
	<!-- Import part of the XSLT Standard Library http://xsltsl.sourceforge.net/ -->
	<xsl:import href="sl/date-time.xsl"/>
	<xsl:include href="instructions.xsl"/>
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes"/>

	<!-- Leg Id -->
	<xsl:param name="LegId"></xsl:param>
	
	<xsl:template match="Job">
		<div align="center">
			<xsl:apply-templates select="//Leg[LegId = $LegId]" />
		</div>
	</xsl:template>
	
	<xsl:template match="Leg">
		<xsl:element name="span">
			<xsl:attribute name="id">spanLeg<xsl:value-of select="LegId" /></xsl:attribute>
			
			<fieldset>
				<legend>Selected Leg - <xsl:value-of select="LegPoints/LegPoint[1]/Point/PostTown/TownName" /> to <xsl:value-of select="LegPoints/LegPoint[2]/Point/PostTown/TownName" /></legend>
				<table width="99%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
					<tbody>
						<tr>
							<td width="50%" valign="top">
								<fieldset>
									<legend>Leg</legend>
									<table width="100%">
											<tr>
												<td width="50%">Leg State</td>
												<td><span style="FONT-WEIGHT:bold; font-size:12px"><xsl:value-of select="LegState" /></span></td>
											</tr>
											<tr>
												<td>Leg Type</td>
												<td><xsl:value-of select="LegType" /></td>
											</tr>
											<tr>
												<td>Start Time</td>
												<td>
													<xsl:call-template name="dt:format-date-time">
														<xsl:with-param name="xsd-date-time" select="PlannedStartDateTime"/>
														<xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
													</xsl:call-template>
												</td>
											</tr>
											<tr>
												<td>End Time</td>
												<td>
													<xsl:call-template name="dt:format-date-time">
														<xsl:with-param name="xsd-date-time" select="PlannedEndDateTime"/>
														<xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
													</xsl:call-template>
												</td>
											</tr>
									</table>
								</fieldset>	
							</td>
							<td width="50%" valign="top">
								<fieldset>
									<legend>Resources</legend>
									<table width="100%">
											<tr>
												<td width="50%">Driver</td>
												<td>
													<xsl:if test="Driver">
														<xsl:element name="a">
															<xsl:attribute name="href"><xsl:value-of select="$webserver" />/resource/driver/addupdatedriver.aspx?identityId=<xsl:value-of select="Driver/Individual/IdentityId" /></xsl:attribute>

															<xsl:value-of select="Driver/Individual/FirstNames" /><xsl:text> </xsl:text><xsl:value-of select="Driver/Individual/LastName" />
														</xsl:element>
													</xsl:if>
												</td>
											</tr>
											<tr>
												<td>Vehicle</td>
												<td>
													<xsl:if test="Vehicle">
														<xsl:element name="a">
															<xsl:attribute name="href"><xsl:value-of select="$webserver" />/resource/vehicle/addupdatevehicle.aspx?resourceId=<xsl:value-of select="Vehicle/ResourceId" /></xsl:attribute>

															<xsl:value-of select="Vehicle/RegNo" />
														</xsl:element>
													</xsl:if>
												</td>
											</tr>
											<tr>
												<td>Trailer</td>
												<td>
													<xsl:if test="Trailer">
														<xsl:element name="a">
															<xsl:attribute name="href"><xsl:value-of select="$webserver" />/resource/trailer/addupdatetrailer.aspx?resourceId=<xsl:value-of select="Trailer/ResourceId" /></xsl:attribute>
															
															<xsl:value-of select="Trailer/TrailerRef"	/>
														</xsl:element>
													</xsl:if>
												</td>
											</tr>
									</table>
								</fieldset>	
							</td>
						</tr>
						<xsl:for-each select="LegPoints/LegPoint">
							<xsl:sort select="LegOrderId" />

							<xsl:element name="tr">
								<xsl:element name="td">
									<xsl:attribute name="colspan">2</xsl:attribute>

									<table width="100%" border="0" cellpadding="1" cellspacing="2">
										<tbody>
											<xsl:choose>
												<xsl:when test="Instructions/Instruction">
													<xsl:apply-templates select="Instructions/Instruction[1]/CollectDropSummary">
														<xsl:with-param name="InstructionTypeId" select="Instructions/Instruction[1]/InstructionTypeId"/>
														<xsl:with-param name="DisplayInstructionType" select="'true'"/>
													</xsl:apply-templates>
												</xsl:when>
												<xsl:otherwise>
													<xsl:apply-templates select="." />
												</xsl:otherwise>	
											</xsl:choose>		
											<tr>
												<td colspan="2"><hr noshade="noshade"	/></td>
											</tr>
										</tbody>
									</table>		
								</xsl:element>
							</xsl:element>
						</xsl:for-each>
						<tr>
							<td colspan="2">
								<div class="buttonbar">
									<xsl:element name="input">
										<xsl:attribute name="type">button</xsl:attribute>
										<xsl:attribute name="value">Close</xsl:attribute>
										<xsl:attribute name="onClick">document.getElementById('spanLeg<xsl:value-of select="LegId"/>').style.display = 'none';</xsl:attribute>	
										
									</xsl:element>
								</div>
							</td>
						</tr>
					</tbody>
				</table>
			</fieldset>
		</xsl:element>
	</xsl:template>

	<xsl:template match="LegPoint">
		<td width="30%" valign="top">
			<xsl:apply-templates select="Point" />
		</td>
		<td width="70%" valign="top">
			<xsl:text> </xsl:text>
		</td>
	</xsl:template>

</xsl:stylesheet>

  