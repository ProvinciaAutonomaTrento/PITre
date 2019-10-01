<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Register TagPrefix="uc1" TagName="DettagliGenerali" Src="../DettagliGenerali.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Ricerca" Assembly="DocsPAWA" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../../UserContext.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="DettaglioAllegati.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Documenti.Allegati.DettaglioAllegati" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../../MainMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MenuDocumento" Src="../MenuDocumento.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Allegati</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta content="DocsPA, Protocollo informatico" name="keywords">
		<meta content="Sistema per la gestione del protocollo informatico" name="description">
		<link title="default" media="screen" href="../../Css/main.css" type="text/css" rel="stylesheet">
			<link media="screen" href="../../Css/menu.css" type="text/css" rel="stylesheet">
				<link media="screen" href="../../Css/docsMenu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" name="Form1" runat="server" method="post">
			<div id="container">
				<div class="skipLinks">
					<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
				</div>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<uc1:MenuDocumento id="MenuDocumento" runat="server"></uc1:MenuDocumento>
				<div id="content">
					<uc1:DettagliGenerali id="DatiGeneraliDocumento" runat="server"></uc1:DettagliGenerali>
					<p id="pnlAllegatiMsg" runat="server" class="message">
						Nessun allegato trovato
					</p>
					<wcag:AccessibleDataGrid id="grdAllegati" runat="server" AutoGenerateColumns="False" PageSize="2" summary="Elenco allegati del documento"
						Caption="Elenco allegati del documento" UseAccessibleHeader="True">
						<Columns>
							<asp:BoundColumn DataField="Codice" HeaderText="Codice">
								<HeaderStyle Width="25%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
								<HeaderStyle Width="45%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="NumeroPagine" HeaderText="Pagine">
								<HeaderStyle Width="5%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="VersionID"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="Acquisito"></asp:BoundColumn>
							<asp:TemplateColumn>
								<HeaderStyle Width="10%"></HeaderStyle>
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate></ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="pager"></PagerStyle>
					</wcag:AccessibleDataGrid>
					<asp:panel id="azioniAllegati" runat="server" visible="false">
						<cc1:imagebutton id="btn_aggAlleg" ImageUrl="../images/btn_nuovo_attivo.gif" AlternateText="Nuovo allegato"
							Runat="server" DisabledUrl="../images/btn_nuovo_nonattivo.gif" Tipologia="DO_ALL_AGGIUNGI"></cc1:imagebutton>
						<cc1:imagebutton id="btn_modifAlleg" ImageUrl="../images/btn_modifica_attivo.gif" AlternateText="Modifica allegato"
							Runat="server" DisabledUrl="../images/btn_modifica_nonattivo.gif" Tipologia="DO_ALL_MODIFICA"></cc1:imagebutton>
						<cc1:imagebutton id="btn_aggiungiAreaLav" ImageUrl="../images/btn_area_attivo.gif" AlternateText="Aggiungi ad Area di lavoro"
							Runat="server" DisabledUrl="../images/btn_area_nonattivo.gif" Tipologia="DO_ADD_ADL"></cc1:imagebutton>
						<cc1:imagebutton id="btn_sostituisciDocPrinc" ImageUrl="../images/btn_scambia_attivo.gif" AlternateText="Scambia con documento principale"
							Runat="server" DisabledUrl="../images/btn_scambia_nonattivo.gif" Tipologia="DO_ALL_SOSTITUISCI"
							Enabled="False"></cc1:imagebutton>
						<cc1:imagebutton id="btn_rimuoviAlleg" ImageUrl="../images/btn_rimuovi_attivo.gif" AlternateText="Rimuovi allegato"
							Runat="server" DisabledUrl="../images/btn_rimuovi_nonattivo.gif" Tipologia="DO_ALL_RIMUOVI"></cc1:imagebutton>
					</asp:panel>
					<p class="centerButtons">
						<asp:button id="btnBack" CssClass="button" runat="server" Text="Torna ai risultati"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
