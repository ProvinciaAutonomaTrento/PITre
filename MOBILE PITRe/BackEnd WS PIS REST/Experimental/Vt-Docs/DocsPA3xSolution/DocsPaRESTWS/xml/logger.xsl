<?xml version="1.0"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:template match="DEBUG">
<html>
	<head>
		<title>DOCSPA > Debug Log</title>
	<style>
	BODY 
	{
		background: #FFFFFF;
	}
	.AllText 
	{
		font-size: 12px; 
		font-family: verdana;
	}
	.Hidden 
	{
		color: #00FFFF;
	}
	</style>
	</head>
	<body>
		<table align="center" border="0" width="650" height="50">				
				<tr bgcolor="#FFFFFF">
					<td align="center">
					<font color="#0000FF">
					<b>DocsPA 3.0 - Debug Log</b>
					</font>
					</td>
				</tr>
		</table>
		
		<xsl:for-each select="LOGENTRY">
			<table class="AllText" align="center" border="0" width="650" bgcolor="black" cellspacing="1">							
				<tr bgcolor="#0000FF">
					<td>
					<font color="#FFFFFF">
					<b>Debug: <xsl:value-of select="@date" /></b>
					</font>
					</td>
				</tr>
				
				<xsl:for-each select="MESSAGE">
				<tr bgcolor="#A0FFFF">
					<td align="left">
					<b><xsl:value-of select="." /></b>
					</td>
				</tr>
				</xsl:for-each>
				
				<tr bgcolor="#FFFFFF" height="2">
					<td></td>
				</tr>
				
				<xsl:for-each select="EXCEPTION">
				<tr bgcolor="#FFFA90">
					<td align="left">
					<b><xsl:value-of select="." /></b>
					</td>
				</tr>
				</xsl:for-each>
				
			</table>
			<br/>
		</xsl:for-each>
	</body>
</html>
</xsl:template>
</xsl:stylesheet>
