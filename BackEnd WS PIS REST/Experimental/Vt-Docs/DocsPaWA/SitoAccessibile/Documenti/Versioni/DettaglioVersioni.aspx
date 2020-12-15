<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../../MainMenu.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="DettaglioVersioni.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Documenti.Versioni.DettaglioVersioni" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MenuDocumento" Src="../MenuDocumento.ascx" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Ricerca" Assembly="DocsPAWA" %>
<%@ Register TagPrefix="cc1" Namespace="DocsPaWebCtrlLibrary" Assembly="DocsPaWebCtrlLibrary" %>
<%@ Register TagPrefix="uc1" TagName="DettagliGenerali" Src="../DettagliGenerali.ascx" %>
<%@ Register TagPrefix="wcag" Namespace="UniSA.FLC.Web.UI.WebControls" Assembly="UniSA.FLC.Web.UI.WebControls.AccessibleDataGrid" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Versioni</title>
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
				<div class="skipLinks"><A href="#content">Vai al contenuto</A> <A href="#navbar">Vai al 
						menu utente</A>
				</div>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<uc1:MenuDocumento id="MenuDocumento" runat="server"></uc1:MenuDocumento>
				<div id="content">
					<uc1:DettagliGenerali id="DatiGeneraliDocumento" runat="server"></uc1:DettagliGenerali>
					<p id="pnlVersioniMsg" runat="server" class="message">
						Nessuna versione trovata
					</p>
					<wcag:AccessibleDataGrid id="grdVersioni" runat="server" AutoGenerateColumns="False" PageSize="2" summary="Elenco versioni del documento"
						Caption="Elenco versioni del documento" UseAccessibleHeader="True">
						<Columns>
							<asp:BoundColumn DataField="Versione" HeaderText="Versione">
								<HeaderStyle Width="10%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Note" HeaderText="Note">
								<HeaderStyle Width="70%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn DataField="Data" HeaderText="Data">
								<HeaderStyle Width="10%"></HeaderStyle>
							</asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="VersionID"></asp:BoundColumn>
							<asp:BoundColumn Visible="False" DataField="Acquisito"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Azioni">
								<HeaderStyle Width="10%"></HeaderStyle>
							</asp:TemplateColumn>
						</Columns>
						<PagerStyle CssClass="pager"></PagerStyle>
					</wcag:AccessibleDataGrid>
					<asp:panel id="azioniAllegati" runat="server" visible="false"></asp:panel>
					<p class="centerButtons">
						<asp:button id="btnBack" CssClass="button" runat="server" Text="Torna ai risultati"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
