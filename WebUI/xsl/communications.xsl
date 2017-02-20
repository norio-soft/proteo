<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://xsltsl.org/date-time">
	<!-- Import part of the XSLT Standard Library http://xsltsl.sourceforge.net/ -->
	<xsl:import href="sl/date-time.xsl"/>
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes"/>

	<!-- Communiction Type -->
	<xsl:param name="CommunicationTypeId">1</xsl:param>
	<!-- Resource Id -->
	<xsl:param name="ResourceId"></xsl:param>
	
	<xsl:template match="Job">
		<xsl:element name="communication">
			<xsl:for-each select="Legs/Leg[Driver/ResourceId = $ResourceId and (LegState = 'Planned' or LegState = 'Completed')]">
				<xsl:sort select="LegOrderId" />

				<xsl:choose>
					<xsl:when test="$CommunicationTypeId = 1">
						<!-- Comments -->
						<xsl:apply-templates select="." mode="Comments" />
					</xsl:when>
					<xsl:when test="$CommunicationTypeId = 2">
						<!-- SMS Text -->
						<xsl:apply-templates select="." mode="SMSText" />
					</xsl:when>
					<xsl:when test="$CommunicationTypeId = 3">
						<!-- In Person -->
						<xsl:apply-templates select="." mode="Comments" />
					</xsl:when>
				</xsl:choose>
			</xsl:for-each>
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="Leg" mode="Comments">
		<xsl:if test="Vehicle or Trailler">
			<xsl:text>Take </xsl:text>
	
			<xsl:if test="Vehicle">
				<xsl:value-of select="Vehicle/RegNo" />
			</xsl:if>
			<xsl:if test="Vehicle and Trailer">
				<xsl:text> and </xsl:text>
			</xsl:if>
			<xsl:if test="Trailer">
				<xsl:value-of select="Trailer/TrailerRef" />
			</xsl:if>
		</xsl:if>

		<xsl:text>From </xsl:text>
		<xsl:value-of select="LegPoints/LegPoint[1]/Point/OrganisationName" />, <xsl:value-of select="LegPoints/LegPoint[1]/Point/Address/PostTown" />, <xsl:value-of select="LegPoints/LegPoint[1]/Point/Description" />
		<xsl:text> at </xsl:text>
		<xsl:call-template name="dt:format-date-time">
			<xsl:with-param name="xsd-date-time" select="PlannedStartDateTime"/>
			<xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
		</xsl:call-template>

		<xsl:choose>
			<xsl:when test="LegPoints/LegPoint[1]/Instructions/Instruction/InstructionTypeId = 1"><xsl:text>For LOAD</xsl:text></xsl:when>
			<xsl:when test="LegPoints/LegPoint[1]/Instructions/Instruction/InstructionTypeId = 2"><xsl:text>For DROP</xsl:text></xsl:when>
		</xsl:choose>	

		<xsl:text>To </xsl:text>
		<xsl:value-of select="LegPoints/LegPoint[2]/Point/OrganisationName" />, <xsl:value-of select="LegPoints/LegPoint[2]/Point/Address/PostTown" />, <xsl:value-of select="LegPoints/LegPoint[2]/Point/Description" />
		<xsl:text> at </xsl:text>
		<xsl:call-template name="dt:format-date-time">
			<xsl:with-param name="xsd-date-time" select="PlannedEndDateTime"/>
			<xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
		</xsl:call-template>

		<xsl:choose>
			<xsl:when test="LegPoints/LegPoint[2]/Instructions/Instruction[1]/InstructionTypeId = 1"><xsl:text>For LOAD</xsl:text></xsl:when>
			<xsl:when test="LegPoints/LegPoint[2]/Instructions/Instruction[1]/InstructionTypeId = 2"><xsl:text>For DROP</xsl:text></xsl:when>
		</xsl:choose>	
	</xsl:template>
	
	<xsl:template match="Leg" mode="SMSText">
		<xsl:if test="Vehicle or Trailler">
			<xsl:if test="Vehicle">
				<xsl:value-of select="Vehicle/RegNo" />
			</xsl:if>
			<xsl:if test="Vehicle and Trailer">
				<xsl:text>,</xsl:text>
			</xsl:if>
			<xsl:if test="Trailer">
				<xsl:value-of select="Trailer/TrailerRef"	/>
			</xsl:if>
		</xsl:if>

		<xsl:value-of select="LegPoints/LegPoint[1]/Point/OrganisationName" />, <xsl:value-of select="LegPoints/LegPoint[1]/Point/Address/PostTown" />
		<xsl:text>@</xsl:text>
		<xsl:call-template name="dt:format-date-time">
			<xsl:with-param name="xsd-date-time" select="PlannedStartDateTime"/>
			<xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
		</xsl:call-template>

		<xsl:choose>
			<xsl:when test="LegPoints/LegPoint[1]/Instructions/Instruction[1]/InstructionTypeId = 1"><xsl:text>4L</xsl:text></xsl:when>
			<xsl:when test="LegPoints/LegPoint[1]/Instructions/Instruction[1]/InstructionTypeId = 2"><xsl:text>4D</xsl:text></xsl:when>
		</xsl:choose>	
		
		<xsl:text disable-output-escaping="yes"> to </xsl:text>

		<xsl:value-of select="LegPoints/LegPoint[2]/Point/OrganisationName" />, <xsl:value-of select="LegPoints/LegPoint[2]/Point/Address/PostTown" />
		<xsl:text>@</xsl:text>
		<xsl:call-template name="dt:format-date-time">
			<xsl:with-param name="xsd-date-time" select="PlannedEndDateTime"/>
			<xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
		</xsl:call-template>

		<xsl:choose>
			<xsl:when test="LegPoints/LegPoint[2]/Instructions/Instruction[1]/InstructionTypeId = 1"><xsl:text>4L</xsl:text></xsl:when>
			<xsl:when test="LegPoints/LegPoint[2]/Instructions/Instruction[1]/InstructionTypeId = 2"><xsl:text>4D</xsl:text></xsl:when>
		</xsl:choose>	
	</xsl:template>
</xsl:stylesheet>