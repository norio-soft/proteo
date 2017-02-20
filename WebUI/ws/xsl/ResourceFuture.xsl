<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://xsltsl.org/date-time">
  <!-- Import part of the XSLT Standard Library http://xsltsl.sourceforge.net/ -->
  <xsl:import href="sl/date-time.xsl"/>
  <xsl:output method="html" version="4.0" omit-xml-declaration="yes"/>

  <xsl:param name="webserver" />
  <xsl:param name="mode" />

  <xsl:template match="rs">
    <xsl:apply-templates select="r" />
  </xsl:template>

  <xsl:template match="r">
    <table border="0" cellpadding="2" cellspacing="0" width="100">
      <tbody>
        <xsl:apply-templates select="l[@group=$mode]" />
      </tbody>
    </table>
  </xsl:template>

  <xsl:template match="l">
    <!-- Handles locations not holding an instruction -->
    <xsl:element name="tr">
      <xsl:attribute name="onMouseOver">
        ShowLegPoint("<xsl:value-of select="$webserver" />
        <xsl:text disable-output-escaping="yes">/point/getLegPointDataForJobhtml.aspx?LegPointId=</xsl:text><xsl:value-of select="@instructionId" />
        <xsl:text disable-output-escaping="yes">&amp;PointId=</xsl:text><xsl:value-of select="@pid" />
        <xsl:text disable-output-escaping="yes">&amp;InstructionId=</xsl:text>
        <xsl:text disable-output-escaping="yes">&amp;InstructionTypeId=</xsl:text>
        <xsl:text disable-output-escaping="yes">&amp;BookedTime=</xsl:text>
        <xsl:text disable-output-escaping="yes">&amp;IsAnyTime=</xsl:text>
        <xsl:text disable-output-escaping="yes">&amp;LegPlannedStart=</xsl:text>
        <xsl:call-template name="dt:format-date-time">
          <xsl:with-param name="xsd-date-time" select="@ls"/>
          <xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
        </xsl:call-template>
        <xsl:text disable-output-escaping="yes">&amp;LegPlannedEnd=</xsl:text>
        <xsl:call-template name="dt:format-date-time">
          <xsl:with-param name="xsd-date-time" select="@le"/>
          <xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
        </xsl:call-template>
        <xsl:text disable-output-escaping="yes">");</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="onMouseOut">HidePoint();</xsl:attribute>

      <xsl:element name="td">
        <xsl:attribute name="width">10</xsl:attribute>
        <xsl:attribute name="valign">top</xsl:attribute>

        <xsl:element name="li" />
      </xsl:element>

      <xsl:element name="td">
        <xsl:element name="a">
          <xsl:attribute name="href">
            javascript:openUpdateLocation(<xsl:value-of select="@instructionId" />);
          </xsl:attribute>

          <xsl:value-of select="@desc" />
        </xsl:element>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="l[@itid]">
    <!-- Handles locations holding an instruction -->
    <xsl:element name="tr">
      <xsl:attribute name="onMouseOver">
        ShowLegPoint("<xsl:value-of select="$webserver" />
        <xsl:text disable-output-escaping="yes">/point/getLegPointDataForJobhtml.aspx?LegPointId=</xsl:text><xsl:value-of select="@instructionId" />
        <xsl:text disable-output-escaping="yes">&amp;PointId=</xsl:text><xsl:value-of select="@pid" />
        <xsl:text disable-output-escaping="yes">&amp;InstructionId=</xsl:text><xsl:value-of select="@iid" />
        <xsl:text disable-output-escaping="yes">&amp;InstructionTypeId=</xsl:text><xsl:value-of select="@itid" />
        <xsl:text disable-output-escaping="yes">&amp;BookedTime=</xsl:text>
        <xsl:call-template name="dt:format-date-time">
          <xsl:with-param name="xsd-date-time" select="@b"/>
          <xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
        </xsl:call-template>
        <xsl:text disable-output-escaping="yes">&amp;IsAnyTime=</xsl:text><xsl:value-of select="@a" />
        <xsl:text disable-output-escaping="yes">&amp;LegPlannedStart=</xsl:text>
        <xsl:call-template name="dt:format-date-time">
          <xsl:with-param name="xsd-date-time" select="@ls"/>
          <xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
        </xsl:call-template>
        <xsl:text disable-output-escaping="yes">&amp;LegPlannedEnd=</xsl:text>
        <xsl:call-template name="dt:format-date-time">
          <xsl:with-param name="xsd-date-time" select="@le"/>
          <xsl:with-param name="format" select="'%d/%m/%Y %H:%M'"/>
        </xsl:call-template>
        <xsl:text disable-output-escaping="yes">");</xsl:text>
      </xsl:attribute>
      <xsl:attribute name="onMouseOut">HidePoint();</xsl:attribute>

      <xsl:element name="td">
        <xsl:attribute name="width">10</xsl:attribute>
        <xsl:attribute name="valign">top</xsl:attribute>

        <xsl:element name="li" />
      </xsl:element>

      <xsl:element name="td">
        <xsl:element name="a">
          <xsl:attribute name="href">
            javascript:OpenJobDetails(<xsl:value-of select="@jid" />);
          </xsl:attribute>

          <xsl:value-of select="@desc" />
        </xsl:element>
      </xsl:element>
    </xsl:element>
  </xsl:template>

  <xsl:template match="l[@rsd]">
    <!-- Handles resource schedule items -->
    <xsl:element name="tr">
      <xsl:element name="td">
        <xsl:attribute name="width">10</xsl:attribute>
        <xsl:attribute name="valign">top</xsl:attribute>

        <xsl:element name="li" />
      </xsl:element>

      <xsl:element name="td">
        <xsl:value-of select="@rsd" />
      </xsl:element>
    </xsl:element>
  </xsl:template>

</xsl:stylesheet>