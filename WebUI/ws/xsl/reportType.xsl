<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
	<!-- Parameter to take the webserver - used to create the urls contained in the stylesheet -->
	<xsl:param name="WebServer" />
	<xsl:param name="ReportType" />
  <xsl:param name="ReportUrl" />

  <xsl:template match="/">
    <html>
      <head>
        <xsl:element name="link">
          <xsl:attribute name="href">
            <xsl:value-of select="$WebServer"/>
            <xsl:text>/style/styles.css</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="type">
            <xsl:text>text/css</xsl:text>
          </xsl:attribute>
          <xsl:attribute name="rel">
            <xsl:text>stylesheet</xsl:text>
          </xsl:attribute>
        </xsl:element>
      </head>
      <body>
        <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
          <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Your report is ready.</div>
          <p>
            Your report has been produced and can be viewed by clicking
            <xsl:element name="a"><xsl:attribute name="href"><xsl:value-of select="$ReportUrl"/></xsl:attribute>here</xsl:element>.
          </p>
          <p>
            In order to view the report you will need to have Adobe Reader installed, which you can download from <a href="http://www.adobe.com/products/acrobat/readstep2.html" title="Download Adobe Reader">here</a>.
          </p>
          <br />
          <p>
            <b>Orchestrator</b>
          </p>
        </fieldset>
      </body>
    </html>
	</xsl:template>
	
</xsl:stylesheet>