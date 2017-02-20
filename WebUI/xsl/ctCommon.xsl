<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:rs="http://www.tempuri.org/ResourceScheduleData.xsd">
	<xsl:output method="xml" version="4.0" omit-xml-declaration="yes"/>

	<xsl:template name="SetUp">
		<xsl:param name="ScheduleStartDate" />
		<xsl:param name="ScheduleEndDate" />
		<xsl:param name="ScheduleStartTime" />
		<xsl:param name="ScheduleEndTime" />
		
		<xsl:element name="VER">7</xsl:element>
		
		<!-- Background -->
		<xsl:element name="BACKCOLOR">16777215</xsl:element>
		<xsl:element name="TODAYLINE">0</xsl:element>
		<xsl:element name="TODAYLINECOLOR">255</xsl:element>
		<xsl:element name="TODAYLINESIZE">1</xsl:element>
		<xsl:element name="TODAYLINEWIDTH">2</xsl:element>
		<xsl:element name="WEEKENDCOLOR">16777215</xsl:element>
		
		<!-- Border -->
		<xsl:element name="BORDERTYPE">0</xsl:element>
		
		<!-- Confilct -->
		<xsl:element name="CONFLICTOFFSET">0</xsl:element>
		
		<!-- Color Bar -->
		
		<!-- Date -->
		<xsl:element name="DATESTART"><xsl:value-of select="$ScheduleStartDate"/></xsl:element>
		<xsl:element name="DATEEND"><xsl:value-of select="$ScheduleEndDate"/></xsl:element>
		<xsl:element name="DATEFORMAT">D, M n/Y</xsl:element>
		<xsl:element name="DATESINBARS">0</xsl:element>
		
		<!-- Font -->
		<xsl:element name="FONTINDEX1">Verdana,8.250000,0,0,0,0</xsl:element>
		<xsl:element name="FONTINDEX2">Verdana,8.250000,0,0,0,0</xsl:element>
		<xsl:element name="FONTINDEX3">Verdana,8.250000,0,0,0,0</xsl:element>
		<xsl:element name="FONTINDEX4">Verdana,8.250000,0,0,0,0</xsl:element>
		
		<!-- Image -->
		<xsl:element name="MASKBITMAP">0</xsl:element>
		<xsl:element name="MASKCOLOR">16711680</xsl:element>
		
		<!-- List -->
		<xsl:element name="ALTLISTCOLOREVEN">14474460</xsl:element>
		<xsl:element name="ALTLISTCOLORODD">15658734</xsl:element>
		<xsl:element name="ALTLISTCOLORS">255</xsl:element>
		<xsl:element name="ALTLISTITEMS">1</xsl:element>
		<xsl:element name="BORDERCOLOR">0</xsl:element>
		<xsl:element name="BARFONT">Verdana,6.000000,0,1,0,0</xsl:element>
		<xsl:element name="BARSELECTFOCUS">1</xsl:element>
		<xsl:element name="BARTEXTWRAP">1</xsl:element>
		<xsl:element name="BARTIPEXT">0</xsl:element>
		<xsl:element name="BARTIPSIZE">5</xsl:element>
		<xsl:element name="BARMINSIZE">0</xsl:element>
		<xsl:element name="CHECKXOFFSET">0</xsl:element>
		<xsl:element name="CHECKYOFFSET">0</xsl:element>
		<xsl:element name="FILLLIST">1</xsl:element>
		<xsl:element name="HEIGHTOFFSET">0</xsl:element>
		<xsl:element name="HORZSCROLTYPE">0</xsl:element>
		<xsl:element name="ITEMFORECOLOR">0</xsl:element>
		<xsl:element name="ITEMHEADERCOLOR">16777215</xsl:element>
		<xsl:element name="LISTBACKCOLOR">16777215</xsl:element>
		<xsl:element name="LISTBARCOLOR">16777215</xsl:element>
		<xsl:element name="LISTBARWIDTH">8</xsl:element>
		<xsl:element name="LISTBORDER">0</xsl:element>
		<xsl:element name="LISTFONT">Verdana,8.000000,0,0,0,0</xsl:element>
		<xsl:element name="LISTHEADERFONT">Verdana,8.250000,0,1,0,0</xsl:element>
		<xsl:element name="LISTFORECOLOR">5789784</xsl:element>
		<xsl:element name="LISTINDENT">8</xsl:element>
		<xsl:element name="LISTWIDTH">123</xsl:element>
		<xsl:element name="LOCKLISTBAR">1</xsl:element>
		<xsl:element name="MAJORVALUE">30</xsl:element>
		<xsl:element name="PICALIGN">0</xsl:element>
		<xsl:element name="PICMAXWIDTH">-1</xsl:element>
		<xsl:element name="PICXOFFSET">0</xsl:element>
		<xsl:element name="PICYOFFSET">0</xsl:element>
		<xsl:element name="PRECOLBACKCOLOR">2147483663</xsl:element>
		<xsl:element name="PRECOLFORECOLOR">2147483666</xsl:element>
		<xsl:element name="PRECOLBORDER">2</xsl:element>
		<xsl:element name="PRECOLTYPE">0</xsl:element>
		<xsl:element name="PRECOLFONT">MS Sans Serif,8.250000,0,0,0,0</xsl:element>
		<xsl:element name="PRECOLWIDTH">30</xsl:element>
		<xsl:element name="PRESELBACKCOLOR">-1</xsl:element>
		<xsl:element name="PRESELFORECOLOR">-1</xsl:element>
		<xsl:element name="PRESELBORDER">2</xsl:element>
		<xsl:element name="SELECTBACKCOLOR">2147483663</xsl:element>
		<xsl:element name="SELECTFORECOLOR">2147483666</xsl:element>
		<xsl:element name="SMALLICONS">0</xsl:element>
		<xsl:element name="TITLEALIGN">0</xsl:element>
		<xsl:element name="TITLEBACKCOLOR">16171169</xsl:element>
		<xsl:element name="TITLEBACKCOLORTO">16171169</xsl:element>
		<xsl:element name="TITLEBORDER">2</xsl:element>
		<xsl:element name="TITLEFORECOLOR">0</xsl:element>
		<xsl:element name="TITLEHEIGHT">40</xsl:element>
		<xsl:element name="TITLEFILLTYPE">0</xsl:element>
		<xsl:element name="TITLEFONT">Verdana,11.250000,0,1,0,0</xsl:element>
		<xsl:element name="TITLEPOS">2</xsl:element>
		<xsl:element name="TITLEXOFFSET">0</xsl:element>
		<xsl:element name="TITLEYOFFSET">0</xsl:element>
				
		<!-- JPG -->
		<xsl:element name="MAXJPGWIDTH">0</xsl:element>
		<xsl:element name="MAXJPGHEIGHT">0</xsl:element>

		<!-- Performance -->
		
		<!-- Print -->
		
		<!-- Schedule -->
		<xsl:element name="ALTSCHEDCOLOREVEN">14474460</xsl:element>
		<xsl:element name="ALTSCHEDCOLORODD">15658734</xsl:element>
		<xsl:element name="ALTSCHEDCOLORS">255</xsl:element>
		<xsl:element name="ALTSCHEDITEMS">1</xsl:element>
		<xsl:element name="DISPLAYONLY">1</xsl:element>
		<xsl:element name="TIMESTART"><xsl:value-of select="$ScheduleStartTime"/></xsl:element>
		<xsl:element name="TIMEEND"><xsl:value-of select="$ScheduleEndTime"/></xsl:element>
		
		<!-- Style -->
		<xsl:element name="DEFAULTSTYLE">0</xsl:element>
		
		<!-- Time Bar -->
		<xsl:element name="BARDATETEXTCOLOR">0</xsl:element>
		<xsl:element name="BARDEFBACKCOLOR">255</xsl:element>
		<xsl:element name="BARDEFFORECOLOR">0</xsl:element>
		<xsl:element name="BARSELECTBACKCOLOR">-1</xsl:element>
		<xsl:element name="BARSELECTBORDERCOLOR">-1</xsl:element>
		<xsl:element name="BARSELECTFORECOLOR">-1</xsl:element>
		<xsl:element name="DISPLAYSUBTEXT">0</xsl:element>
		<xsl:element name="GRIDFREQ">0</xsl:element>
		<xsl:element name="OVERLAPCOLOR">16777215</xsl:element>
		<xsl:element name="OVERLAPLINES">0</xsl:element>
		<xsl:element name="OVERLAPWIDTH">0</xsl:element>
		<xsl:element name="SNAPTOGRID">0</xsl:element>
		<xsl:element name="TEXTINTOVIEW">0</xsl:element>
		<xsl:element name="VERTREPOSITION">0</xsl:element>
		
		<!-- Time Lines -->
		<xsl:element name="TIMELINEBREAK">1</xsl:element>
		<xsl:element name="TIMELINECOLOR">2147483664</xsl:element>
		<xsl:element name="TIMELINES">255</xsl:element>
		<xsl:element name="TIMELINESTART">0</xsl:element>
		<xsl:element name="TIMELINEWIDTH">1</xsl:element>
		
		<!-- Time Ruler -->
		<xsl:element name="DAYNAMES">Sunday;Monday;Tuesday;Wednesday;Thursday;Friday;Saturday</xsl:element>
		<xsl:element name="INCLUDEMINUTES">255</xsl:element>
		<xsl:element name="INCLUDEAMPM">0</xsl:element>
		<xsl:element name="LINEMAXSIZE">5</xsl:element>
		<xsl:element name="LINEMINSIZE">3</xsl:element>
		<xsl:element name="MARKERBARCOLOR">255</xsl:element>
		<xsl:element name="MARKERBARS">1</xsl:element>
		<xsl:element name="MILITARYTIME">255</xsl:element>
		<xsl:element name="MONTHLYTYPE">1</xsl:element>
		<xsl:element name="MONTHNAMES">January,February,March,April,May,June,July,August,September,October,November,December</xsl:element>
		<xsl:element name="RULERALIGN">3</xsl:element>
		<xsl:element name="RULERBACKCOLOR">16171169</xsl:element>
		<xsl:element name="RULERBACKCOLORTO">16171169</xsl:element>
		<xsl:element name="RULERFORECOLOR">0</xsl:element>
		<xsl:element name="RULERLINECOLOR">0</xsl:element>
		<xsl:element name="RULERBORDER">2</xsl:element>
		<xsl:element name="RULERDAYS">0</xsl:element>
		<xsl:element name="RULERDAYSIZE">1</xsl:element>
		<xsl:element name="RULERDIVISIONS">2</xsl:element>
		<xsl:element name="RULERFILLTYPE">0</xsl:element>
		<xsl:element name="RULERFONT">Verdana,6.750000,0,0,0,0</xsl:element>
		<xsl:element name="RULERSPLIT">1</xsl:element>
		<xsl:element name="RULERTITLE"/>
		<xsl:element name="RULERTHREED">0</xsl:element>
		<xsl:element name="RULERXOFFSET">0</xsl:element>
		<xsl:element name="RULERYOFFSET">0</xsl:element>
		<xsl:element name="STARTWEEKDAY">0</xsl:element>
		<xsl:element name="TIMEDISTANCE">35</xsl:element>
		<xsl:element name="TIMETAGAM">AM</xsl:element>
		<xsl:element name="TIMETAGPM">PM</xsl:element>
		<xsl:element name="TIMETYPE">0</xsl:element>
		<xsl:element name="WEEKENDDATES">1</xsl:element>
		<xsl:element name="WEEKENDS">65</xsl:element>
		<xsl:element name="YEARLYTYPE">1</xsl:element>
		
		<!-- Tool Tips -->
		<xsl:element name="TIPSBACKCOLOR">16171169</xsl:element>
		<xsl:element name="TIPSDELAY">0</xsl:element>
		<xsl:element name="TIPSDISTANCE">1</xsl:element>
		<xsl:element name="TIPSFONT">Verdana,6.000000,0,0,0,0</xsl:element>
		<xsl:element name="TIPSFORECOLOR">0</xsl:element>
		<xsl:element name="TIPSPOSITION">0</xsl:element>
		<xsl:element name="TIPSTEXTALIGN">0</xsl:element>
		<xsl:element name="TIPSTYPE">3</xsl:element>
		
	</xsl:template>

</xsl:stylesheet>

  