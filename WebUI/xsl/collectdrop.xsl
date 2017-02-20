<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes" />
	
	<xsl:include href="CommonTemplates.xsl" />

	<xsl:template match="collectDrop">
		<xsl:element name="table">
			<xsl:attribute name="cellpadding">1</xsl:attribute>
			<xsl:attribute name="cellspacing">0</xsl:attribute>
			
			<xsl:element name="tr">
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:text disable-output-escaping="yes">Must be completed:&amp;nbsp;</xsl:text>
				</xsl:element>
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:apply-templates select="dateTime" />
				</xsl:element>
			</xsl:element>
			
			<xsl:element name="tr">
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:text disable-output-escaping="yes">Weight:&amp;nbsp;</xsl:text>
				</xsl:element>
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:value-of select="@weight" />
				</xsl:element>
			</xsl:element>
			
			<xsl:element name="tr">
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:text disable-output-escaping="yes">Number of Cases:&amp;nbsp;</xsl:text>
				</xsl:element>
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:value-of select="@numberOfCases" />
				</xsl:element>
			</xsl:element>
			
			<xsl:element name="tr">
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:text disable-output-escaping="yes">Number of Pallets:&amp;nbsp;</xsl:text>
				</xsl:element>
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:value-of select="@numberOfPallets" />
				</xsl:element>
			</xsl:element>
			
			<xsl:element name="tr">
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:text disable-output-escaping="yes">Clients Customer Reference:&amp;nbsp;</xsl:text>
				</xsl:element>
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:value-of select="clientsCustomerReference" />
				</xsl:element>
			</xsl:element>
			
			<xsl:element name="tr">
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:text disable-output-escaping="yes">Planner Notes:&amp;nbsp;</xsl:text>
				</xsl:element>
				<xsl:element name="td">
					<xsl:attribute name="valign">top</xsl:attribute>
					
					<xsl:value-of select="plannerNotes" />
				</xsl:element>
			</xsl:element>
			
		</xsl:element>
	</xsl:template>
	
	<xsl:template match="dateTime">
		<xsl:call-template name="DateDisplay" />
	</xsl:template>
	
</xsl:stylesheet>