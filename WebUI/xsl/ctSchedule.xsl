<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rs="http://www.tempuri.org/ResourceScheduleData.xsd">
	<xsl:output method="xml" version="4.0" omit-xml-declaration="yes"/>
	
	<xsl:param name="ScheduleStartDate"/>
	<xsl:param name="ScheduleStartTime"/>
	<xsl:param name="ScheduleEndDate"/>
	<xsl:param name="ScheduleEndTime"/>
	<xsl:param name="TrafficArea"/>
	<xsl:param name="DisplayMode"/>
	<xsl:param name="ShowComingIn" />
	<xsl:param name="ShowStayingIn" />
	<xsl:param name="ShowGoingOut" />
	<xsl:param name="ShowNotAvailable" />
	
	<xsl:include href="ctCommon.xsl" />
	
	<xsl:template match="rs:ResourceScheduleData">
		<xsl:element name="ctScheduleXML">
			<xsl:call-template name="SetUp">
				<xsl:with-param name="ScheduleStartDate"><xsl:value-of select="$ScheduleStartDate" /></xsl:with-param>
				<xsl:with-param name="ScheduleEndDate"><xsl:value-of select="$ScheduleEndDate" /></xsl:with-param>
				<xsl:with-param name="ScheduleStartTime"><xsl:value-of select="$ScheduleStartTime" /></xsl:with-param>
				<xsl:with-param name="ScheduleEndTime"><xsl:value-of select="$ScheduleEndTime" /></xsl:with-param>
			</xsl:call-template>
			<xsl:call-template name="Columns" />
			
				<!-- DEBUG COUNTS
				<xsl:element name="TypeCounts">
					<xsl:attribute name="TrafficArea"><xsl:value-of select="$TrafficArea"/></xsl:attribute>
					
					<xsl:element name="ComingIn">
						<xsl:value-of select="count(rs:Resource[rs:PreviousPointTrafficAreaId != $TrafficArea and (rs:DuringPointTrafficAreaId = $TrafficArea or rs:AfterPointTrafficAreaId = $TrafficArea)])" />
					</xsl:element>
					<xsl:element name="StayingIn">
						<xsl:value-of select="count(rs:Resource[rs:PreviousPointTrafficAreaId = $TrafficArea and (rs:DuringPointTrafficAreaId = $TrafficArea or rs:AfterPointTrafficAreaId != $TrafficArea)])" />
					</xsl:element>
					<xsl:element name="GoingOut">
						<xsl:value-of select="count(rs:Resource[rs:PreviousPointTrafficAreaId = $TrafficArea and (rs:DuringPointTrafficAreaId != $TrafficArea or rs:AfterPointTrafficAreaId != $TrafficArea)])" />
					</xsl:element>
					<xsl:element name="NotAvailable">
						<xsl:value-of select="count(rs:Resource[rs:PreviousPointTrafficAreaId != $TrafficArea and rs:DuringPointTrafficAreaId != $TrafficArea and rs:AfterPointTrafficAreaId != $TrafficArea])" />
					</xsl:element>
				</xsl:element>
				-->
			<xsl:choose>
				<xsl:when test="$DisplayMode = 'ByLocation'">
				
					<!-- Coming In -->
					<xsl:if test="$ShowComingIn = 'Y'">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Coming In</xsl:with-param>
						</xsl:call-template>
						
						<xsl:for-each select="rs:Resource[rs:PreviousPointTrafficAreaId != $TrafficArea and (rs:DuringPointTrafficAreaId = $TrafficArea or rs:AfterPointTrafficAreaId = $TrafficArea)]">
							<xsl:apply-templates select="."/>
						</xsl:for-each>
					</xsl:if>
					
					<!-- Staying In -->
					<xsl:if test="$ShowStayingIn = 'Y'">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Staying In</xsl:with-param>
						</xsl:call-template>

						<xsl:for-each select="rs:Resource[rs:PreviousPointTrafficAreaId = $TrafficArea and (rs:DuringPointTrafficAreaId = $TrafficArea or rs:AfterPointTrafficAreaID = $TrafficArea)]">
							<xsl:apply-templates select="."/>
						</xsl:for-each>
					</xsl:if>
					
					<!-- Going Out -->
					<xsl:if test="$ShowGoingOut = 'Y'">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Going Out</xsl:with-param>
						</xsl:call-template>

						<xsl:for-each select="rs:Resource[rs:PreviousPointTrafficAreaId = $TrafficArea and (rs:DuringPointTrafficAreaId != $TrafficArea or rs:AfterPointTrafficAreaId != $TrafficArea)]">
							<xsl:apply-templates select="."/>
						</xsl:for-each>
					</xsl:if>
					
					<!-- Not Available -->
					<xsl:if test="$ShowNotAvailable = 'Y'">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Not Available</xsl:with-param>
						</xsl:call-template>

						<xsl:for-each select="rs:Resource[rs:PreviousPointTrafficAreaId != $TrafficArea and rs:DuringPointTrafficAreaId != $TrafficArea and rs:AfterPointTrafficAreaId != $TrafficArea]">
							<xsl:apply-templates select="."/>
						</xsl:for-each>
					</xsl:if>
				</xsl:when>

				<xsl:when test="$DisplayMode = 'ByResourceType'">
					<!-- Drivers -->
					<xsl:call-template name="OutputHeader">
						<xsl:with-param name="Text">Drivers</xsl:with-param>
					</xsl:call-template>
					<xsl:for-each select="rs:Resource[rs:ResourceTypeId = 3]">
						<xsl:sort select="rs:FullName" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
					
					<!-- Vehicles -->
					<xsl:call-template name="OutputHeader">
						<xsl:with-param name="Text">Vehicles</xsl:with-param>
					</xsl:call-template>
					<xsl:for-each select="rs:Resource[rs:ResourceTypeId = 1]">
						<xsl:sort select="rs:RegNo" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
					
					<!-- Trailers -->
					<xsl:call-template name="OutputHeader">
						<xsl:with-param name="Text">Trailers</xsl:with-param>
					</xsl:call-template>
					<xsl:for-each select="rs:Resource[rs:ResourceTypeId = 2]">
						<xsl:sort select="rs:TrailerRef" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
					
				</xsl:when>
			</xsl:choose>
		</xsl:element>
	</xsl:template>

	<xsl:template match="rs:Resource">
		<xsl:variable name="ResourceId" select="rs:ResourceId"/>
		
		<xsl:element name="ITEM">
			<xsl:element name="TEXT">
				<xsl:value-of select="rs:RegNo"/><xsl:value-of select="rs:TrailerRef"/><xsl:value-of select="rs:FullName"/>;<xsl:value-of select="$ResourceId"/>;<xsl:value-of select="rs:ResourceTypeId"/>
<!--
				<xsl:choose>
					<xsl:when test="rs:PreviousPointTrafficAreaId != $TrafficArea and rs:DuringPointTrafficAreaId != $TrafficArea and rs:AfterPointTrafficAreaId != $TrafficArea">;;</xsl:when>
					<xsl:otherwise>;<xsl:value-of select="$ResourceId"/>;<xsl:value-of select="rs:ResourceTypeId"/></xsl:otherwise>	
				</xsl:choose>
-->
			</xsl:element>

			<xsl:apply-templates select="../rs:Schedule[rs:ResourceId = $ResourceId]"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="rs:Schedule">

		<xsl:element name="BAR">
			<xsl:element name="TIMESTART">
				<xsl:value-of select="rs:StartMinutes"/>
			</xsl:element>
			<xsl:element name="TIMEEND">
				<xsl:value-of select="rs:EndMinutes"/>
			</xsl:element>
			<xsl:element name="DATESTART">
				<xsl:value-of select="rs:StartDayCount"/>
			</xsl:element>
			<xsl:element name="DATEEND">
				<xsl:value-of select="rs:EndDayCount"/>
			</xsl:element>
			<xsl:choose>
				<!-- Busy -->
				<xsl:when test="rs:ResourceActivityTypeId = 1"><xsl:element name="BARSTYLE"><![CDATA[1]]></xsl:element></xsl:when>			
				<xsl:when test="rs:ResourceActivityTypeId = 2"><xsl:element name="BARSTYLE"><![CDATA[1]]></xsl:element></xsl:when>			
				<xsl:when test="rs:ResourceActivityTypeId = 3"><xsl:element name="BARSTYLE"><![CDATA[1]]></xsl:element></xsl:when>			
				
				<!-- Free ?? -->
				<xsl:when test="rs:ResourceActivityTypeId = 4"><xsl:element name="BARSTYLE"><![CDATA[2]]></xsl:element></xsl:when>			
				<xsl:when test="rs:ResourceActivityTypeId = 5"><xsl:element name="BARSTYLE"><![CDATA[2]]></xsl:element></xsl:when>			
				<xsl:when test="rs:ResourceActivityTypeId = 6"><xsl:element name="BARSTYLE"><![CDATA[2]]></xsl:element></xsl:when>			
				
				<!-- Tentative -->
				<xsl:when test="rs:ResourceActivityTypeId = 7"><xsl:element name="BARSTYLE"><![CDATA[3]]></xsl:element></xsl:when>			
				<xsl:when test="rs:ResourceActivityTypeId = 8"><xsl:element name="BARSTYLE"><![CDATA[3]]></xsl:element></xsl:when>			
				<xsl:when test="rs:ResourceActivityTypeId = 9"><xsl:element name="BARSTYLE"><![CDATA[3]]></xsl:element></xsl:when>			
				
				<!-- Out of Office -->
				<xsl:when test="rs:ResourceActivityTypeId = 10"><xsl:element name="BARSTYLE"><![CDATA[4]]></xsl:element></xsl:when>			
			</xsl:choose>
			<xsl:element name="ALIGNMENT"><![CDATA[2]]></xsl:element>
			<xsl:element name="TEXT">
				<xsl:value-of select="rs:Display"/>
			</xsl:element>
			<xsl:element name="KEYID">
				<xsl:value-of select="rs:ResourceScheduleId"/>
			</xsl:element>
		</xsl:element>
	</xsl:template>

	<xsl:template name="OutputHeader">
		<xsl:param name="Text"	/>
		
		<xsl:element name="ITEM">
			<xsl:element name="TEXT"><xsl:value-of select="$Text" /></xsl:element>
			<xsl:element name="BACKCOLOR">7315965</xsl:element>
			<xsl:element name="FORECOLOR">0</xsl:element>
			<xsl:element name="HEADER">1</xsl:element>
		</xsl:element>
	</xsl:template>
	
	<xsl:template name="Columns">
		<xsl:element name="COLUMN">
			<xsl:element name="TEXT">Resource</xsl:element>
			<xsl:element name="WIDTH">266</xsl:element>
		</xsl:element>
		<xsl:element name="COLUMN">
			<xsl:element name="TEXT" />
			<xsl:element name="WIDTH">0</xsl:element>
		</xsl:element>
		<xsl:element name="COLUMN">
			<xsl:element name="TEXT" />
			<xsl:element name="WIDTH">0</xsl:element>
		</xsl:element>
		<xsl:element name="COLUMN">
			<xsl:element name="TEXT"	/>
			<xsl:element name="WIDTH">0</xsl:element>
		</xsl:element>
		
		<!-- Styles for Schedule Items -->
		
		<!-- Busy -->
		<xsl:element name="STYLE">
			<xsl:element name="BARHEIGHT">27</xsl:element>
			<xsl:element name="FILLSTYLE">0</xsl:element>
			<xsl:element name="BACKCOLOR">12422476</xsl:element>
			<xsl:element name="FORECOLOR">0</xsl:element>
		</xsl:element>
		
		<!-- Free -->
		<xsl:element name="STYLE">
			<xsl:element name="BARHEIGHT">27</xsl:element>
			<xsl:element name="FILLSTYLE">1</xsl:element>
			<xsl:element name="GRADFROM">16777215</xsl:element>
			<xsl:element name="GRADTO">16777215</xsl:element>
		</xsl:element>
		
		<!-- Tentative -->
		<xsl:element name="STYLE">
			<xsl:element name="BARHEIGHT">27</xsl:element>
			<xsl:element name="FILLSTYLE">6</xsl:element>
			<xsl:element name="BACKCOLOR">12422476</xsl:element>
		</xsl:element>
		
		<!-- Out of Office -->
		<xsl:element name="STYLE">
			<xsl:element name="BARHEIGHT">27</xsl:element>
			<xsl:element name="FILLSTYLE">1</xsl:element>
			<xsl:element name="FORECOLOR">0</xsl:element>
			<xsl:element name="GRADFROM">8388736</xsl:element>
			<xsl:element name="GRADTO">8388736</xsl:element>
		</xsl:element>
	</xsl:template>
	
</xsl:stylesheet>
