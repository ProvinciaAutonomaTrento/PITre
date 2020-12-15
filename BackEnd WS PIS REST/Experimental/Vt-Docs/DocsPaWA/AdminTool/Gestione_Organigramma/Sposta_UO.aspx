<%@ Page language="c#" Codebehind="Sposta_UO.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Organigramma.Sposta_UO" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<SCRIPT language="JavaScript">						
			function apriPopup() {
				window.open('../help.aspx?from=SUO','','width=450,height=500,scrollbars=YES');
			}		
			function ApriOrganigramma() 
			{			
				var idAmm = Form1.hd_idAmm.value;
				var myUrl = "Navigazione_Organigramma.aspx?readonly=0&navigazione=1&selezione=1&subselezione=&idAmm="+idAmm;			
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:600px;dialogHeight:600px;status:no;resizable:yes;scroll:yes;center:yes;help:no;"); 				
				Form1.hd_returnValueModal.value = rtnValue;		
				window.document.Form1.submit();	
			}								
		</SCRIPT>
		<base target="_self">
		<script language="javascript" id="btn_sposta_click" event="onclick()" for="btn_sposta">
			if(document.getElementById ("txt_ricDesc").value != "")
			{
                var atipicita = '<%= DocsPAWA.Utils.GetAbilitazioneAtipicita() %>';
                
                var mex = 'AVVISO,\nvista la complessità delle operazioni da svolgere,\nil sistema potrebbe richiedere l\'attesa di un tempo piuttosto lungo.\n\n';

                if(atipicita == 'True')
                    mex += 'Attenzione! Questa procedura non avvierà il calcolo di atipicità sugli oggetti dei ruoli contenuti nella UO.\n\n';

                mex += 'Avviare ora la procedura?';

				if (!window.confirm(mex)) {return false};			
			}
			else
			{
				window.confirm('Attenzione, non è stata selezionata alcuna Uo di destinazione');
				return false;				
			}
			
					
			window.document.body.style.cursor='wait';
			
			var w_width = 550;
			var w_height = 350;					
			document.getElementById ("WAIT").style.top = 0;
			document.getElementById ("WAIT").style.left = 0;
			document.getElementById ("WAIT").style.width = w_width;
			document.getElementById ("WAIT").style.height = w_height;				
			document.getElementById ("WAIT").style.visibility = "visible";	
		</script>
	</HEAD>
	<body bottomMargin="3" leftMargin="3" topMargin="3" rightMargin="3" MS_POSITIONING="GridLayout">
		<form id="Form1" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Sposta UO in organigramma" />
			<!-- campi nascosti -->
			<INPUT id="hd_livelloUO_DaSpostare" type="hidden" name="hd_livelloUO_DaSpostare" runat="server">
			<INPUT id="hd_idCorrGlobUODaSpostare" type="hidden" name="hd_idCorrGlobUODaSpostare" runat="server">
			<INPUT id="hd_descUODaSpostare" type="hidden" name="hd_descUODaSpostare" runat="server">
			<INPUT id="hd_codUODaSpostare" type="hidden" name="hd_codUODaSpostare" runat="server">
			<INPUT id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<INPUT id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server"> <INPUT id="hd_idCorrGlobDest" type="hidden" name="hd_idCorrGlobDest" runat="server">
			<table width="500" align="center">
				<tr>
					<!-- OPZIONI DI TESTA -->
					<td class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A onclick="apriPopup();" href="#">Help</A>&nbsp;&nbsp;|&nbsp;&nbsp;<A onclick="javascript: self.close();" href="#">Chiudi</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" align="right" height="10">&nbsp;</td>
				</tr>
			</table>
			<div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:label id="lbl_intest1" CssClass="testo_piccolo" Runat="server">UO da spostare:</asp:label></div>
			<table cellSpacing="3" cellPadding="0" width="500" align="center" border="0">
				<tr>
					<!-- STRISCIA DELLA UO -->
					<td class="testo_grigio_scuro"><asp:label id="lbl_uo" runat="server"></asp:label></td>
				</tr>
			</table>
			<div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:label id="lbl_intest2" CssClass="testo_piccolo" Runat="server">Sposta sotto:</asp:label></div>
			<TABLE class="contenitore" cellSpacing="3" cellPadding="0" width="500" align="center" border="0">
				<TR>
					<TD class="testo_grigio_scuro">Codice:&nbsp;
						<asp:textbox id="txt_ricCod" Runat="server" CssClass="testo_grigio_scuro" Width="100" AutoPostBack="True"></asp:textbox><asp:textbox id="txt_ricDesc" tabIndex="1" Runat="server" CssClass="testo_grigio_scuro" Width="300"
							ReadOnly="True"></asp:textbox></TD>
					<TD><asp:imagebutton id="btn_org" Runat="server" AlternateText="Ricerca in organigramma" ImageUrl="../../images/proto/ico_titolario.gif"
							tabIndex="2"></asp:imagebutton></TD>
				</TR>
			</TABLE>
			<br>
			<div>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:label id="lbl_intest3" CssClass="testo_piccolo" Runat="server">Riepilogo:</asp:label></div>
			<TABLE class="contenitore" cellSpacing="3" cellPadding="0" width="500" align="center" border="0">
				<TR>
					<TD class="testo_grigio_scuro">Codice:</TD>
				</TR>
				<TR>
					<TD class="testo_grigio_scuro"><asp:textbox id="txt_codice" runat="server" CssClass="testo_verde" Width="490px"></asp:textbox></TD>
				</TR>
				<TR>
					<TD class="testo_grigio_scuro">Descrizione:</TD>
				</TR>
				<TR>
					<TD class="testo_grigio_scuro"><asp:textbox id="txt_descrizione" runat="server" CssClass="testo_verde" Width="490px"></asp:textbox></TD>
				</TR>
			</TABLE>
			<br>
			<table cellSpacing="0" cellPadding="3" width="500" align="center" border="0">
				<tr>
					<td align="center"><asp:button id="btn_sposta" runat="server" CssClass="testo_btn" Width="200px" Text="Sposta UO"></asp:button></td>
				</tr>
			</table>
			<DIV id="WAIT" style="BORDER-RIGHT: #840000 1px solid; BORDER-TOP: #840000 1px solid; VISIBILITY: hidden; BORDER-LEFT: #840000 1px solid; BORDER-BOTTOM: #840000 1px solid; POSITION: absolute; BACKGROUND-COLOR: #ffefd5">
				<table height="100%" cellSpacing="0" cellPadding="0" width="100%" border="0">
					<tr>
						<td style="FONT-SIZE: 10pt; FONT-FAMILY: Verdana" vAlign="middle" align="center">Procedura 
							di spostamento UO in esecuzione...<br>
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
