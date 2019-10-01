<%@ Register TagPrefix="uc3" TagName="MenuTendina" Src="../UserControl/MenuTendina.ascx" %>
<%@ Register TagPrefix="uc2" TagName="MenuLog" Src="../UserControl/MenuLogAmm.ascx" %>
<%@ Register TagPrefix="uc1" TagName="Testata" Src="../Gestione_VociMenu/Testata.ascx" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<%@ Page language="c#" Codebehind="GestLogsAmm.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Logs.GestLogsAmm" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD runat = "server">
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
							
			function apriPopup() {
				hlp = window.open('../help.aspx?from=GL','','width=450,height=500,scrollbars=YES');
			}					
			function cambiaPwd() {
				cambiapass = window.open('../CambiaPwd.aspx','','width=410,height=300,scrollbars=NO');
			}	
			function ClosePopUp()
			{	
				if(typeof(cambiapass) != 'undefined')
				{
					if(!cambiapass.closed)
						cambiapass.close();
				}
				if(typeof(hlp) != 'undefined')
				{
					if(!hlp.closed)
						hlp.close();
				}				
			}
		</SCRIPT>
	</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" MS_POSITIONING="GridLayout"
		onunload="ClosePopUp()">
		<form id="Form1" method="post" runat="server">
			<uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione Log Amministrazione" />
			<!-- Gestione del menu a tendina --><uc3:menutendina id="MenuTendina" runat="server"></uc3:menutendina>
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
					<!-- STRISCIA DEL TITOLO DELLA PAGINA -->
					<td class="titolo" align="center" bgColor="#e0e0e0" height="20">Gestione Log Amministrazione</td>
				</tr>
				<tr>
					<td vAlign="top" align="center" bgColor="#f6f4f4" height="100%">
						<!-- INIZIO: INSERIRE QUI IL CORPO CENTRALE -->
						<table height="100" cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr height="100%" width="100%">
								<td width="120" height="100%"><uc2:menulog id="MenuLogAmm" runat="server"></uc2:menulog></td>
								<td width="100%" height="100%" valign="middle" align="center">
									<DIV id="DivSel" style="OVERFLOW: auto; HEIGHT: 551px" class="testo_grigio_scuro"><br>
										<br>
										<br>
										<br>
										<br>
										<asp:Label id="lbl_seleziona" Runat="server" CssClass="testo_grigio_scuro">
										Selezionare una voce di menù...
										</asp:Label>
										<br>
										<br>
										<asp:Panel ID="pnl_archivio" Runat="server" Visible="False" Width="100%">
											<TABLE cellSpacing="0" cellPadding="15" border="1">
												<TR>
													<TD align="center">
														<asp:Label id="lbl_archivio" CssClass="testo_grigio_scuro" Runat="server"></asp:Label></TD>
												</TR>
											</TABLE>
										</asp:Panel>
									</DIV>
								</td>
							</tr>
						</table>
						<!-- FINE CORPO CENTRALE --></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
