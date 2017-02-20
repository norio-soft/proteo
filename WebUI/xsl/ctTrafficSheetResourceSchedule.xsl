<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="xml" version="4.0" omit-xml-declaration="yes"/>
	
	<xsl:param name="ScheduleStartDate"/>
	<xsl:param name="ScheduleStartTime"/>
	<xsl:param name="ScheduleEndDate"/>
	<xsl:param name="ScheduleEndTime"/>
	<xsl:param name="ResourceTypeId"/>
	<xsl:param name="DisplayHeaders">0</xsl:param>
	
	<xsl:include href="ctCommon.xsl" />
	
	<xsl:template match="Resources">
		<xsl:element name="ctScheduleXML">
			<xsl:call-template name="SetUp">
				<xsl:with-param name="ScheduleStartDate"><xsl:value-of select="$ScheduleStartDate" /></xsl:with-param>
				<xsl:with-param name="ScheduleEndDate"><xsl:value-of select="$ScheduleEndDate" /></xsl:with-param>
				<xsl:with-param name="ScheduleStartTime"><xsl:value-of select="$ScheduleStartTime" /></xsl:with-param>
				<xsl:with-param name="ScheduleEndTime"><xsl:value-of select="$ScheduleEndTime" /></xsl:with-param>
			</xsl:call-template>
			
			<xsl:call-template name="Columns" />
			
			<xsl:choose>
				<xsl:when test="$ResourceTypeId = 3">
					<!-- Drivers -->
					<xsl:if test="$DisplayHeaders = 1">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Drivers</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<xsl:for-each select="Resource[@ResourceTypeId = 3]">
						<xsl:sort select="FullName" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
				</xsl:when>
				<xsl:when test="$ResourceTypeId = 1">
					<!-- Vehicles -->
					<xsl:if test="$DisplayHeaders = 1">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Vehicles</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<xsl:for-each select="Resource[@ResourceTypeId = 1]">
						<xsl:sort select="RegNo" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
				</xsl:when>
				<xsl:when test="$DisplayHeaders = 2">
					<!-- Trailers -->
					<xsl:if test="$DisplayHeaders = 1">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Trailers</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<xsl:for-each select="Resource[@ResourceTypeId = 2]">
						<xsl:sort select="TrailerRef" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
				</xsl:when>
				<xsl:otherwise>
					<!-- Drivers -->
					<xsl:if test="$DisplayHeaders = 1">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Drivers</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<xsl:for-each select="Resource[@ResourceTypeId = 3]">
						<xsl:sort select="FullName" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
					
					<!-- Vehicles -->
					<xsl:if test="$DisplayHeaders = 1">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Vehicles</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<xsl:for-each select="Resource[@ResourceTypeId = 1]">
						<xsl:sort select="RegNo" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
					
					<!-- Trailers -->
					<xsl:if test="$DisplayHeaders = 1">
						<xsl:call-template name="OutputHeader">
							<xsl:with-param name="Text">Trailers</xsl:with-param>
						</xsl:call-template>
					</xsl:if>
					<xsl:for-each select="Resource[@ResourceTypeId = 2]">
						<xsl:sort select="TrailerRef" />
						
						<xsl:apply-templates select="."/>
					</xsl:for-each>
				</xsl:otherwise>
			</xsl:choose>

		</xsl:element>
	</xsl:template>

	<xsl:template match="Resource">
		<xsl:variable name="ResourceId" select="@ResourceId"/>
		
		<xsl:element name="ITEM">
			<xsl:element name="TEXT">
				<xsl:value-of select="@RegNo"/><xsl:value-of select="@TrailerRef"/><xsl:value-of select="@FullName"/>;<xsl:value-of select="$ResourceId"/>;<xsl:value-of select="@ResourceTypeId"/>
			</xsl:element>

			<xsl:apply-templates select="Schedules/Schedule"/>
		</xsl:element>
	</xsl:template>

	<xsl:template match="Schedule">

		<xsl:element name="BAR">
			<xsl:element name="TIMESTART">
				<xsl:value-of select="@StartMinutes"/>
			</xsl:element>
			<xsl:element name="TIMEEND">
				<xsl:value-of select="@EndMinutes"/>
			</xsl:element>
			<xsl:element name="DATESTART">
				<xsl:value-of select="@StartDayCount"/>
			</xsl:element>
			<xsl:element name="DATEEND">
				<xsl:value-of select="@EndDayCount"/>
			</xsl:element>
			<xsl:choose>
				<!-- Busy -->
				<xsl:when test="@ResourceActivityTypeId = 1"><xsl:element name="BARSTYLE"><![CDATA[1]]></xsl:element></xsl:when>			
				<xsl:when test="@ResourceActivityTypeId = 2"><xsl:element name="BARSTYLE"><![CDATA[1]]></xsl:element></xsl:when>			
				<xsl:when test="@ResourceActivityTypeId = 3"><xsl:element name="BARSTYLE"><![CDATA[1]]></xsl:element></xsl:when>			
				
				<!-- Free ?? -->
				<xsl:when test="@ResourceActivityTypeId = 4"><xsl:element name="BARSTYLE"><![CDATA[2]]></xsl:element></xsl:when>			
				<xsl:when test="@ResourceActivityTypeId = 5"><xsl:element name="BARSTYLE"><![CDATA[2]]></xsl:element></xsl:when>			
				<xsl:when test="@ResourceActivityTypeId = 6"><xsl:element name="BARSTYLE"><![CDATA[2]]></xsl:element></xsl:when>			
				
				<!-- Tentative -->
				<xsl:when test="@ResourceActivityTypeId = 7"><xsl:element name="BARSTYLE"><![CDATA[3]]></xsl:element></xsl:when>			
				<xsl:when test="@ResourceActivityTypeId = 8"><xsl:element name="BARSTYLE"><![CDATA[3]]></xsl:element></xsl:when>			
				<xsl:when test="@ResourceActivityTypeId = 9"><xsl:element name="BARSTYLE"><![CDATA[3]]></xsl:element></xsl:when>			
				
				<!-- Out of Office -->
				<xsl:when test="@ResourceActivityTypeId = 10"><xsl:element name="BARSTYLE"><![CDATA[4]]></xsl:element></xsl:when>			
			</xsl:choose>
			<xsl:element name="ALIGNMENT"><![CDATA[2]]></xsl:element>
			<xsl:element name="TEXT">
				<xsl:value-of select="@Display"/>
			</xsl:element>
			<xsl:element name="KEYID">
				<xsl:value-of select="@ResourceScheduleId"/>
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