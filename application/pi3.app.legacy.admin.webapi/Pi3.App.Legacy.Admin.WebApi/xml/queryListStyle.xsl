<?xml version='1.0'?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
<xsl:template match="/">
<html>
	<head>
		<title>********** Lista query **********</title>
	<style>
	BODY {
		background: #FFFFFF;
	}
    A:link {color: #80FF00}
    A:visited {color: #FF00FF}
    H1 {font-size: 24pt; font-family: arial}
    H2 {font-size: 18pt; font-family: arial}
    H3 {font-size: 14pt; font-family: arial}
	</style>
	</head>
	<body>
		
		<xsl:for-each select="listeQuery">
			<table border="0" width="100%" bgcolor="black" cellspacing="1">				
				<tr bgcolor="#FFBB00">
					<td width="30%" align="center"><b>Nome query</b></td>
					<td width="70%" align="center"><b>query</b></td>
				</tr>
				<xsl:for-each select="query">
				<tr bgcolor="yellow">
					<td align="left">
						<xsl:value-of select="name" /></td>
						
					<td align="left">
						<xsl:value-of select="value" /></td>
				</tr>
				</xsl:for-each>
			</table>
			<br/>
		</xsl:for-each>
	</body>
</html>
</xsl:template>
</xsl:stylesheet>
