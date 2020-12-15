<%@ Page language="c#" Codebehind="GestUtenti_inRuoloUO.aspx.cs" AutoEventWireup="false" Inherits="Amministrazione.Gestione_Organigramma.GestUtenti_inRuoloUO" %>
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
		<SCRIPT language="JavaScript">								
			function apriPopup() {
				window.open('../help.aspx?from=EUR','','width=450,height=500,scrollbars=YES');
			}			
			function AvvisoRuoloConTX(utente) 
			{
				var myUrl = "AvvisoRuoloConTrasm.aspx?utente="+utente;
				rtnValue = window.showModalDialog(myUrl,"","dialogWidth:410px;dialogHeight:350px;status:no;resizable:no;scroll:no;center:yes;help:no;"); 				
				
				frm_gestioneUtenti.hd_returnValueModal.value = rtnValue;
								
				window.document.frm_gestioneUtenti.submit();				
			}					
		</SCRIPT>
		<script language="javascript" id="btn_find_click" event="onclick()" for="btn_find">
			window.document.body.style.cursor='wait';
		</script>
	</HEAD>
	<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
		<form id="frm_gestioneUtenti" method="post" runat="server">
            <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione utenti nel ruolo" />	
			<input id="hd_returnValueModal" type="hidden" name="hd_returnValueModal" runat="server">
			<table width="740" align="center">
				<tr>
					<!-- OPZIONI DI TESTA -->
					<td class="testo_grigio_scuro_grande" align="right" height="10">|&nbsp;&nbsp;<A onclick="javascript: self.close();" href="#">Chiudi</A>&nbsp;&nbsp;|&nbsp;&nbsp;</td>
				</tr>
				<tr>
					<td align="left" height="48"><IMG height="48" src="../Images/logo.gif" width="218" border="0"></td>
				</tr>
				<tr>
					<!-- TITOLO PAGINA -->
					<td><asp:label id="lbl_percorso" tabIndex="6" runat="server" CssClass="label_percorso"></asp:label></td>
				</tr>
				<tr>
					<td class="testo_grigio_scuro_grande" height="2">&nbsp;</td>
				</tr>
			</table>
			<table height="390" cellSpacing="0" cellPadding="0" width="740" align="center" border="0">
				<tr>
					<td vAlign="top" align="center" width="58%">
						<!-- frame SX-->
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td class="testo_bianco" tabIndex="7" align="center" width="95" background="../Images/tasto_a.gif"
									height="25">Ricerca</td>
								<td width="320" background="../Images/pixel.gif" height="25"></td>
							</tr>
						</table>
						<table class="contenitore">
							<tr>
								<td class="pulsanti" vAlign="top">
									<!-- RICERCA UTENTE -->
									<table cellSpacing="1" cellPadding="0" width="100%" border="0">
										<tr>
											<td class="testo_grigio_scuro"><asp:dropdownlist id="ddl_ricerca" tabIndex="3" CssClass="testo_grigio_scuro_grande" Runat="server"
													AutoPostBack="True">
													<asp:ListItem Value="*" >Tutti</asp:ListItem>
													<asp:ListItem Value="var_cod_rubrica">Codice</asp:ListItem>
													<asp:ListItem Value="var_cognome" Selected="True">Cognome</asp:ListItem>
													<asp:ListItem Value="var_nome">Nome</asp:ListItem>
												</asp:dropdownlist></td>
											<td><asp:textbox id="txt_ricerca" tabIndex="1" CssClass="testo_grigio_scuro_grande" Runat="server"
													Width="270px"></asp:textbox></td>
											<td><asp:button id="btn_find" tabIndex="2" CssClass="testo_btn" Runat="server" Text="Cerca"></asp:button></td>
										</tr>
										<!-- AVVISI -->
										<tr>
											<td colSpan="3"><asp:label id="lbl_avviso" runat="server" CssClass="testo_rosso"></asp:label></td>
										</tr>
									</table>
								</td>
							</tr>
							<tr>
								<td>
									<!-- RISULTATO DELLA RICERCA -->
									<DIV style="OVERFLOW: auto; HEIGHT: 331px"><asp:datagrid id="dg_utentiTrovati" tabIndex="3" runat="server" Width="400px" PageSize="15" AllowPaging="True"
											ShowHeader="False" BorderColor="Gray" BorderWidth="1px" CellPadding="1" AutoGenerateColumns="False"
                                            >
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="IDUtente" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
												<asp:BoundColumn DataField="codice" ReadOnly="True" HeaderText="Codice">
													<ItemStyle Width="20%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="Utente">
													<ItemStyle Width="80%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm"></asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDPeople" ReadOnly="True"></asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/freccia_dx.gif border=0 alt='Inserisci'&gt;" CommandName="Inserimento">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
											</Columns>
											<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
										</asp:datagrid></DIV>
								</td>
							</tr>
						</table>
						<!-- fine frame SX--></td>
					<td vAlign="top" width="1%">&nbsp;
					</td>
					<td vAlign="top" align="center" width="41%">
						<!-- frame DX-->
						<table cellSpacing="0" cellPadding="0" border="0">
							<tr>
								<td class="testo_bianco" tabIndex="8" align="center" width="95" background="../Images/tasto_a.gif"
									height="25">Utenti nel ruolo</td>
								<td width="208" background="../Images/pixel.gif" height="25"></td>
							</tr>
						</table>
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td>
									<DIV style="OVERFLOW: auto; HEIGHT: 369px"><asp:label id="lbl_risultatoUtentiRuolo" tabIndex="4" CssClass="testo_grigio_scuro_grande"
											Runat="server" Visible="False">Nessun utente presente.</asp:label><asp:datagrid id="dg_utenti" OnItemDataBound="dg_utenti_ItemDataBaund" tabIndex="5" runat="server" Width="285px" ShowHeader="False" BorderColor="Gray"
											BorderWidth="1px" CellPadding="1" AutoGenerateColumns="False">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="IDUtente" ReadOnly="True"></asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="codice" ReadOnly="True"></asp:BoundColumn>
												<asp:BoundColumn DataField="descrizione" ReadOnly="True">
													<ItemStyle Width="95%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True"></asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDPeople" ReadOnly="True"></asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" CommandName="Eliminazione">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
											</Columns>
										</asp:datagrid></DIV>
								</td>
							</tr>
						</table>
						<!-- fine frame DX--></td>
				</tr>
			</table>
		</form>
	</body>
</HTML>
