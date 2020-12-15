<%@ Register TagPrefix="uc1" TagName="UserContext" Src="../UserContext.ascx" %>
<%@ Register TagPrefix="uc1" TagName="DocumentiFascicolo" Src="DocumentiFascicolo.ascx" %>
<%@ Register TagPrefix="uc1" TagName="MainMenu" Src="../MainMenu.ascx" %>
<%@ Page language="c#" Codebehind="DettagliDocumentiFascicolo.aspx.cs" AutoEventWireup="false" Inherits="DocsPAWA.SitoAccessibile.Fascicoli.DettagliDocumentiFascicolo" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Strict//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd">
<HTML>
	<HEAD>
		<title>DOCSPA > Ricerca - Documenti del fascicolo</title>
		<meta http-equiv="Content-Type" content="text/html;charset=UTF-8">
		<meta http-equiv="Content-Language" content="it-IT">
		<meta content="DocsPA, Protocollo informatico" name="keywords">
		<meta content="Sistema per la gestione del protocollo informatico" name="description">
		<LINK title="default" media="screen" href="../Css/main.css" type="text/css" rel="stylesheet">
			<LINK media="screen" href="../Css/menu.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body>
		<form id="frmDettagliDocumentiFascicolo" method="post" runat="server">
			<div id="container">
				<div class="skipLinks"><A href="#content">Vai al contenuto</A> <A href="#docsMenu">Vai 
						al menu documento</A> <A href="#navbar">Vai al menu utente</A>
				</div>
				<div id="header"><uc1:usercontext id="UserContext" runat="server"></uc1:usercontext></div>
				<uc1:mainmenu id="MainMenu" runat="server"></uc1:mainmenu>
				<div id="content">
					
					<uc1:documentifascicolo id="DocumentiFascicolo" runat="server"></uc1:documentifascicolo>
					<p class="centerButtons"><asp:button id="btnBack" runat="server" Text="Torna alla ricerca" CssClass="button"></asp:button></p>
				</div> <!-- end content --></div> <!-- end container --></form>
	</body>
</HTML>
