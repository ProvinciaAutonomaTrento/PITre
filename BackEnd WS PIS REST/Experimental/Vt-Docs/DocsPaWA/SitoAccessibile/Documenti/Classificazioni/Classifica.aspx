<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<%@ Register TagPrefix="uc1" TagName="DettagliGenerali" Src="../DettagliGenerali.ascx" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Ricerca" Assembly="DocsPAWA" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../../MainMenu.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="Classifica.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Documenti.Classificazioni.Classifica" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MenuDocumento" Src="../MenuDocumento.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Classifica</title>
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
				<div class="skipLinks"><A href="#content">Vai al contenuto</A> <A href="#docsMenu">Vai 
						al menu documento</A> <A href="#navbar">Vai al menu utente</A>
				</div>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<uc1:MenuDocumento id="MenuDocumento" runat="server"></uc1:MenuDocumento>
				<div id="content">
					<uc1:DettagliGenerali id="DatiGeneraliDocumento" runat="server"></uc1:DettagliGenerali>
					<p id="pnlClassMsg" runat="server" class="message">
						Nessuna classificazione trovata
					</p>
					<wcag:accessibledatagrid id="grdFascicoli" runat="server" Caption="Elenco fascicoli contenenti il documento"
						summary="Elenco fascicoli contenenti il documento" PageSize="2" AutoGenerateColumns="False" UseAccessibleHeader="True">
						<Columns>
							<asp:BoundColumn Visible="False" DataField="ID"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="IDRegistro"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Codice">
								<HeaderStyle Width="15%"></HeaderStyle>
								<ItemTemplate>
									<asp:Label id="Label1" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Codice") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Descrizione">
								<HeaderStyle Width="70%"></HeaderStyle>
								<ItemTemplate>
									<asp:Label id="Label3" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Descrizione") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Registro">
								<HeaderStyle Width="10%"></HeaderStyle>
								<ItemTemplate>
									<asp:Label id="Label4" runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Registro") %>'>
									</asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:TemplateColumn HeaderText="Azioni">
								<HeaderStyle Width="5%"></HeaderStyle>
								<ItemTemplate>
									<asp:Button id="btnShowFascicolo" runat="server" Text="Apri" CssClass="gridButton" CommandName="SHOW_FASCICOLO"></asp:Button>
								</ItemTemplate>
							</asp:TemplateColumn>
						</Columns>
					</wcag:accessibledatagrid>
					<p class="centerButtons">
						<asp:button id="btnBack" CssClass="button" runat="server" Text="Torna ai risultati"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
