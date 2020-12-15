<%@ Page language="c#" Codebehind="Esito_Sposta_Ruolo.aspx.cs" AutoEventWireup="false" Inherits="SAAdminTool.AdminTool.Gestione_Organigramma.Esito_Sposta_Ruolo" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
		<title></title>
		<meta name="GENERATOR" Content="Microsoft Visual Studio .NET 7.1">
		<meta name="CODE_LANGUAGE" Content="C#">
		<meta name="vs_defaultClientScript" content="JavaScript">
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self">
	</HEAD>
	<body MS_POSITIONING="GridLayout" onload="document.Form1.si.focus();">
		<form id="Form1" method="post" runat="server">
        	<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Esito Sposta ruolo tra UO" />
			<input type="hidden" name="idAmm" id="idAmm" runat="server"> <input type="hidden" name="idCorrGlobUO" id="idCorrGlobUO" runat="server">
			<input type="hidden" name="idCorrGlobRuolo" id="idCorrGlobRuolo" runat="server">
			<input type="hidden" name="idGruppo" id="idGruppo" runat="server">
			<table height="350" cellSpacing="0" cellPadding="0" width="550" border="0">
				<tr>
					<td style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Operazione 
						di spostamento ruolo eseguita correttamente.<br>
						<br>
						<br>
						<br>
						Impostare ora la visibilità sui documenti e sui fascicoli?
						<br>
						<br>
						<asp:Button ID="si" Runat="server" CssClass="testo_btn" Text="Sì, ora" Width="120px" TabIndex="0"></asp:Button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
						<asp:Button ID="no" Runat="server" CssClass="testo_btn" Text="No, successivamente" Width="150px"
							TabIndex="1"></asp:Button>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
