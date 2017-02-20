<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://xsltsl.org/date-time">
	<!-- Import part of the XSLT Standard Library http://xsltsl.sourceforge.net/ -->
	<xsl:import href="sl/date-time.xsl"/>
	<xsl:output method="html" version="4.0" omit-xml-declaration="yes"/>

	<xsl:include href="instructions.xsl" />

	<!-- Instruction Id -->
	<xsl:param name="InstructionId"></xsl:param>
	
	<xsl:template match="Job">
		<table width="100%" border="0" cellpadding="1" cellspacing="2">
			<tbody>
				<xsl:for-each select="//Instruction[InstructionId = $InstructionId]">
					<tr>
						<xsl:apply-templates select="CollectDropSummary" />
					</tr>
					<tr>
						<td colspan="2"><hr noshade="noshade" /></td>
					</tr>
				</xsl:for-each>
			</tbody>
		</table>
	</xsl:template>
	
</xsl:stylesheet>