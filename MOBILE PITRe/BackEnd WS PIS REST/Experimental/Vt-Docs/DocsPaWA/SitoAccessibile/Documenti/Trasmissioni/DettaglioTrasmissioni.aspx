<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Ricerca" Assembly="DocsPAWA" %>
<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../../Paging/ListPagingNavigationControls.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DettTrasmRicevute" Src="../../Trasmissioni/DettTrasmRicevute.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../../MainMenu.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="DettaglioTrasmissioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Documenti.Trasmissioni.DettaglioTrasmissioni" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MenuDocumento" Src="../MenuDocumento.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DettTrasmEffettuate" Src="../../Trasmissioni/DettTrasmEffettuate.ascx" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile.Ricerca" %>
<%@ Import Namespace="DocsPAWA.SitoAccessibile" %>
<%@ Import Namespace="DocsPAWA" %>
<%@ Import Namespace="DocsPAWA.DocsPaWR" %>
<%@ Register TagPrefix="uc1" TagName="DettagliGenerali" Src="../DettagliGenerali.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.1//EN"
"http://www.w3.org/TR/xhtml11/DTD/xhtml11.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Trasmissioni</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta content="DocsPA, Protocollo informatico" name="keywords">
		<meta content="Sistema per la gestione del protocollo informatico" name="description">
		<LINK title="default" media="screen" href="../../Css/main.css" type="text/css" rel="stylesheet">
			<LINK media="screen" href="../../Css/menu.css" type="text/css" rel="stylesheet">
				<LINK media="screen" href="../../Css/docsMenu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" name="docDetails" method="post" runat="server">
			<div id="container">
				<div class="skipLinks"><A id="skipTrasmissioni" href="#grdTrasmissioni" runat="server">Vai 
						alle trasmissioni trovate</A> <A href="#content">Vai al contenuto</A> <A href="#docsMenu">
						Vai al menu documento</A> <A href="#navbar">Vai al menu utente</A>
				</div>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<uc1:MenuDocumento id="MenuDocumento" runat="server"></uc1:MenuDocumento>
				<div id="content">
					<uc1:dettagligenerali id="DatiGeneraliDocumento" runat="server"></uc1:dettagligenerali>
					<p class="message" id="pnlNoTrasm" runat="server">Nessuna trasmissione trovata
					</p>
					<fieldset class="filtroTrasmissioni">
						<legend>Filtro trasmissioni</legend>
						<asp:radiobuttonlist id="rblSearchType" runat="server" TextAlign="Left" RepeatDirection="Horizontal"
							RepeatLayout="Flow">
							<asp:ListItem Value="Effettuate">Effettuate</asp:ListItem>
							<asp:ListItem Value="Ricevute">Ricevute</asp:ListItem>
						</asp:radiobuttonlist>
						<asp:button id="btnSearch" runat="server" Text="Ricerca" CssClass="button"></asp:button>
					</fieldset>
					<wcag:accessibledatagrid id="grdTrasmissioni" runat="server" Caption="Elenco trasmissioni" summary="Elenco delle trasmissioni"
						PageSize="5" UseAccessibleHeader="True" AutoGenerateColumns="False">
						<Columns>
							<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
							<asp:BoundColumn DataField="DataInvio" HeaderText="Data invio"></asp:BoundColumn>
							<asp:BoundColumn DataField="Utente" HeaderText="Utente"></asp:BoundColumn>
							<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Azioni">
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate>
									<asp:Button id="btnDetailTrasmissioniEffettuate" runat="server" CssClass="gridButton" Text="Dettagli"
										CommandName="SHOW_DETAILS"></asp:Button>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</wcag:accessibledatagrid>
					<uc1:listpagingnavigationcontrols id="pagingTrasmEffettuate" runat="server" Visible="False"></uc1:listpagingnavigationcontrols>
					<uc1:DettTrasmEffettuate id="DettTrasmEffettuate" runat="server" Visible="False"></uc1:DettTrasmEffettuate>
					<wcag:accessibledatagrid id="grdTrasmissioniRicevute" runat="server" Caption="Elenco trasmissioni" summary="Elenco delle trasmissioni"
						PageSize="5" AutoGenerateColumns="False" UseAccessibleHeader="True">
						<Columns>
							<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
							<asp:BoundColumn DataField="DataInvio" HeaderText="Data invio"></asp:BoundColumn>
							<asp:BoundColumn DataField="Mittente" HeaderText="Mittente"></asp:BoundColumn>
							<asp:BoundColumn DataField="Ruolo" HeaderText="Ruolo"></asp:BoundColumn>
							<asp:BoundColumn DataField="Ragione" HeaderText="Ragione"></asp:BoundColumn>
							<asp:BoundColumn DataField="DataScadenza" HeaderText="Data scadenza"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Azioni">
								<ItemStyle HorizontalAlign="Center"></ItemStyle>
								<ItemTemplate>
									<asp:Button id="btnDetailTrasmissioniRicevute" runat="server" CssClass="gridButton" Text="Dettagli"
										CommandName="SHOW_DETAILS"></asp:Button>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</wcag:accessibledatagrid>
					<uc1:listpagingnavigationcontrols id="pagingTrasmRicevute" runat="server" Visible="False"></uc1:listpagingnavigationcontrols>
					<uc1:DettTrasmRicevute id="DettTrasmRicevute" runat="server" Visible="False"></uc1:DettTrasmRicevute>
					<p class="centerButtons">
						<asp:button id="btnBack" runat="server" CssClass="button" Text="Torna ai risultati"></asp:button></p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
