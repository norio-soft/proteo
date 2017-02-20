<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://xsltsl.org/date-time">
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes"/>

	<xsl:include href="instructions.xsl" />

	<!-- Leg Point Id -->
	<xsl:param name="LegPointId"></xsl:param>
	
	<xsl:template match="Job">
		<table width="100%" border="0" cellpadding="1" cellspacing="2">
			<tbody>
				<tr>
					<td>
						<xsl:apply-templates select="//LegPoint[LegPointId = $LegPointId]/Point" />
					</td>
				</tr>
			</tbody>
		</table>
	</xsl:template>
	
</xsl:stylesheet>