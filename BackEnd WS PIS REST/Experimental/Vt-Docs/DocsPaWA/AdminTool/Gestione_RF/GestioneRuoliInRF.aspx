<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestioneRuoliInRF.aspx.cs" Inherits="DocsPAWA.AdminTool.Gestione_RF.GestioneRuoliInRF" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
        <title></title>
        <meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
		<base target="_self">
		<script language=javascript>
		
		</script>
</head>
<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout" >
    <form id="formRuoliRF" runat="server" method="post">
    <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione ruoli in RF" />
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
					<td vAlign="top" align="left" width="58%">
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
									<!-- RICERCA RUOLO -->
									<table cellSpacing="1" cellPadding="0" width="100%" border="0">
										<tr>
										    <td class="testo_grigio_scuro">
										        <asp:DropDownList ID="ddl_tipoRicerca" runat="server" CssClass="testo_grigio_scuro_grande">
										            <asp:ListItem Value="Ruolo" Selected="True">Ruolo</asp:ListItem>
										            <asp:ListItem Value="UO">UO</asp:ListItem>
										        </asp:DropDownList>
										    </td>
											<td class="testo_grigio_scuro"><asp:dropdownlist id="ddl_ricerca" tabIndex="3" CssClass="testo_grigio_scuro_grande" Runat="server"
													AutoPostBack="True" OnSelectedIndexChanged="ddl_ricerca_SelectedIndexChanged">
													<asp:ListItem Value="*" >Tutti</asp:ListItem>
													<asp:ListItem Value="var_cod_rubrica">Codice</asp:ListItem>
													<asp:ListItem Value="var_desc_corr" Selected="True">Descrizione</asp:ListItem>
												</asp:dropdownlist></td>
											<td><asp:textbox id="txt_ricerca" tabIndex="1" CssClass="testo_grigio_scuro_grande" Runat="server"
													Width="270px"></asp:textbox></td>
											<td><asp:button id="btn_find" tabIndex="2" CssClass="testo_btn" Runat="server" Text="Cerca" OnClick="btn_find_Click"></asp:button></td>
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
								    <asp:Panel ID="pnl_ruoli" runat="server">
									<!-- RISULTATO DELLA RICERCA -->
									<DIV id="div_ruoli" runat="server" style="OVERFLOW: auto; HEIGHT: 331px"><asp:datagrid id="dg_ruoliTrovatiInRF" 
                                 tabIndex="3" runat="server" Width="430px" OnPreRender="dg_ruoliTrovatiInRF_PreRender"
											ShowHeader="False" BorderColor="Gray" BorderWidth="1px" CellPadding="1" 
                                 AutoGenerateColumns="False" OnItemCommand="dg_ruoliTrovatiInRF_ItemCommand" 
                                 AllowPaging="True" 
                                 onpageindexchanged="dg_ruoliTrovatiInRF_PageIndexChanged">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="IDRuolo" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
												<asp:BoundColumn DataField="codice" ReadOnly="True" HeaderText="Codice">
													<ItemStyle Width="25%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="Ruolo">
													<ItemStyle Width="70%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True" HeaderText="IDAmm"></asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDGruppo" ReadOnly="True"></asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/freccia_dx.gif border=0 alt='Inserisci'&gt;" CommandName="Inserimento">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
											</Columns>
											<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
										</asp:datagrid></DIV>
									</asp:Panel>
								</td>
							</tr>

							<tr>
								<td>
								    <asp:Panel ID="pnl_uo" runat="server">
									<!-- RISULTATO DELLA RICERCA -->
									<DIV id="div_uo" runat="server" style="OVERFLOW: auto; HEIGHT: 331px">
									    <asp:datagrid id="dg_UOTrovatiInRF" tabIndex="3" runat="server" Width="430px" OnPreRender="dg_UOTrovatiInRF_PreRender"
											ShowHeader="False" BorderColor="Gray" BorderWidth="1px" CellPadding="1" 
                                 AutoGenerateColumns="False" OnItemCommand="dg_UOTrovatiInRF_ItemCommand" 
                                 AllowPaging="True" 
                                 onpageindexchanged="dg_UOTrovatiInRF_PageIndexChanged">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
											<Columns>
												<asp:BoundColumn DataField="codice" ReadOnly="True" HeaderText="Codice">
													<ItemStyle Width="25%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDUO" ReadOnly="True" HeaderText="ID"></asp:BoundColumn>
												<asp:BoundColumn DataField="descrizione" ReadOnly="True" HeaderText="Descrizione">
													<ItemStyle Width="70%"></ItemStyle>
												</asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/freccia_dx.gif border=0 alt='Inserisci'&gt;" CommandName="Inserimento">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
											</Columns>
											<PagerStyle HorizontalAlign="Center" BackColor="#EAEAEA" CssClass="bg_grigioN" Mode="NumericPages"></PagerStyle>
										</asp:datagrid></DIV>
								    </asp:Panel>
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
									height="25">Ruoli in RF</td>
								<td width="208" background="../Images/pixel.gif" height="25"></td>
							</tr>
						</table>
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td valign=top align=left>
									<DIV style="OVERFLOW: auto; HEIGHT: 369px">
									    <asp:label id="lbl_risultatoRuoliRF" tabIndex="4" CssClass="testo_grigio_scuro_grande"
											Runat="server" Visible="False">Nessun ruolo presente.</asp:label>
									    <asp:datagrid id="dg_ruoli" tabIndex="5" runat="server" Width="100%" ShowHeader="False" BorderColor="Gray"
											BorderWidth="1px" CellPadding="1" AutoGenerateColumns="False" OnItemCommand="dg_ruoli_ItemCommand" OnItemCreated="dg_ruoli_ItemCreated">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="IDRuolo" ReadOnly="True"></asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="codice" ReadOnly="True"></asp:BoundColumn>
												<asp:BoundColumn DataField="descrizione" ReadOnly="True">
													<ItemStyle Width="95%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDAmministrazione" ReadOnly="True"></asp:BoundColumn>
												<asp:BoundColumn Visible="False" DataField="IDGruppo" ReadOnly="True"></asp:BoundColumn>
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
</html>
