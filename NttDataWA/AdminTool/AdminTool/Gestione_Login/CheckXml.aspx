<%@ Page language="c#" Codebehind="CheckXml.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Login.CheckXml" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>DOCSPA - AMMINISTRAZIONE > Verifica file XML</title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<SCRIPT language="JavaScript">	
			
			var hlp;
								
			function apriPopup() {
				hlp = window.open('../help.aspx?from=CX','','width=450,height=500,scrollbars=YES');
			}	
			function ClosePopUp()
			{					
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
			}	
		</SCRIPT>
		<script language="javascript" id="btn_si_click" event="onclick()" for="btn_si">
			window.document.body.style.cursor='wait';
		</script>
		<script language="javascript" id="btn_no_click" event="onclick()" for="btn_no">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body leftMargin="0" topMargin="0" MS_POSITIONING="GridLayout" onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
			<table cellSpacing="0" cellPadding="0" width="400" align="center" border="0">
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro" align="right" height="10">|&nbsp;&nbsp;&nbsp;<a href="#" onClick="apriPopup();">Help</a>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;<A href="../Exit.aspx">Chiudi</A>&nbsp;&nbsp;&nbsp;|&nbsp;&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="left" height="48"><IMG height="48" src="../Images/logo.gif" width="218" border="0"></td>
				</tr>
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td bgColor="#b16f6f" height="6"></td>
				</tr>
				<tr>
					<td align="center" height="15"><asp:label id="lbl_error" CssClass="titolo" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td align="center">
						<TABLE class="contenitore2" cellSpacing="0" cellPadding="5" border="0">
							<TR bgColor="lightsalmon">
								<TD class="titolo" align="center">Avviso</TD>
							</TR>
							<TR>
								<TD height="10"></TD>
							</TR>
							<TR>
								<TD class="titolo">
									<asp:Label id="lbl_msgAmm" runat="server"></asp:Label></TD>
							</TR>
							<TR>
								<TD height="10"></TD>
							</TR>
							<TR>
								<TD align="center">
									<asp:Button id="btn_si" runat="server" CssClass="testo_btn" Text="Ricarica dal server"></asp:Button>&nbsp;&nbsp;&nbsp;
									<asp:Button id="btn_no" runat="server" CssClass="testo_btn" Text="Mantieni"></asp:Button></TD>
							</TR>
						</TABLE>
					</td>
				</tr>
				<tr>
					<td height="6">&nbsp;</td>
				</tr>
				<tr>
					<td bgColor="#b16f6f" height="6"></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
