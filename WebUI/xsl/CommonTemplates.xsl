<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes" />

	<xsl:template name="DateDisplay">

		<xsl:value-of select="@day" />
		<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
		<xsl:call-template name="MonthDisplay">
			<xsl:with-param name="monthOrdinal"><xsl:value-of select="@month" /></xsl:with-param>
		</xsl:call-template>
		<xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
		<xsl:value-of select="@year" />

		<xsl:element name="br"/>

		<xsl:choose>
			<xsl:when test="@anyTime = 'True'">Anytime</xsl:when>
			<xsl:otherwise><xsl:value-of select="@time" /></xsl:otherwise>
		</xsl:choose>
	</xsl:template>
	
	<xsl:template name="MonthDisplay">
		<xsl:param name="monthOrdinal" />
		
		<xsl:choose>
			<xsl:when test="$monthOrdinal = 1">January</xsl:when>
			<xsl:when test="$monthOrdinal = 2">February</xsl:when>
			<xsl:when test="$monthOrdinal = 3">March</xsl:when>
			<xsl:when test="$monthOrdinal = 4">April</xsl:when>
			<xsl:when test="$monthOrdinal = 5">May</xsl:when>
			<xsl:when test="$monthOrdinal = 6">June</xsl:when>
			<xsl:when test="$monthOrdinal = 7">July</xsl:when>
			<xsl:when test="$monthOrdinal = 8">August</xsl:when>
			<xsl:when test="$monthOrdinal = 9">September</xsl:when>
			<xsl:when test="$monthOrdinal = 10">October</xsl:when>
			<xsl:when test="$monthOrdinal = 11">November</xsl:when>
			<xsl:when test="$monthOrdinal = 12">December</xsl:when>
		</xsl:choose>		
	</xsl:template>

</xsl:stylesheet>

  