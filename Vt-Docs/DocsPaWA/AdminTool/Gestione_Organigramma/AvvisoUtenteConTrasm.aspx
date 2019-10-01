<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AvvisoUtenteConTrasm.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_Organigramma.AvvisoUtenteConTrasm" %>
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
        <script type="text/javascript">
        function OpenExport(idPeople, idCorrGlobali) {
            var myUrl = "ExportDettagli.aspx?ExportType=TRASM_UTENTE&idPeople=" + idPeople + "&idCorrGlobali=" + idCorrGlobali;
            rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:600px;dialogHeight:200px;status:no;resizable:no;scroll:no;center:yes;help:no;");
        }
    </script>
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="frmAvviso" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Avviso utente con trasmissioni" />
			<table border="0" cellpadding="0" cellspacing="2" align="center" width="100%">
                <tr>
					<!-- OPZIONI DI TESTA -->
					<td colspan="2" class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A id="btn_Annulla" runat="server" href="#">Annulla</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="center" colspan="2"><br>
						<asp:Image ID="img_alert" ImageUrl="../Images/alert.gif" Runat="server"></asp:Image></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" align="center" colspan="3"><br>
						Attenzione!<br>
                        L'utente selezionato per la rimozione presenta<br>
                        trasmissioni che necessitano 'Accettazione'.<br><br>
						Proseguendo con l'operazione tutte le trasmissioni<br>
						saranno accettate.<br>
						<br>
						<br>
						<br>
					</td>
				</tr>
				<tr>
					<td align="center"><asp:Button ID="btnExport" CssClass="testo_btn" Runat="server" Text="Esporta Trasmissioni" OnClick="btnExport_Click" ToolTip="Esporta trasmissioni"></asp:Button></td>
					<td align="center"><asp:Button ID="btnRimuovi" CssClass="testo_btn" Runat="server" Text="Rimuovi utente" ToolTip="Rimuovi utente"></asp:Button></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>