<%@ Page language="c#" Codebehind="ErrorPage.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.ErrorPage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Errore</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="CSS/AmmStyle.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="400" align="center" border="0">
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="right" height="10">|&nbsp;&nbsp;&nbsp;<A href="javascript: self.close()">Chiudi</A>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="left" height="48"><IMG height="48" src="Images/logo.gif" width="218" border="0"></td>
				</tr>
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td bgColor="#b16f6f" height="6"></td>
				</tr>
				<tr>
					<td align="center" class="testo_rosso">
						<br>
						<br>
						<img src="Images/alert.gif" border="0">
						<br>
						<br>
						ATTENZIONE!<br>
						<br>
						si è verificato un errore irreversibile.<br>
						<br>
					</td>
				</tr>
				<tr>
					<td bgColor="#b16f6f" height="6"></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
