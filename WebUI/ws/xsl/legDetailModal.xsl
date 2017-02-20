<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes" />
	
	<xsl:include href="address.xsl" />
	<xsl:include href="CommonTemplates.xsl" />
	
	<xsl:template match="leg">
	
		<xsl:element name="fieldset">
			<xsl:element name="legend">Leg Information</xsl:element>
			
			<xsl:element name="table">
				<xsl:attribute name="border">0</xsl:attribute>
				<xsl:attribute name="cellpadding">1</xsl:attribute>
				<xsl:attribute name="cellspacing">0</xsl:attribute>
				<xsl:attribute name="width">100%</xsl:attribute>
				
				<xsl:element name="tr">
					<xsl:apply-templates select="startDate" />
					<xsl:apply-templates select="endDate" />
				</xsl:element>
				
				<xsl:element name="tr">
				
					<xsl:apply-templates select="points/point" />
					
				</xsl:element>
				
				<xsl:element name="tr">
				
					<xsl:apply-templates select="points/point/instructions" />
				
				</xsl:element>
				
			</xsl:element>
		</xsl:element>
		
		<xsl:apply-templates select="resources" />
	</xsl:template>
	
	<xsl:template match="startDate">
		<xsl:element name="td">
			<xsl:attribute name="width">50%</xsl:attribute>
			
			<xsl:call-template name="DateDisplay" />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="endDate">
		<xsl:element name="td">
			<xsl:attribute name="width">50%</xsl:attribute>
			
			<xsl:call-template name="DateDisplay" />
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="point">
		
		<xsl:element name="td">
			<xsl:attribute name="valign">top</xsl:attribute>
			<xsl:attribute name="width">50%</xsl:attribute>
		
			<xsl:apply-templates select="address" />

		</xsl:element>
		
	</xsl:template>
	
	<xsl:template match="instructions">
	
		<xsl:element name="td">
			<xsl:attribute name="valign">top</xsl:attribute>
			<xsl:attribute name="width">50%</xsl:attribute>
		
			<xsl:element name="hr">
				<xsl:attribute name="noshade">noshade</xsl:attribute>
				
			</xsl:element>
			
			<xsl:apply-templates />

		</xsl:element>
	</xsl:template>
	
	<xsl:template match="instruction">
		<!--
		<xsl:element name="a">
			<xsl:attribute name="href">
				<xsl:text>addupdateinstructionactual.aspx?instructionId=</xsl:text>
				<xsl:value-of select="@id" />
			</xsl:attribute>
		-->
			<xsl:value-of select="@instruction" />
		<!--
		</xsl:element>
		-->

		<xsl:element name="br" />
	</xsl:template>
	
	<xsl:template match="resources">
		<xsl:element name="fieldset">
			<xsl:element name="legend">Resources</xsl:element>

			<xsl:element name="table">
				<xsl:attribute name="border">0</xsl:attribute>
				<xsl:attribute name="cellpadding">1</xsl:attribute>
				<xsl:attribute name="cellspacing">0</xsl:attribute>
				<xsl:attribute name="width">100%</xsl:attribute>
				
				<xsl:apply-templates select="driver" />
				<xsl:apply-templates select="vehicle" />
				<xsl:apply-templates select="trailer" />
				
			</xsl:element>		
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="driver">
		<xsl:element name="tr">
			<xsl:element name="td">
				<xsl:attribute name="align">left</xsl:attribute>
				<xsl:attribute name="valign">top</xsl:attribute>
				
				<xsl:text>Driver:</xsl:text>
			</xsl:element>
			<xsl:element name="td">
				<xsl:attribute name="align">left</xsl:attribute>
				<xsl:attribute name="valign">top</xsl:attribute>
				
				<xsl:choose>
					<xsl:when test="@assigned = 'True'">
						<xsl:value-of select="." />
						<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
						<!--
						<xsl:element name="a">
							<xsl:attribute name="href">
								<xsl:text>listDriverCommunications.aspx?driverId=</xsl:text>
								<xsl:value-of select="@id" />
								<xsl:text disable-output-escaping="yes">&amp;legId=</xsl:text>
								<xsl:value-of select="//leg/@id" />
							</xsl:attribute>
							<xsl:text>Communications</xsl:text>
						</xsl:element>
						-->
					</xsl:when>
					<xsl:otherwise>
						<xsl:element name="i">There is no driver assigned to this leg</xsl:element>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:element>
	</xsl:template>

	<xsl:template match="vehicle">
		<xsl:element name="tr">
			<xsl:element name="td">
				<xsl:attribute name="align">left</xsl:attribute>
				<xsl:attribute name="valign">top</xsl:attribute>
				
				<xsl:text>Vehicle:</xsl:text>
			</xsl:element>
			<xsl:element name="td">
				<xsl:attribute name="align">left</xsl:attribute>
				<xsl:attribute name="valign">top</xsl:attribute>
				
				<xsl:choose>
					<xsl:when test="@assigned = 'True'">
						<xsl:value-of select="." />
					</xsl:when>
					<xsl:otherwise>
						<xsl:element name="i">There is no vehicle assigned to this leg</xsl:element>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:element>
	</xsl:template>

	<xsl:template match="trailer">
		<xsl:element name="tr">
			<xsl:element name="td">
				<xsl:attribute name="align">left</xsl:attribute>
				<xsl:attribute name="valign">top</xsl:attribute>
				
				<xsl:text>Trailer:</xsl:text>
			</xsl:element>
			<xsl:element name="td">
				<xsl:attribute name="align">left</xsl:attribute>
				<xsl:attribute name="valign">top</xsl:attribute>
				
				<xsl:choose>
					<xsl:when test="@assigned = 'True'">
						<xsl:value-of select="." />
					</xsl:when>
					<xsl:otherwise>
						<xsl:element name="i">There is no trailer assigned to this leg</xsl:element>
					</xsl:otherwise>
				</xsl:choose>
			</xsl:element>
		</xsl:element>
	</xsl:template>

</xsl:stylesheet>

  