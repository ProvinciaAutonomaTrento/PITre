<%@ Page language="c#" Codebehind="AvvisoRuoloConTrasm.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Organigramma.AvvisoRuoloConTrasm" %>
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
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="frmAvviso" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Avviso ruolo con trasmissioni" />
			<table border="0" cellpadding="0" cellspacing="3" align="center" width="100%">
				<tr>
					<!-- OPZIONI DI TESTA -->
					<td colspan="2" class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A onclick="javascript: self.close();" href="#">Annulla</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="center" colspan="2"><br>
						<asp:Image ID="img_alert" ImageUrl="../Images/alert.gif" Runat="server"></asp:Image></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" align="center" colspan="2"><br>
						Attenzione!<br>
						stai eliminando l'unico utente del ruolo.<br>
						<br>
						Poiché il ruolo presenta trasmissioni<br>
						che necessitano 'Accettazione',<br>
						non è consigliato lasciare il ruolo senza utenti.<br>
						<br>
						Vuoi inserire ora un nuovo utente che<br>
						prenderà il posto di
						<asp:Label ID="lbl_utente" Runat="server"></asp:Label>?
						<br>
						<br>
						<br>
					</td>
				</tr>
				<tr>
					<td align="center"><asp:Button ID="btn_si" CssClass="testo_btn" Runat="server" Text="Sì, inserisco nuovo utente"></asp:Button></td>
					<td align="center"><asp:Button ID="btn_no" CssClass="testo_btn" Runat="server" Text="No, non importa"></asp:Button></td>
				</tr>
				<tr>
					<td class="testo" align="center">sarà obbligatorio inserire<br>
						un nuovo utente</td>
					<td class="testo" align="center">
						tutte le trasmissioni<br>
						saranno rifiutate</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
