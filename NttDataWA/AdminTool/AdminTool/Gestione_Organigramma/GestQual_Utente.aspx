<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GestQual_Utente.aspx.cs" Inherits="SAAdminTool.AdminTool.Gestione_Organigramma.GestQual_Utente" %>
<%@ Register src="../../UserControls/AppTitleProvider.ascx" tagname="AppTitleProvider" tagprefix="uct" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<html>
<head runat="server">
    <title></title>
    <LINK href="../CSS/AmmStyle.css" type="text/css" rel="stylesheet">
    <base target="_self">    
</head>
<body bottomMargin="2" leftMargin="2" topMargin="2" rightMargin="2" MS_POSITIONING="GridLayout">
    <form id="form_gestioneQualifiche" runat="server">
        <uct:AppTitleProvider ID="appTitleProvider" runat="server" PageName = "AMMINISTRAZIONE > Gestione qualifiche per utente" />	
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
									height="25">Lista qualifiche</td>
								<td width="320" background="../Images/pixel.gif" height="25"></td>
							</tr>
						</table>
						<table class="contenitore">
							<tr>
								<td>
									<!-- RISULTATO DELLA RICERCA -->
									<DIV style="OVERFLOW: auto; HEIGHT: 331px"><asp:datagrid id="dg_qualifiche" tabIndex="3" runat="server" Width="400px" PageSize="15" AllowPaging="True"
											ShowHeader="False" BorderColor="Gray" BorderWidth="1px" CellPadding="1" AutoGenerateColumns="False">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<HeaderStyle CssClass="menu_1_bianco_dg"></HeaderStyle>
											<Columns>
												<asp:BoundColumn Visible="False" DataField="idQual" ReadOnly="True" HeaderText="idQual"></asp:BoundColumn>
												<asp:BoundColumn DataField="Codice" ReadOnly="True" HeaderText="Codice">
													<ItemStyle Width="20%"></ItemStyle>
												</asp:BoundColumn>
												<asp:BoundColumn DataField="Descrizione" ReadOnly="True" HeaderText="Descrizione">
													<ItemStyle Width="80%"></ItemStyle>
												</asp:BoundColumn>
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
									height="25">Qual. utente</td>
								<td width="208" background="../Images/pixel.gif" height="25"></td>
							</tr>
						</table>
						<table cellSpacing="0" cellPadding="0" width="100%" border="0">
							<tr>
								<td>
									<DIV style="OVERFLOW: auto; HEIGHT: 369px"><asp:label id="lbl_risultatoUtentiQualifiche" tabIndex="4" CssClass="testo_grigio_scuro_grande"
											Runat="server" Visible="false">Nessuna qualifica associata.</asp:label><asp:datagrid id="dg_utente" tabIndex="5" runat="server" Width="285px" ShowHeader="False" BorderColor="Gray"
											BorderWidth="1px" CellPadding="1" AutoGenerateColumns="False">
											<SelectedItemStyle CssClass="bg_grigioS"></SelectedItemStyle>
											<AlternatingItemStyle CssClass="bg_grigioA"></AlternatingItemStyle>
											<ItemStyle CssClass="bg_grigioN"></ItemStyle>
											<Columns>
                                                <asp:BoundColumn Visible="False" DataField="system_id" ReadOnly="True"></asp:BoundColumn>
												<asp:BoundColumn DataField="Descrizione_utente" ReadOnly="True">
													<ItemStyle Width="95%"></ItemStyle>
												</asp:BoundColumn>
												<asp:ButtonColumn Text="&lt;img src=../Images/cestino.gif border=0 alt='Elimina'&gt;" CommandName="Eliminazione">
													<HeaderStyle Width="5%"></HeaderStyle>
													<ItemStyle HorizontalAlign="Center"></ItemStyle>
												</asp:ButtonColumn>
											</Columns>
										</asp:datagrid></DIV>
								</td>
							</tr>
						</table>
                     </td><!-- fine frame DX-->
				</tr>
			</table>

    </form>
</body>
</html>
