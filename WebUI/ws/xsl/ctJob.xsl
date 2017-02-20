<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rs="http://www.tempuri.org/ResourceScheduleData.xsd">
	<xsl:output method="xml" version="4.0" omit-xml-declaration="yes"/>
	
	<xsl:param name="ScheduleStartDate"/>
	<xsl:param name="ScheduleStartTime"/>
	<xsl:param name="ScheduleEndDate"/>
	<xsl:param name="ScheduleEndTime"/>
	<xsl:param name="TrafficArea"/>
	<xsl:param name="ShowComingIn" />
	<xsl:param name="ShowStayingIn" />
	<xsl:param name="ShowGoingOut" />
	<xsl:param name="ShowNotAvailable" />
	
	<xsl:include href="ctCommon.xsl" />
	
</xsl:stylesheet>

  