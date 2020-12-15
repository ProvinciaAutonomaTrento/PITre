<%@ Register TagPrefix="uc2" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Page language="c#" Codebehind="Verifica.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Homepage.Verifica" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD id="HEAD1" runat = "server">
        <title></title>	
		<meta content="Microsoft Visual Studio 7.0" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<script language="JavaScript" src="../CSS/caricamenujs.js"></script>
		<SCRIPT language="JavaScript">

		    var cambiapass;
		    var hlp;

		    function Init() {

		    }
		    function apriPopup() {
		        hlp = window.open('../help.aspx?from=HP', '', 'width=450,height=500,scrollbars=YES');
		    }
		    function cambiaPwd() {
		        cambiapass = window.open('../CambiaPwd.aspx', '', 'width=410,height=300,scrollbars=NO');
		    }
		    function ClosePopUp() {
		        if (typeof (cambiapass) != 'undefined') {
		            if (!cambiapass.closed)
		                cambiapass.close();
		        }
		        if (typeof (hlp) != 'undefined') {
		            if (!hlp.closed)
		                hlp.close();
		        }
		    }
		</SCRIPT>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" 
		rightMargin="0" onunload="Init();ClosePopUp()">
		<form id="Form1" method="post" runat="server">
		    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Verifica" />
			<input id="hd_idAmm" type="hidden" name="hd_idAmm" runat="server"> 
			<!-- Gestione del menu a tendina --><uc2:menutendina id="MenuTendina" runat="server"></uc2:menutendina>
			<table height="100%" cellSpacing="1" cellPadding="0" width="100%" border="0">
				<tr>
					<td>
						<!-- TESTATA CON MENU' --><uc1:testata id="Testata" runat="server"></uc1:testata></td>
				</tr>
				<tr>
					<!-- STRISCIA SOTTO IL MENU -->
					<td class="testo_grigio_scuro" bgColor="#c0c0c0" height="20"><asp:label id="lbl_position" runat="server"></asp:label></td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td align="center" colSpan="2" height="25"><asp:label id="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:label></td>
							</tr>
							<tr>
								<td vAlign="top" width="650">
									<table width="100%" border="0">
										<tr>
											<td class="pulsanti" height="30">
												<TABLE cellSpacing="0" cellPadding="0">
													<TR>
														<TD class="testo_grigio_scuro" align="left" width="60%">Numero di documenti in Ingresso senza Mittente &nbsp;&nbsp;
														<TD align="right" width="40%"><asp:Label runat="server" ID="lbl_doc_no_mitt"></asp:Label></asp:button></TD>
													</TR>
                                                    <tr><td height="10px">&nbsp;</td></tr>
													<TR>
														<TD class="testo_grigio_scuro" align="left" width="60%">Numero di documenti in Uscita senza Destinatari &nbsp;&nbsp;
														<TD align="right" width="40%"><asp:Label runat="server" ID="lbl_doc_no_dest"></asp:Label></asp:button></TD>
													</TR>
                                                    <tr><td height="10px">&nbsp;</td></tr>
													<TR>
														<TD class="testo_grigio_scuro" align="left" width="60%">Numero di documenti con Destinatari in CC nulli &nbsp;&nbsp;
														<TD align="right" width="40%"><asp:Label runat="server" ID="lbl_doc_cc"></asp:Label></asp:button></TD>
													</TR>
												</TABLE>
											</td>
										</tr>
                                    </table>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
