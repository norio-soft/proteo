<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dt="http://xsltsl.org/date-time">
  <!-- Import part of the XSLT Standard Library http://xsltsl.sourceforge.net/ -->
  <xsl:import href="sl/date-time.xsl"/>
  <xsl:output method="html" version="4.0" omit-xml-declaration="yes"/>

  <!-- Load Instructions -->
  <xsl:param name="InstructionTypeId">1</xsl:param>
  <!-- The text used to identify the docket number -->
  <xsl:param name="DocketText">Docket Number</xsl:param>
  <!-- Web Server -->
  <xsl:param name="webserver"></xsl:param>
  <xsl:param name="showShortAddress"></xsl:param>
  <xsl:param name="showDockets">true</xsl:param>

  <xsl:template match="Job">
    <table width="98%" border="0" cellpadding="1" cellspacing="2">
      <tbody>
        <xsl:for-each select="//Instruction[contains($InstructionTypeId, InstructionTypeId) and (InstructionTypeId != '7' or CollectDrops/CollectDrop/Order)]">
          <tr>
            <xsl:call-template name="InstructionTemp">
            </xsl:call-template>
          </tr>
          <tr>
            <xsl:choose>
              <xsl:when test="$showDockets = 'true'">
                <td colspan="99">
                  <hr/>
                </td>
              </xsl:when>
              <xsl:otherwise>
                <td>
                  <hr/>
                </td>
              </xsl:otherwise>
            </xsl:choose>
          </tr>
        </xsl:for-each>
      </tbody>
    </table>
  </xsl:template>

  <xsl:template name="InstructionTemp">
    <xsl:param name="InstructionTypeId" />

    <td width="30%" valign="top">

      <p style="background-color: #363636; padding: 3px; color: white; margin-left: 0px; margin-top: 0px; margin-bottom: 10px;">
		  
        <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>

        <xsl:choose>
          <xsl:when test="InstructionTypeId = '1'">
            <xsl:text>Load</xsl:text>
          </xsl:when>
          <xsl:when test="InstructionTypeId = '2'">
            <xsl:text>Drop</xsl:text>
          </xsl:when>
          <xsl:when test="InstructionTypeId = '3'">
            <xsl:text>Leave Pallets</xsl:text>
          </xsl:when>
          <xsl:when test="InstructionTypeId = '4'">
            <xsl:text>Dehire Pallets</xsl:text>
          </xsl:when>
          <xsl:when test="InstructionTypeId = '5'">
            <xsl:text>Pickup Pallets</xsl:text>
          </xsl:when>
          <xsl:when test="InstructionTypeId = '6'">
            <xsl:text>Leave Goods</xsl:text>
          </xsl:when>
          <xsl:when test="InstructionTypeId = '7'">
            <xsl:text>Trunk</xsl:text>
          </xsl:when>
        </xsl:choose>
        <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
      </p>
      <xsl:apply-templates select="Point" />
      <br />
      <table cellpadding="0" cellspacing="0">
        <tr>
          <td class="formCellLabel">
            <xsl:text disable-output-escaping="yes">Booked&amp;nbsp;</xsl:text>
          </td>
          <td>
              <xsl:call-template name="dt:format-date-time">
                <xsl:with-param name="xsd-date-time" select="BookedDateTime"/>
                <xsl:with-param name="format" select="'%d/%m/%y'"/>
              </xsl:call-template>
              <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
              <xsl:choose>
                <xsl:when test="IsAnyTime = 'false'">
                  <xsl:call-template name="dt:format-date-time">
                    <xsl:with-param name="xsd-date-time" select="BookedDateTime"/>
                    <xsl:with-param name="format" select="'%H:%M'"/>
                  </xsl:call-template>
                </xsl:when>
                <xsl:otherwise>
                  <xsl:text>AnyTime</xsl:text>
                </xsl:otherwise>
              </xsl:choose>
          </td>
        </tr>

        <xsl:choose>
          <xsl:when test="InstructionActuals">
            <xsl:call-template name="InstructionActuals" />
          </xsl:when>
          <xsl:otherwise>
          </xsl:otherwise>
        </xsl:choose>

        <tr>
          <td colspan="2">
            <xsl:if test="InstructionTypeId = 1 and (/Job/JobState != 'Invoiced')">
              <xsl:call-template name="DisplayUpdateDocketButton" />
            </xsl:if>
            <xsl:if test="InstructionTypeId = 1">
              <xsl:if test="/Job/JobType='Groupage'">
                <xsl:call-template name="DisplayLoadOrderButton"/>
              </xsl:if>
              <xsl:call-template name="DisplayLoadReleaseButton" />
            </xsl:if>
          </td>
        </tr>

      </table>

    </td>
    <xsl:if test="$showDockets = 'true'">
      <td width="70%" valign="top">
        <xsl:apply-templates select="CollectDrops" />
      </td>
    </xsl:if>
  </xsl:template>

  <xsl:template name="DisplayLoadReleaseButton">
    <xsl:element name="input">
      <xsl:attribute name="type">button</xsl:attribute>
      <xsl:attribute name="style">width:130px</xsl:attribute>
      <xsl:attribute name="class">buttonClass</xsl:attribute>
      <xsl:attribute name="value">Load Release Form</xsl:attribute>
      <xsl:attribute name="onclick">
        <xsl:text>javascript:window.open('</xsl:text>
        <xsl:value-of select="$webserver" />
        <xsl:text>/Traffic/JobManagement/LoadReleaseNotification.aspx?jobId=</xsl:text>
        <xsl:value-of select="/Job/JobId" />
        <xsl:text disable-output-escaping="yes">&amp;amp;instructionId=</xsl:text>
        <xsl:value-of select="./InstructionID"	/>
        <xsl:text>');</xsl:text>
      </xsl:attribute>
    </xsl:element>
  </xsl:template>

  <xsl:template name="DisplayUpdateDocketButton">
    <xsl:element name="input">
      <xsl:attribute name="type">button</xsl:attribute>
      <xsl:attribute name="style">width:83px</xsl:attribute>
      <xsl:attribute name="class">buttonClass</xsl:attribute>
      <xsl:attribute name="value">Dockets</xsl:attribute>
      <xsl:attribute name="onclick">
        <xsl:text>javascript:showDocketWindow(</xsl:text>
        <xsl:value-of select="./InstructionID"	/>
        <xsl:text>);</xsl:text>
      </xsl:attribute>
    </xsl:element>
  </xsl:template>

  <xsl:template name="DisplayDemurrageButton">
    <xsl:variable name="instructionid" select="InstructionID"></xsl:variable>
    <xsl:variable name="extraId" select ="/Job/Extras/Extra[InstructionId = $instructionid and ExtraType='Demurrage']/ExtraId"></xsl:variable>
    <xsl:element name="input">
      <xsl:attribute name="style">width:83px</xsl:attribute>
      <xsl:attribute name="class">buttonClass</xsl:attribute>
      <xsl:attribute name="type">button</xsl:attribute>
      <xsl:choose>
        <xsl:when test="/Job/Extras/Extra[InstructionId = $instructionid and ExtraType='Demurrage']/ExtraId">

          <xsl:attribute name="value">Demurrage *</xsl:attribute>
        </xsl:when>
        <xsl:otherwise>
          <xsl:attribute name="value">Demurrage</xsl:attribute>
        </xsl:otherwise>
      </xsl:choose>
      <xsl:attribute name="onclick">
        <xsl:text>javascript:showDemurrageWindow(</xsl:text>
        <xsl:value-of select="//JobId" />
        <xsl:text>,</xsl:text>
        <xsl:value-of select="InstructionID"	/>
        <xsl:text>,</xsl:text>
        <xsl:choose>
          <xsl:when test="$extraId">
            <xsl:value-of select="$extraId"	/>
          </xsl:when>
          <xsl:otherwise>0</xsl:otherwise>
        </xsl:choose>

        <xsl:text>);</xsl:text>
      </xsl:attribute>

    </xsl:element>

  </xsl:template>

  <xsl:template name="DisplayLoadOrderButton">

    <xsl:element name="input">

      <xsl:attribute name="type">button</xsl:attribute>
      <xsl:attribute name="style">width:83px</xsl:attribute>
      <xsl:attribute name="class">buttonClass</xsl:attribute>
      <xsl:attribute name="value">Load Order</xsl:attribute>
      <xsl:attribute name="onclick">
        <xsl:text>javascript:ShowLoadOrder(</xsl:text>
        <xsl:value-of select="./InstructionID" />
        <xsl:text>);</xsl:text>
      </xsl:attribute>
    </xsl:element>

  </xsl:template>

  <xsl:template name="InstructionActuals">

    <tr>
      <td class="formCellLabel">
        <xsl:text disable-output-escaping="yes">Arrival&amp;nbsp;</xsl:text>
      </td>
      <td class="formCellField">
          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/ArrivalDateTime"/>
            <xsl:with-param name="format" select="'%d/%m/%y'"/>
          </xsl:call-template>

          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>

          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/ArrivalDateTime"/>
            <xsl:with-param name="format" select="'%H:%M'"/>
          </xsl:call-template>
      </td>
    </tr>
    <tr>
      <td class="formCellLabel">
        <xsl:text disable-output-escaping="yes">Departure&amp;nbsp;</xsl:text>
      </td>
      <td class="formCellField">
          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/LeaveDateTime"/>
            <xsl:with-param name="format" select="'%d/%m/%y'"/>
          </xsl:call-template>
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/LeaveDateTime"/>
            <xsl:with-param name="format" select="'%H:%M'"/>
          </xsl:call-template>
      </td>
    </tr>
    <tr>
      <td class="formCellLabel">
        <xsl:text disable-output-escaping="yes">Called In&amp;nbsp;</xsl:text>
      </td>
      <td class="formCellField">
          <xsl:value-of select="InstructionActuals/InstructionActual/CreateUser" />
      </td>
    </tr>
    <tr>
      <td></td>
      <td>
          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/CreateDate"/>
            <xsl:with-param name="format" select="'%d/%m/%y'"/>
          </xsl:call-template>
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/CreateDate"/>
            <xsl:with-param name="format" select="'%H:%M'"/>
          </xsl:call-template>
      </td>
    </tr>

    <xsl:if test="InstructionActual/LastUpdateUser != ''">
      <tr>
        <td>
          <xsl:text disable-output-escaping="yes">Updated:&amp;nbsp;</xsl:text>
        </td>
        <td>
            <xsl:value-of select="InstructionActuals/InstructionActual/LastUpdateUser"/>

            <xsl:call-template name="dt:format-date-time">
              <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/LastUpdateDate"/>
              <xsl:with-param name="format" select="'%d/%m/%y'"/>
            </xsl:call-template>
            <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
            <xsl:call-template name="dt:format-date-time">
              <xsl:with-param name="xsd-date-time" select="InstructionActuals/InstructionActual/LastUpdateDate"/>
              <xsl:with-param name="format" select="'%H:%M'"/>
            </xsl:call-template>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="$showDockets = 'true'">
      <tr style="padding-top:5px;">
        <td colspan="2">
          <xsl:element name="input">
            <xsl:attribute name="type">button</xsl:attribute>
            <xsl:attribute name="style">width:83px</xsl:attribute>
            <xsl:attribute name="class">buttonClass</xsl:attribute>
            <xsl:attribute name="value">View Call-in</xsl:attribute>
            <xsl:attribute name="onclick">
              <xsl:text>javascript:window.location='</xsl:text>
              <xsl:value-of select="$webserver" />
              <xsl:text>/Traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&amp;jobId=</xsl:text>
              <xsl:value-of select="//JobId" />
              <xsl:text disable-output-escaping="yes">&amp;instructionId=</xsl:text>
              <xsl:value-of select="InstructionID"	/>
              <xsl:text>'</xsl:text>
            </xsl:attribute>
          </xsl:element>
          <xsl:call-template name="DisplayDemurrageButton"></xsl:call-template>
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

  <xsl:template match="Point">
    <xsl:choose>
      <xsl:when test="$showShortAddress = 'true'">
        <span id="spnCollectionPoint" onClick="" class="orchestratorLink">
          <xsl:attribute name="onMouseOver">
            ShowPointToolTip(this,<xsl:value-of select="PointId"/>)
          </xsl:attribute>
          <xsl:attribute name="onMouseOut">closeToolTip();</xsl:attribute>
          <b>
            <xsl:value-of select="OrganisationName" />
          </b>
        </span>
        <br/>
      </xsl:when>
      <xsl:otherwise>
        <b>
          <xsl:value-of select="OrganisationName" />
        </b>
        <br></br>
        <xsl:if test="Address/AddressLine1 != ''">
          <xsl:value-of select="Address/AddressLine1"	/>
          <br></br>
        </xsl:if>
        <xsl:if test="Address/AddressLine2 != ''">
          <xsl:value-of select="Address/AddressLine2"	/>
          <br></br>
        </xsl:if>
        <xsl:if test="Address/AddressLine3 != ''">
          <xsl:value-of select="Address/AddressLine3"	/>
          <br></br>
        </xsl:if>
        <xsl:if test="Address/PostTown != ''">
          <xsl:value-of select="Address/PostTown"	/>
          <br></br>
        </xsl:if>
        <xsl:if test="Address/County != ''">
          <xsl:value-of select="Address/County"	/>
          <br></br>
        </xsl:if>
        <xsl:if test="Address/PostCode != ''">
          <xsl:value-of select="Address/PostCode"	/>
          <br></br>
        </xsl:if>
      </xsl:otherwise>
    </xsl:choose>

  </xsl:template>

  <xsl:template match="CollectDrops">
    <table border="0" cellpadding="0" cellspacing="0" style="width:100%;" >
      <xsl:choose>
        <xsl:when test="//JobType = 'Normal'">
          <tr class="DataGridListHeadSmall">
            <td valign="top" style="border-bottom: 1px solid #000; border-left 1px solid #000;" nowrap="true">
              <xsl:value-of select="$DocketText" />
            </td>
            <td valign="top" style="border-bottom: 1px solid #000;" nowrap="true">Reference</td>
            <td valign="top" style="border-bottom: 1px solid #000;" nowrap="true">Pallets</td>
            <td valign="top" style="border-bottom: 1px solid #000;" nowrap="true">Weight</td>
            <td valign="top" style="border-bottom: 1px solid #000;" nowrap="true">Cases</td>
            <td valign="top" style="border-bottom: 1px solid #000;" nowrap="true">Goods Type</td>
          </tr>
          <tbody bgcolor="#FFFFFF">
            <xsl:apply-templates select="CollectDrop" mode="Normal" />
            <tr>
              <td colspan="6">
                <hr noshade="noshade"	/>
              </td>
            </tr>
            <xsl:if test="../../Note != ''">
              <tr>
                <td colspan="5">
                  <xsl:value-of select="../../Note" />
                </td>
              </tr>
            </xsl:if>
          </tbody>
        </xsl:when>
        <xsl:when test="//JobType = 'Groupage'">
          <tr class="DataGridListHeadSmall">
            <td valign="top" style="border-bottom: 1px solid #000; width:80px;"  nowrap="true">Action</td>
            <td valign="top" style="border-bottom: 1px solid #000; width:60px;"  nowrap="true">Order ID</td>
            <td valign="top" style="border-bottom: 1px solid #000; width:80px;"  nowrap="true">Status</td>
            <td valign="top" style="border-bottom: 1px solid #000; width:190px;" nowrap="true">Customer Order No</td>
            <td valign="top" style="border-bottom: 1px solid #000; width:80px;" nowrap="true">Weight</td>
            <td valign="top" style="border-bottom: 1px solid #000; width:80px;" nowrap="true">Pallets</td>
            <td valign="top" style="border-bottom: 1px solid #000; width:80px;" nowrap="true">Goods Type</td>
          </tr>
          <tbody bgcolor="#FFFFFF">
            <xsl:apply-templates select="CollectDrop" mode="Groupage" />
            <tr>
              <td colspan="7">
                <hr noshade="noshade"	/>
              </td>
            </tr>
            <xsl:if test="../../Note != ''">
              <tr>
                <td colspan="5">
                  <xsl:value-of select="../../Note" />
                </td>
              </tr>
            </xsl:if>
          </tbody>
        </xsl:when>
        <xsl:when test="//JobType = 'Return'">
          <thead>
            <tr class="DataGridListHeadSmall">
              <td valign="top" style="border-bottom: 1px solid #000;">Product Name</td>
              <td valign="top" style="border-bottom: 1px solid #000;">Quantity Refused</td>
              <td valign="top" style="border-bottom: 1px solid #000;">Time Frame</td>
            </tr>
          </thead>
          <tbody bgcolor="#FFFFFF">
            <xsl:apply-templates select="CollectDrop" mode="GoodsReturn" />
            <tr>
              <td colspan="5">
                <hr noshade="noshade"	/>
              </td>
            </tr>
            <xsl:if test="../../Note != ''">
              <tr>
                <td colspan="5">
                  <xsl:value-of select="../../Note" />
                </td>
              </tr>
            </xsl:if>
          </tbody>
        </xsl:when>
        <xsl:when test="//JobType = 'PalletReturn'">
          <thead>
            <tr class="DataGridListHeadSmall">
              <th valign="top" style="border-bottom: 1px solid #000;">Pallet Owner</th>
              <th valign="top" style="border-bottom: 1px solid #000;">Number of Pallets</th>
            </tr>
          </thead>
          <tbody bgcolor="#FFFFFF">
            <xsl:apply-templates select="CollectDrop" mode="PalletReturn" />
            <tr>
              <td colspan="5">
                <hr noshade="noshade"	/>
              </td>
            </tr>
            <xsl:if test="../../Note != ''">
              <tr>
                <td colspan="5">
                  <xsl:value-of select="../../Note" />
                </td>
              </tr>
            </xsl:if>
          </tbody>
        </xsl:when>
      </xsl:choose>
    </table>
    <xsl:if test="../InstructionActuals/InstructionActual/Discrepancies != ''">
      <p>
        <b>Discrepancies:</b>&#160;
        <xsl:value-of select="../InstructionActuals/InstructionActual/Discrepancies"/>
      </p>
    </xsl:if>
  </xsl:template>

  <xsl:template match="CollectDrop" mode="Normal">
    <xsl:variable name="CollectDropId" select="CollectDropId"/>
    <tr>
      <td valign="top">
        <xsl:value-of select="Docket" />
      </td>
      <td valign="top">
        <xsl:value-of select="ClientsCustomerReference" />
      </td>
      <td  valign="top">
        <xsl:value-of select="NoPallets" />
        <xsl:if test="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]">
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <b>
            <xsl:text>(</xsl:text>
            <xsl:value-of select="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]/NumberOfPallets" />
            <xsl:text>)</xsl:text>
          </b>
        </xsl:if>
      </td>
      <td  valign="top">
        <xsl:value-of select="format-number(Weight, '0')" />
        <xsl:if test="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]">
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <b>
            <xsl:text>(</xsl:text>
            <xsl:value-of select="format-number(../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]/Weight, '0')" />
            <xsl:text>)</xsl:text>
          </b>
        </xsl:if>
      </td>
      <td  valign="top">
        <xsl:value-of select="NoCases" />
        <xsl:if test="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]">
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <b>
            <xsl:text>(</xsl:text>
            <xsl:value-of select="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]/NumberOfCases" />
            <xsl:text>)</xsl:text>
          </b>
        </xsl:if>
      </td>
      <td  valign="top">
        <xsl:value-of select="GoodsTypeDescription" />
      </td>
    </tr>
    <xsl:if test="../../Note">
      <tr>
        <td colspan="5" valign="top">
          <xsl:value-of select="../../Note" />
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

  <xsl:template match="CollectDrop" mode="PalletReturn">
    <xsl:variable name="CollectDropId" select="CollectDropId"/>
    <xsl:variable name="Docket" select="Docket"/>
    <tr>
      <td valign="top">
        <xsl:value-of select="//Instruction[InstructionTypeId = 2]/ClientsCustomer[../CollectDrops/CollectDrop[Docket = $Docket]]" />
      </td>
      <td valign="top">
        <xsl:value-of select="NoPallets" />
        <xsl:if test="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]">
          <xsl:text> </xsl:text>
          <b>
            <xsl:text>(</xsl:text>
            <xsl:value-of select="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]/NumberOfPallets" />
            <xsl:text>)</xsl:text>
          </b>
        </xsl:if>
      </td>
    </tr>
    <xsl:if test="../../Note">
      <tr>
        <td colspan="3" valign="top">
          <xsl:value-of select="../../Note" />
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

  <xsl:template match="CollectDrop" mode="GoodsReturn">
    <xsl:if test="./GoodsRefusal">
      <tr>
        <td valign="top">
          <xsl:element name="a">
            <xsl:attribute name="href">
              <xsl:text disable-output-escaping="yes">javascript:openDialogWithScrollbars('~/traffic/jobmanagement/DriverCallIn/CallIn.aspx?wiz=true&amp;</xsl:text>jobId=<xsl:value-of select="GoodsRefusal/JobId" /><xsl:text disable-output-escaping="yes">&amp;instructionId=</xsl:text><xsl:value-of select="GoodsRefusal/InstructionId" />','600','400');
            </xsl:attribute>
            <xsl:value-of select="GoodsRefusal/ProductName" />
          </xsl:element>
        </td>
        <td valign="top">
          <xsl:value-of select="GoodsRefusal/QuantityRefused" />
        </td>
        <td valign="top">
          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="GoodsRefusal/TimeFrame"/>
            <xsl:with-param name="format" select="'%d/%m/%y'"/>
          </xsl:call-template>
          <xsl:text disable-output-escaping="yes">&amp;nbsp;</xsl:text>
          <xsl:call-template name="dt:format-date-time">
            <xsl:with-param name="xsd-date-time" select="GoodsRefusal/TimeFrame"/>
            <xsl:with-param name="format" select="'%H:%M'"/>
          </xsl:call-template>
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="../../Note">
      <tr>
        <td colspan="3" valign="top">
          <xsl:value-of select="../../Note" />
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

  <xsl:template match="CollectDrop" mode="Groupage">
    <xsl:variable name="CollectDropId" select="CollectDropId"/>

    <xsl:if test="./Order">
      <tr>
        <td valign="top">
          <xsl:choose>
            <xsl:when test="../../../InstructionTypeId = '1'">Load</xsl:when>
            <xsl:when test="../../../InstructionTypeId = '2' and OrderAction = 'Default'">Deliver</xsl:when>
            <xsl:otherwise>
              <xsl:value-of select="translate(OrderAction, '_', ' ')" />
            </xsl:otherwise>
          </xsl:choose>
        </td>
        <td valign="top">
          <table cellpadding="1" cellspacing="0" border="0">
            <tr>
              <td>
                <xsl:element name="a">
                  <xsl:attribute name="href">
                    <xsl:text disable-output-escaping="yes">javascript:viewOrder(</xsl:text>
                    <xsl:value-of select="Order/OrderID" />
                    <xsl:text disable-output-escaping="yes">);</xsl:text>
                  </xsl:attribute>
                  <xsl:value-of select="Order/OrderID" />
                </xsl:element>
                <xsl:text> </xsl:text>
              </td>
            </tr>
          </table>
        </td>
        <td valign="top">
          <xsl:value-of select="Order/OrderStatus" />
        </td>
        <td valign="top">
          <xsl:element name="a">
            <xsl:attribute name="href">
              <xsl:text disable-output-escaping="yes">javascript:viewOrder(</xsl:text>
              <xsl:value-of select="Order/OrderID" />
              <xsl:text disable-output-escaping="yes">);</xsl:text>
            </xsl:attribute>

            <xsl:value-of select="Order/CustomerOrderNumber" />
          </xsl:element>
        </td>
        <td valign="top" >
          <xsl:value-of select="format-number(Weight, '0')" />
          <xsl:if test="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]">
            <xsl:text> </xsl:text>
            <b>
              <xsl:text>(</xsl:text>
              <xsl:value-of select="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]/Weight" />
              <xsl:text>)</xsl:text>
            </b>
          </xsl:if>
        </td>
        <td valign="top" >
          <xsl:value-of select="NoPallets" />
          <xsl:if test="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]">
            <xsl:text> </xsl:text>
            <b>
              <xsl:text>(</xsl:text>
              <xsl:value-of select="../../InstructionActuals/InstructionActual/CollectDropActuals/CollectDropActual[CollectDropId = $CollectDropId]/NumberOfPallets" />
              <xsl:text>)</xsl:text>
            </b>
          </xsl:if>
        </td>
        <td valign="top">
          <xsl:value-of select="GoodsTypeDescription" />
        </td>
      </tr>
    </xsl:if>
    <xsl:if test="../../Note">
      <tr>
        <td colspan="3" valign="top">
          <xsl:value-of select="../../Note" />
        </td>
      </tr>
    </xsl:if>
  </xsl:template>

</xsl:stylesheet>
