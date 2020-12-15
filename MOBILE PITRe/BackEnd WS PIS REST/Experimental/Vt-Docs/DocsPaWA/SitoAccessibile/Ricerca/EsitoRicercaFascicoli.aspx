<%@ Register TagPrefix="uc1" TagName="ListPagingNavigationControls" Src="../Paging/ListPagingNavigationControls.ascx" %>
<%@ Page language="c#" Codebehind="EsitoRicercaFascicoli.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Ricerca.EsitoRicercaFascicoli" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title><%
            string titolo = System.Configuration.ConfigurationManager.AppSettings["TITLE"];
            if (titolo != null)
            {
             %>
                <%= titolo%>
             <%
                   }
            else
            {
             %>
                DOCSPA
		     <%} %>  > Ricerca - Fascicoli trovati</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta name="keywords" content="DocsPA, Protocollo informatico">
		<meta name="description" content="Sistema per la gestione del protocollo informatico">
		<link rel="stylesheet" href="../Css/main.css" type="text/css" media="screen" title="default">
			<link rel="stylesheet" href="../Css/menu.css" type="text/css" media="screen">
	</HEAD>
	<body>
		<form id="frmEsitoRicercaDocumenti" runat="server" method="post" class="searchResults">
			<div id="container">
				<div class="skipLinks">
					<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
				</div>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<div id="content">
					<p id="pnlMessage" runat="server" class="message">
					</p>
					<wcag:AccessibleDataGrid id="grdEsitoRicercaFascicoli" runat="server" AutoGenerateColumns="False" UseAccessibleHeader="True"
						Width="100%" summary="Elenco dei fascicoli trovati">
						<Columns>
							<asp:BoundColumn Visible="False" DataField="ID" HeaderText="ID"></asp:BoundColumn>
							<asp:BoundColumn DataField="Tipo" HeaderText="Tipo">
								<HeaderStyle Width="5%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Codice" HeaderText="Codice">
								<HeaderStyle Width="15%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Descrizione" HeaderText="Descrizione">
								<HeaderStyle Width="50%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="DataApertura" HeaderText="Data apertura">
								<HeaderStyle Width="10%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="DataChiusura" HeaderText="Data chiusura">
								<HeaderStyle Width="10%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Azioni">
								<HeaderStyle Width="10%"></HeaderStyle>
								<ItemTemplate>
									<asp:Button id="btnDetails" runat="server" CssClass="gridButton" Text="Apri" CommandName="SHOW_DETAILS"></asp:Button>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</wcag:AccessibleDataGrid>
					<uc1:ListPagingNavigationControls id="ListPagingNavigationControl" runat="server"></uc1:ListPagingNavigationControls>
					<p class="centerButtons">
						<asp:Button id="btnBack" CssClass="button" Text="Torna alla ricerca" runat="server" />
					</p>
				</div> <!-- End content -->
			</div> <!-- End container -->
		</form>
	</body>
</HTML>
