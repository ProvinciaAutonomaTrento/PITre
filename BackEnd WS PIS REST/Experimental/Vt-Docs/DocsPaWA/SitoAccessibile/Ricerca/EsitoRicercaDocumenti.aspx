<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ListaDocumentiProtocollati" Src="ListaDocumentiProtocollati.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="EsitoRicercaDocumenti.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Ricerca.EsitoRicercaDocumenti" %>
<%@ Register TagPrefix="uc1" TagName="ListaDocumentiGrigi" Src="ListaDocumentiGrigi.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ListaDocumentiUnificati" Src="ListaDocumentiUnificati.ascx" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Ricerca" Assembly="DocsPAWA" %>
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
		     <%} %>  > Ricerca - Documenti trovati</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta name="keywords" content="DocsPA, Protocollo informatico">
		<meta name="description" content="Sistema per la gestione del protocollo informatico">
		<link rel="stylesheet" href="../Css/main.css" type="text/css" media="screen" title="default">
			<link rel="stylesheet" href="../Css/menu.css" type="text/css" media="screen">
	</HEAD>
	<body>
		<form id="back" method="post" action="EsitoRicercaDocumenti.aspx" class="searchResults"
			runat="server">
			<div id="container">
				<div class="skipLinks">
					<a href="#content">Vai al contenuto</a> <a href="#navbar">Vai al menu utente</a>
				</div>
					<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<div id="content">
					<uc1:ListaDocumentiUnificati id="listaDocumentiUnificati" runat="server"></uc1:ListaDocumentiUnificati>
					<p class="centerButtons">
						<asp:Button id="btnBack" CssClass="button" Text="Torna alla ricerca" runat="server" />
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
