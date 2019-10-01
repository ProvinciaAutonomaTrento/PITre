<%@ Page language="c#" Codebehind="Sposta_Utente.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Organigramma.Sposta_Utente" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language="javascript" id="btn_sposta_click" event="onclick()" for="btn_sposta">		
			window.document.body.style.cursor='wait';
			
			var w_width = 550;
			var w_height = 250;				
			document.getElementById ("WAIT").style.top = 0;
			document.getElementById ("WAIT").style.left = 0;
			document.getElementById ("WAIT").style.width = w_width;
			document.getElementById ("WAIT").style.height = w_height;				
			document.getElementById ("WAIT").style.visibility = "visible";				
		</script>
		<SCRIPT language="JavaScript">						
		function apriPopup() {
			window.open('../help.aspx?from=SU','','width=450,height=500,scrollbars=YES');
		}			
		function ApriOrganigramma() 
		{			
			var idAmm = Form1.hd_idAmm.value;
			var myUrl = "Navigazione_Organigramma.aspx?readonly=0&navigazione=2&selezione=&subselezione=2&idAmm="+idAmm;						
			rtnValue = window.showModalDialog(myUrl,"","dialogWidth:600px;dialogHeight:600px;status:no;resizable:yes;scroll:yes;center:yes;help:no;"); 				
			Form1.hd_returnValueModal.value = rtnValue;		
			window.document.Form1.submit();	
			//window.open(myUrl,'','width=600,height=600,scrollbars=YES,resizable=YES');
		}
		function AvvisoRuoloConLF(tipoTitolare, idRuolo, idUtente, numProcessi, numIstanze) {
		    var myUrl = "AvvisoRuoloConLF.aspx?tipoTitolare=" + tipoTitolare + "&idRuolo=" + idRuolo + "&idUtente=" + idUtente + "&numProcessi=" + numProcessi + "&numIstanze=" + numIstanze;
		    rtnValue = window.showModalDialog(myUrl, "", "dialogWidth:450px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;");

		    frm_gestioneUtenti.hd_returnValueModalLF.value = rtnValue;

		    window.document.frm_gestioneUtenti.submit();
		}
		</SCRIPT>
	</HEAD>
	<body bottomMargin="3" leftMargin="3" topMargin="3" rightMargin="3" MS_POSITIONING="GridLayout" onload="document.Form1.txt_ricCod.focus();">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Sposta utente in organigramma" />
			<input id="hd_idCorrGlobUtente" type="hidden" name="hd_idCorrGlobUtente" runat="server">
			<input id="hd_userid" type="hidden" name="hd_userid" runat="server"> <input id="hd_countUtenti" type="hidden" name="hd_countUtenti" runat="server">
			<input id="hd_idCorrGlobGruppo" type="hidden" name="hd_idCorrGlobGruppo" runat="server">
			<input id="hd_idGruppo" type="hidden" name="hd_idGruppo" runat="server"> <input id="hd_idPeople" type="hidden" name="hd_idPeople" runat="server">
			<input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server"> <input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<input id="hd_idGruppoDest" type="hidden" name="hd_idGruppoDest" runat="server">
			<input id="hd_idCorrGlobGruppoDest" type="hidden" name="hd_idCorrGlobGruppoDest" runat="server">
            <input id="hd_returnValueModalLF" type="hidden" name="hd_returnValueModalLF" runat="server">
			<table width="500" align="center">
				<tr>
					<!-- OPZIONI DI TESTA -->
					<td class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A onclick="apriPopup();" tabIndex="5" href="#">Help</A>&nbsp;&nbsp;|&nbsp;&nbsp;<A onclick="javascript: self.close();" tabIndex="6" href="#">Chiudi</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" align="right" height="10">&nbsp;</td>
				</tr>
			</table>
			<div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:label id="lbl_intest1" Runat="server" CssClass="testo_piccolo">Utente da spostare:</asp:label></div>
			<table cellSpacing="3" cellPadding="0" width="500" align="center" border="0">
				<tr>
					<!-- STRISCIA DELL'UTENTE -->
					<td class="titolo"><asp:label id="lbl_utente" runat="server"></asp:label></td>
				</tr>
			</table>
			<br>
			<div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:label id="lbl_intest2" Runat="server" CssClass="testo_piccolo">Ruolo di destinazione:</asp:label></div>
			<TABLE class="contenitore" cellSpacing="3" cellPadding="0" width="500" align="center" border="0">
				<TR>
					<TD class="testo_grigio_scuro">Codice:&nbsp;
						<asp:textbox id="txt_ricCod" Runat="server" CssClass="testo_grigio_scuro" Width="100" AutoPostBack="True"></asp:textbox><asp:textbox id="txt_ricDesc" tabIndex="1" Runat="server" CssClass="testo_grigio_scuro" Width="300"
							ReadOnly="True"></asp:textbox></TD>
					<TD><asp:imagebutton id="btn_org" Runat="server" AlternateText="Ricerca in organigramma" ImageUrl="../../images/proto/ico_titolario.gif"></asp:imagebutton></TD>
				</TR>
			</TABLE>
			<br>
			<table cellSpacing="0" cellPadding="3" width="500" align="center" border="0">
				<tr>
					<td align="center"><asp:button id="btn_sposta" tabIndex="4" runat="server" CssClass="testo_btn" Width="200px" Text="Sposta utente"></asp:button></td>
				</tr>
			</table>
			<DIV id="WAIT" style="BORDER-RIGHT: #840000 1px solid; BORDER-TOP: #840000 1px solid; VISIBILITY: hidden; BORDER-LEFT: #840000 1px solid; BORDER-BOTTOM: #840000 1px solid; POSITION: absolute; BACKGROUND-COLOR: #ffefd5">
				<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Procedura 
							di spostamento utente in esecuzione...<br>
							<br>
							<br>
							Si raccomanda di attendere la fine della procedura<br>
							e<br>
							verificare l'esito delle operazioni.
						</td>
					</tr>
				</table>
			</DIV>
		</form>
	</body>
</HTML>
