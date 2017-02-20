<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://xsltsl.org/date-time">
  <xsl:output method="html" version="4.0" omit-xml-declaration="yes"/>

  <xsl:template match="NewDataSet">
    <table width="100%" border="0" cellpadding="1" cellspacing="2">
      <thead>
        <tr style="background-image: url('../images/header_rowBg.gif');background-repeat: repeat-x;height:24px;">
          <th align="left" valign="top">Product Name</th>
          <th align="left" valign="top">Product Code</th>
          <th align="left" valign="top">Qty</th>
          <th align="left" valign="top">Refusal Type</th>
          <th align="left" valign="top">Refused on Job</th>
          <th align="left" valign="top">Refused at</th>
          <th align="left" valign="top">Returning on Job</th>
          <th align="left" valign="top">Return to</th>
        </tr>
      </thead>
      <tbody>
        <xsl:apply-templates select="//Table" />
      </tbody>
    </table>
  </xsl:template>

  <xsl:template match="Table">
    <tr>
      <td align="left" valign="top"><xsl:value-of select="ProductName"/></td>
      <td align="left" valign="top"><xsl:value-of select="ProductCode"/></td>
      <td align="left" valign="top"><xsl:value-of select="QuantityRefused"/></td>
      <td align="left" valign="top"><xsl:value-of select="RefusalType"/></td>
      <td align="right" valign="top"><xsl:value-of select="RefusalJobId"/></td>
      <td align="left" valign="top"><xsl:value-of select="RefusedAt"/></td>
      <td align="right" valign="top"><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text><xsl:value-of select="ReturnJobId"/></td>
      <td align="left" valign="top"><xsl:value-of select="ReturnPoint"/><xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text></td>
    </tr>
  </xsl:template>

</xsl:stylesheet>