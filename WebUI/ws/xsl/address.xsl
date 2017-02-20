<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes" />
	
	<xsl:template match="address">
		<xsl:apply-templates />
	</xsl:template>
	
	<xsl:template match="addressLine1">
		<xsl:value-of select="." />
		<xsl:text>,</xsl:text>
		<xsl:element name="br" />
	</xsl:template>

	<xsl:template match="addressLine2">
		<xsl:value-of select="." />
		<xsl:text>,</xsl:text>
		<xsl:element name="br" />
	</xsl:template>

	<xsl:template match="addressLine3">
		<xsl:value-of select="." />
		<xsl:text>,</xsl:text>
		<xsl:element name="br" />
	</xsl:template>

	<xsl:template match="postTown">
		<xsl:value-of select="." />
		<xsl:text>,</xsl:text>
		<xsl:element name="br" />
	</xsl:template>

	<xsl:template match="county">
		<xsl:value-of select="." />
		<xsl:text>,</xsl:text>
		<xsl:element name="br" />
	</xsl:template>

	<xsl:template match="postCode">
		<xsl:value-of select="." />
		<xsl:element name="br" />
	</xsl:template>

</xsl:stylesheet>