<%@ Register TagPrefix="uc1" TagName="DettagliGenerali" Src="../Documenti/DettagliGenerali.ascx" %>
<%@ Register TagPrefix="sa" Namespace="DocsPAWA.SitoAccessibile.Ricerca" Assembly="DocsPAWA" %>
<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Page EnableSessionState="true" language="c#" Codebehind="DettagliDocumento.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Documenti.DettagliDocumento" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MenuDocumento" Src="../Documenti/MenuDocumento.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DettagliProfilo" Src="../Documenti/DettagliProfilo.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DettagliProtocollo" Src="../Documenti/DettagliProtocollo.ascx" %>
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
		     <%} %>  > Dettagli del documento</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta content="DocsPA, Protocollo informatico" name="keywords">
		<meta content="Sistema per la gestione del protocollo informatico" name="description">
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet">
			<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet">
				<LINK media="screen" href="../Css/docsMenu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="Form1" name="docDetails" method="post" runat="server">
			<div id="container">
				<div class="skipLinks">
					<A href="#content">Vai al contenuto</A> <A href="#docsMenu">Vai al menu documento</A>
					<A href="#navbar">Vai al menu utente</A>
				</div>
					
				<uc1:UserContext id="UserContext" runat="server"></uc1:UserContext>
				<uc1:MainMenu id="MainMenu" runat="server"></uc1:MainMenu>
				<uc1:MenuDocumento id="MenuDocumento" runat="server"></uc1:MenuDocumento>
				<div id="content">
					<div id="containerDettagliDocumento" runat="server">
						<uc1:DettagliGenerali id="DettagliGenerali" runat="server"></uc1:DettagliGenerali>
						<uc1:DettagliProtocollo id="DettagliProtocollo" runat="server"></uc1:DettagliProtocollo>
						<uc1:DettagliProfilo id="DettagliProfilo" runat="server"></uc1:DettagliProfilo>
					</div>
					<p class="centerButtons">
						<asp:button id="btnBack" runat="server" Text="Torna ai risultati" CssClass="button"></asp:button>
						<asp:button id="btnShowDocumento" Runat="server" Text="Visualizza contenuto documento" CssClass="button"></asp:button>
					</p>
				</div> <!-- end content -->
			</div> <!-- end container -->
		</form>
	</body>
</HTML>
