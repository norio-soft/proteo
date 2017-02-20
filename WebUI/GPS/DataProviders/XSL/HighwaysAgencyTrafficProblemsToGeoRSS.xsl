<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:xs="http://www.w3.org/2001/XMLSchema" xmlns:fn="http://www.w3.org/2005/xpath-functions" xmlns:xdt="http://www.w3.org/2005/xpath-datatypes" xmlns:geo="http://www.w3.org/2003/01/geo/wgs84_pos#">
	<xsl:output method="xml" version="1.0" encoding="UTF-8" indent="yes"/>

	<xsl:template match="channel">
		<xsl:element name="channel">

			<xsl:apply-templates select="item" />
		</xsl:element>
	</xsl:template>

	<xsl:template match="item">
		<xsl:element name="item">
			<xsl:copy-of select="title" />
			<xsl:copy-of select="description" />
			<xsl:apply-templates select="link" />
		</xsl:element>
	</xsl:template>

	<xsl:template match="link">
		<xsl:element name="geo:lat">
			<xsl:call-template name="GetPartOfUrl">
				<xsl:with-param name="find">lat</xsl:with-param>
			</xsl:call-template>
		</xsl:element>
		<xsl:element name="geo:long">
			<xsl:call-template name="GetPartOfUrl">
				<xsl:with-param name="find">lon</xsl:with-param>
			</xsl:call-template>
		</xsl:element>
	</xsl:template>

	<xsl:template name="GetPartOfUrl">
		<xsl:param name="find">lat</xsl:param>

		<xsl:value-of select="substring-before(substring-after(., concat($find, '=')), '&amp;')" />
	</xsl:template>
</xsl:stylesheet>
