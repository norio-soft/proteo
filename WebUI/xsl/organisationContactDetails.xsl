<?xml version="1.0" encoding="utf-8"?>

<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">

<xsl:template match="Organisation">
  <table cellpadding="2" cellspacing="0" border="0" width="100%">
    <tr>
      <td>
        <b>
          Telephone Number
        </b>
      </td>
      <td>
        <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
      </td>
      <td>
        <xsl:value-of select="Locations/OrganisationLocation[OrganisationLocationType='HeadOffice']/TelephoneNumber"/>
      </td>
    </tr>
    <tr>
      <td>
        <b>
          Fax Number
        </b>
      </td>
      <td>
        <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
      </td>
      <td>
        <xsl:value-of select="Locations/OrganisationLocation[OrganisationLocationType='HeadOffice']/FaxNumber"/>
      </td>
    </tr>
    <tr>
      <td>
        <b>
          Primary Email
        </b>
      </td>
      <td>
        <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
      </td>
      <td>
        <xsl:value-of select="PrimaryContact/Contacts/Contact[ContactType='Email']/ContactDetail"/>
      </td>
    </tr>
    <tr>
      <td colspan="3">
        <br />
        <xsl:value-of select="OrganisationName"/>
        <br />
        <xsl:apply-templates select="Locations/OrganisationLocation[OrganisationLocationType='HeadOffice']/Point/Address"/>
      </td>
    </tr>
  </table>
</xsl:template>

  <xsl:template match="Address">
    <xsl:if test="not(AddressLine1 = '')">
      <xsl:value-of select="AddressLine1"/>
      <br />
    </xsl:if>
    <xsl:if test="not(AddressLine2 = '')">
      <xsl:value-of select="AddressLine2"/>
      <br />
    </xsl:if>
    <xsl:if test="not(AddressLine3 = '')">
      <xsl:value-of select="AddressLine3"/>
      <br />
    </xsl:if>
    <xsl:if test="not(PostTown = '')">
      <xsl:value-of select="PostTown"/>
      <br />
    </xsl:if>
    <xsl:if test="not(County = '')">
      <xsl:value-of select="County"/>
      <br />
    </xsl:if>
    <xsl:if test="not(PostCode = '')">
      <xsl:value-of select="PostCode"/>
      <br />
    </xsl:if>
  </xsl:template>

</xsl:stylesheet> 

