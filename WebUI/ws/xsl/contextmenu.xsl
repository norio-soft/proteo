<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes" />

	<xsl:template match="/">
		<table class="menuCanvas" id="Context Menu" cellSpacing="0" cellPadding="0" width="142" border="0">
			<tr class="menuTitleBar">
				<td align="center"><xsl:value-of select="Nodes/@MenuTitle"/></td>
			</tr>
			<tr>
				<td style="PADDING-BOTTOM: 4px">
					<table cellSpacing="0" cellPadding="1" width="100%" align="center">
						<xsl:apply-templates />
					</table>
				</td>
			</tr>
		</table>
	</xsl:template>
	
	<xsl:template match="Node">
		<tr>
			<td align="center">
				<xsl:element name="a">
					<xsl:attribute name="href"><xsl:value-of select="Url"/></xsl:attribute>
					<xsl:value-of select="Text"/>
				</xsl:element>
			</td>
		</tr>
	</xsl:template>
	
	<xsl:template match="Seperator">
		<tr style="height:3px;"><td style="height:5px;border-bottom:solid 1pt #cccccc;"><img src="/images/blank.gif" height="5"/></td></tr>
	</xsl:template>
</xsl:stylesheet>
