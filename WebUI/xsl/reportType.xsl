<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:fo="http://www.w3.org/1999/XSL/Format">
	<!-- Parameter to take the webserver - used to create the urls contained in the stylesheet -->
	<xsl:param name="WebServer" />
	<xsl:param name="ReportType" />
  <xsl:param name="ReportUrl" />
  <xsl:param name="CompanyName" />
  <xsl:param name="AddressLine1" />
  <xsl:param name="AddressLine2" />
  <xsl:param name="AddressLine3" />
  <xsl:param name="CompanyLogoImageName" />

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
          <p>
            To Whom It May Concern
            </p>
          <p>
            We have created a report for your attention, the report can be viewed by clicking on the following link: 
            <xsl:element name="a"><xsl:attribute name="href"><xsl:value-of select="$ReportUrl"/></xsl:attribute><xsl:value-of select="$ReportType"/></xsl:element>.
          </p>
          <p>
            In order to view this report you will need to have Adobe Reader installed, which you can download from <a href="https://get.adobe.com/reader/" title="Download Adobe Reader">here</a>.
          </p>
          <p>
            Kind Regards,
          </p>
        <p>
          <xsl:value-of select="$CompanyName"/><br />
          <xsl:value-of select="$AddressLine1"/><br />
          <xsl:value-of select="$AddressLine2"/><br />
          <xsl:value-of select="$AddressLine3"/><br />
          <br />
          <xsl:if test="not($CompanyLogoImageName = 'cid:' )" >
            <xsl:element name="img"><xsl:attribute name="src"><xsl:value-of select="$CompanyLogoImageName"/></xsl:attribute></xsl:element>
          </xsl:if>
        </p>
        </fieldset>
      </body>
    </html>
	</xsl:template>
	
</xsl:stylesheet>